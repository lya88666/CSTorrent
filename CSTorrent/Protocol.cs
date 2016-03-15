using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace CSTorrent
{
    public enum ActionType { CONNECT = 0, ANNOUNCE = 1, SCRAPE = 2, ERROR = 3 };
    public enum EventType { NONE = 0, COMPLETED = 1, STARTED = 2, STOPPED = 3 };
    public class ConnectRequest
    {
        public Int64 connection_id {set;get;}
        public Int32 action {set;get;}
        public Int32 transaction_id { set; get; }
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(connection_id)), 0, bytes, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(action)), 0, bytes, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(transaction_id)), 0, bytes, 12, 4);

            return bytes;
        }
    }

    public class ConnectResponse
    {
        private ConnectResponse(Int32 a, Int32 t, Int64 c)
        {
        }
        public Int32 action { set; get; }
        public Int32 transaction_id { set; get; }
        public Int64 connection_id { set; get; }

        public static ConnectResponse Parse(byte[] bytes, Int32 expected_transaction_id)
        {
            //Check whether the packet is at least 16 bytes
            if (bytes.Length < 16)
                return null;

            Int32 action = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));
            Int32 transaction_id = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 4));
            Int64 connection_id = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(bytes, 8));

            if (transaction_id != expected_transaction_id)
                return null;

            return new ConnectResponse(action, transaction_id, connection_id);
        }
    }

    public class AnnounceRequest
    {
        //0	64-bit integer	connection_id
        public Int64 connection_id;
        //8	32-bit integer	action	1
        public Int32 action = 1;
        //12	32-bit integer	transaction_id
        public Int32 transaction_id;
        //16	20-byte string	info_hash
        public byte[] info_hash = new byte[20];
        //36	20-byte string	peer_id
        public byte[] peer_id = new byte[20];
        //56	64-bit integer	downloaded
        public Int64 downloaded;
        //64	64-bit integer	left
        public Int64 left;
        //72	64-bit integer	uploaded
        public Int64 uploaded;
        //80	32-bit integer	event
        public Int32 evnt;
        //84	32-bit integer	IP address	0
        public Int32 ipaddr = 0;
        //88	32-bit integer	key
        public Int32 key;
        //92	32-bit integer	num_want	-1
        public Int32 num_want = -1;
        //96	16-bit integer	port 
        public Int16 port;
        //98

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[98];

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(connection_id)), 0, bytes, 0, 8);//connection_id
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(action)), 0, bytes, 8, 4);//action
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(transaction_id)), 0, bytes, 12, 4);//transaction_id
            Buffer.BlockCopy(info_hash, 0, bytes, 16, 20);//info_hash
            Buffer.BlockCopy(peer_id, 0, bytes, 36, 20);//peer_id
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(downloaded)), 0, bytes, 56, 8);//downloaded
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(left)), 0, bytes, 64, 8);//left
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(left)), 0, bytes, 72, 8);//uploaded
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(evnt)), 0, bytes, 80, 4);//event
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(ipaddr)), 0, bytes, 84, 4);//ipaddr
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(key)), 0, bytes, 88, 4);//key
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(num_want)), 0, bytes, 92, 4);//num_want
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(port)), 0, bytes, 96, 2);//port

            return bytes;
        }
    }

    public class AnnounceResponse
    {
        //0           32-bit integer  action          1 // announce
        public Int32 action;
        //4           32-bit integer  transaction_id
        public Int32 transaction_id;
        //8           32-bit integer  interval
        public Int32 interval;
        //12          32-bit integer  leechers
        public Int32 leechers;
        //16          32-bit integer  seeders
        public Int32 seeders;
        //20 + 6 * n  32-bit integer  IP address
        //24 + 6 * n  16-bit integer  TCP port
        //20 + 6 * N
        public IPEndPoint[] ipeps; //an array of ip/port pairs

        public static AnnounceResponse Parse(byte[] bytes, Int32 expected_transaction_id)
        {
            if (bytes.Length < 20)
                return null;

            AnnounceResponse announceResponse = new AnnounceResponse();

            announceResponse.action = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));

            if (announceResponse.action != (int)ActionType.ANNOUNCE)
                return null;

            announceResponse.transaction_id = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 4));

            if (announceResponse.transaction_id != expected_transaction_id)
                return null;

            announceResponse.interval = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 8));
            announceResponse.leechers = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 12));
            announceResponse.seeders = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 16));

            int N = (bytes.Length - 20) / 6;

            announceResponse.ipeps = new IPEndPoint[N];

            for (int n = 0; n < N; ++n)
            {
                
                UInt32 ipaddress = BitConverter.ToUInt32(bytes, 20 + 6 * n);
                UInt16 port = (UInt16)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, 24 + 6 * n));

                try
                {
                    IPEndPoint ipep = new IPEndPoint((Int64)ipaddress, (Int32)port);

                    announceResponse.ipeps[n] = ipep;
                }
                catch (Exception)
                {
                    continue;
                }
            }       

            return announceResponse;
        }
    }

    public class ScrapeRequest
    {
        //0               64-bit integer  connection_id
        public Int64 connection_id;
        //8               32-bit integer  action          2 // scrape
        public const Int32 action = (int)ActionType.SCRAPE;
        //12              32-bit integer  transaction_id
        public Int32 transaction_id;
        public List<byte[]> info_hash_list;
        //16 + 20 * n     20-byte string  info_hash
        //16 + 20 * N
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[16 + 20 * info_hash_list.Count];

            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(connection_id)), 0, bytes, 0, 8);//connection_id
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(action)), 0, bytes, 8, 4);//action
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(transaction_id)), 0, bytes, 12, 4);//transaction_id

            for (int i = 0; i < info_hash_list.Count; ++i)
            {
                Buffer.BlockCopy(info_hash_list[i], 0, bytes, 16 + 20 * i, 20);//info_hash
            }

            return bytes;
        }
    }

    public class ScrapeInfo
    {
        public Int32 seeders;
        public Int32 completed;
        public Int32 leechers;
    }

    public class ScrapeResponse
    {
        //0           32-bit integer  action          2 // scrape
        public Int32 action;
        //4           32-bit integer  transaction_id
        public Int32 transaction_id;
        //8 + 12 * n  32-bit integer  seeders
        //12 + 12 * n 32-bit integer  completed
        //16 + 12 * n 32-bit integer  leechers
        public List<ScrapeInfo> scrapeInfoList;
        //8 + 12 * N

        public static ScrapeResponse Parse(byte[] bytes, Int32 expected_transaction_id)
        {
            if (bytes.Length < 20)
                return null;

            ScrapeResponse scrapeResponse = new ScrapeResponse();

            scrapeResponse.action = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 0));

            if (scrapeResponse.action != (int)ActionType.SCRAPE)
                return null;

            scrapeResponse.transaction_id = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 4));
            if (scrapeResponse.transaction_id != expected_transaction_id)
                return null;

            int N = (bytes.Length - 8) / 12;

            if (N > 0)
                scrapeResponse.scrapeInfoList = new List<ScrapeInfo>();

            for (int n = 0; n < N; ++n)
            {
                ScrapeInfo si = new ScrapeInfo();

                si.seeders = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 8 + 12 * n));
                si.completed = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 12 + 12 * n));
                si.leechers = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 16 + 12 * n));

                scrapeResponse.scrapeInfoList.Add(si);
            }

            return scrapeResponse;
        }
    }

    public class Protocol
    {
        public const Int64 INVALID_CONNECTION_ID = -1;
    }
}

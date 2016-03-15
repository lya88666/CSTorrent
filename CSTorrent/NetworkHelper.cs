using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace CSTorrent
{
    public class NetworkHelper
    {
        //public UdpClient
        public static bool SendAndReceive(string host, int port, byte[] data, ref byte[] response)
        {
            
            //get host address
            try
            {
                IPAddress[] addrs = Dns.GetHostAddresses(host);

                if (addrs.Length == 0)
                    return false;

                IPAddress addr = null;

                for (int i = 0; i < addrs.Length; ++i)
                {
                    //ignore loopbacks
                    if (IPAddress.IsLoopback(addrs[i]))
                        continue;

                    addr = addrs[i];
                    break;
                }

                //no valid ip address is found
                if (addr == null)
                    return false;

                //construct end point
                IPEndPoint ipep = new IPEndPoint(addr, port);

                //construct udp client
                UdpClient uc = new UdpClient();
                uc.Client.SendTimeout = 1000;
                uc.Client.ReceiveTimeout = 1000;
                
                //send request
                uc.Send(data, data.Length, ipep);

                IPEndPoint remote= new IPEndPoint(IPAddress.Any, 0);

                response = uc.Receive(ref remote);
            }
            catch (SocketException)
            {
                return false;
            }
            
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace CSTorrent
{
    public class SHA1
    {
        public const int SHA1LENGTH = 20;

        public SHA1(SHA1 sha1)
        {
            this.value = new byte[SHA1.SHA1LENGTH];

            Buffer.BlockCopy(sha1.value, 0, value, 0, SHA1.SHA1LENGTH);
        }

        public SHA1(byte[] sha1)
        {
            this.value = new byte[SHA1.SHA1LENGTH];

            if (sha1.Length >= SHA1.SHA1LENGTH)
            {
                Buffer.BlockCopy(sha1, 0, value, 0, SHA1.SHA1LENGTH);
            }
        }

        public SHA1(byte[] bytes, int start)
        {
            this.value = new byte[SHA1.SHA1LENGTH];

            if (bytes.Length >= start + SHA1.SHA1LENGTH)
            {
                Buffer.BlockCopy(bytes, start, value, 0, SHA1.SHA1LENGTH);
            }
        }

        public byte[] value { set; get; }
    }

    public class FileInfo
    {
        public const string LENGTH = "length";
        public const string PATH = "path";
        public const string MD5SUM = "md5sum";

        public int length { set; get; }
        public List<string> path { set; get; }
        public string md5sum { set; get; }

        public bool Parse(BObject obj)
        {
            //the object type should be BDictionary
            if (obj.GetBType() != BType.BDICTIONARY)
                return false;

            foreach (KeyValuePair<BString, BObject> kvpair in (BDictionary)obj)
            {
                if (kvpair.Key.CompareTo(FileInfo.LENGTH) == 0)
                {
                    this.length = ((BInteger)kvpair.Value).GetInt();
                }
                else if (kvpair.Key.CompareTo(FileInfo.PATH) == 0)
                {
                    if (kvpair.Value.GetBType() != BType.BLIST)
                        return false;

                    this.path = new List<string>();

                    foreach (BObject o in (BList)kvpair.Value)
                    {
                        if(o.GetBType() != BType.BSTRING)
                            return false;

                        this.path.Add(((BString)o).GetString());
                    }
                }
                else if (kvpair.Key.CompareTo(FileInfo.MD5SUM) == 0)
                {
                    this.md5sum = ((BString)kvpair.Value).GetString();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unknown key in FileInfo: " + kvpair.Key.GetString());
                }
            }
            return true;
        }
    }

    public class DataInfo
    {
        public const string LENGTH = "length";
        public const string FILES = "files";
        public const string NAME = "name";
        public const string PIECELENGTH = "piece length";
        public const string PIECES = "pieces";
        public const string PRIVATE = "private";

        public List<FileInfo> files { set; get; }
        public int pieceLength { set; get; }
        public string name { set; get; }
        public byte[] pieces { set; get; }
        public int isPrivate { set; get; }
        public int totalLength { set; get; } //the total length of all data
        public List<SHA1> sha1List { set; get; } //the list of sha1

        public bool Parse(BObject obj)
        {
            //this object should be a BDictionary
            if (obj.GetBType() != BType.BDICTIONARY)
                return false;

            this.totalLength = 0;

            foreach (KeyValuePair<BString, BObject> kvpair in (BDictionary)obj)
            {
                if (kvpair.Key.CompareTo(DataInfo.FILES) == 0)
                {
                    if (kvpair.Value.GetBType() != BType.BLIST)
                        return false;

                    this.files = new List<FileInfo>();

                    foreach (BObject o in (BList)kvpair.Value)
                    {
                        FileInfo fi = new FileInfo();

                        if (!fi.Parse(o))
                            return false;

                        this.files.Add(fi);
                    }
                }
                else if (kvpair.Key.CompareTo(DataInfo.NAME) == 0)
                {
                    this.name = ((BString)kvpair.Value).GetString();
                }
                else if (kvpair.Key.CompareTo(DataInfo.PIECELENGTH) == 0)
                {
                    this.pieceLength = ((BInteger)kvpair.Value).GetInt();
                }
                else if (kvpair.Key.CompareTo(DataInfo.PIECES) == 0)
                {
                    this.pieces = ((BString)kvpair.Value).GetBytes();
                }
                else if (kvpair.Key.CompareTo(DataInfo.PRIVATE) == 0)
                {
                    this.isPrivate = ((BInteger)kvpair.Value).GetInt();
                }
                else if (kvpair.Key.CompareTo(DataInfo.LENGTH) == 0)
                {
                    this.totalLength = ((BInteger)kvpair.Value).GetInt();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unknown key in DataInfo: " + kvpair.Key.GetString());
                }
            }

            //calculate total length
            if (this.totalLength == 0)
            {

                foreach (FileInfo fi in this.files)
                {
                    this.totalLength += fi.length;
                }
            }

            //build sha1 list
            this.sha1List = new List<SHA1>();
            int start = 0;
            
            while (start < this.pieces.Length)
            {
                this.sha1List.Add(new SHA1(this.pieces, start));

                start += SHA1.SHA1LENGTH;
            }

            return true;
        }
    }

    public class MetaInfo
    {
        public const string ANNOUNCE = "announce";
        public const string ANNOUNCELIST = "announce-list";
        public const string COMMENT = "comment";
        public const string CREATEDBY = "created by";
        public const string CREATIONDATE = "creation date";
        public const string DURATION = "duration";
        public const string ENCODEDRATE = "encoded rate";
        public const string ENCODING = "encoding";
        public const string INFO = "info";
        
        public string announce { set; get; }
        public List<List<string>> announceList { set; get; }
        public DateTime createTime { set; get; }
        public string createdBy { set; get; }
        public string comment { set; get; }
        public int duration { set; get; }
        public int encodedRate { set; get; }
        public string encoding { set; get; }
        public DataInfo info;
        public byte[] infoHash;

        public bool Parse(BObject obj)
        {
            //can only parse dictionary here
            if (obj.GetBType() != BType.BDICTIONARY)
                return false;

            foreach (KeyValuePair<BString, BObject> kvpair in (BDictionary)obj)
            {
                if (kvpair.Key.CompareTo(MetaInfo.ANNOUNCE) == 0)
                {
                    this.announce = ((BString)kvpair.Value).GetString();
                }
                else if (kvpair.Key.CompareTo(MetaInfo.ANNOUNCELIST) == 0)
                {
                    if (kvpair.Value.GetBType() != BType.BLIST)
                        return false;

                    this.announceList = new List<List<string>>();

                    foreach (BObject o in (BList)kvpair.Value)
                    {
                        if (o.GetBType() != BType.BLIST)
                            return false;

                        List<string> l = new List<string>();

                        foreach (BObject o1 in (BList)o)
                        {
                            if (o1.GetBType() != BType.BSTRING)
                                return false;

                            l.Add(((BString)o1).GetString());
                        }

                        this.announceList.Add(l);
                    }
                    
                }
                else if (kvpair.Key.CompareTo(MetaInfo.COMMENT) == 0)
                {
                    this.comment = ((BString)kvpair.Value).GetString();
                }
                else if (kvpair.Key.CompareTo(MetaInfo.CREATEDBY) == 0)
                {
                    this.createdBy = ((BString)kvpair.Value).GetString();
                }
                else if (kvpair.Key.CompareTo(MetaInfo.CREATIONDATE) == 0)
                {
                    this.createTime = Utils.ConvertFromUnixTimestamp(((BInteger)kvpair.Value).GetInt());
                }
                else if (kvpair.Key.CompareTo(MetaInfo.DURATION) == 0)
                {
                    this.duration = ((BInteger)kvpair.Value).GetInt();
                }
                else if (kvpair.Key.CompareTo(MetaInfo.ENCODEDRATE) == 0)
                {
                    this.encodedRate = ((BInteger)kvpair.Value).GetInt();
                }
                else if (kvpair.Key.CompareTo(MetaInfo.ENCODING) == 0)
                {
                    this.encoding = ((BString)kvpair.Value).GetString();
                }
                else if (kvpair.Key.CompareTo(MetaInfo.INFO) == 0)
                {
                    this.info = new DataInfo();

                    if (!this.info.Parse(kvpair.Value))
                        return false;

                    HashAlgorithm hash = new SHA1Managed();
                    this.infoHash = hash.ComputeHash(BEncoder.Encode(kvpair.Value));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unknown key in MetaInfo: " + kvpair.Key.GetString());
                }
            }

            //validate data
            if (!ValidateData())
                return false;

            return true;
        }

        public bool ValidateData()
        {
            return true;
        }
    }
}

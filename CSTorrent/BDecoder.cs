using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSTorrent
{
    public class BDecoder
    {
        public static BObject Decode(byte[] bytes)
        {
            List<BObject> list = new List<BObject>();
            int length = bytes.Length;
            int start = 0;
            int pos = 0;

            BObject obj = BDecoder.DecodeOneObject(bytes, start, ref pos);

            //invalid format
            if (obj == null)
                return null;

            return obj;
        }
        public static BObject DecodeOneObject(byte[] bytes, int start, ref int pos)
        {
            if(start >= bytes.Length)
                return null;

            char c = (char)bytes[start];

            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    {
                        return BDecoder.DecodeBString(bytes, start, ref pos);
                    }
                case 'i':
                    {
                        return BDecoder.DecodeBInteger(bytes, start, ref pos);
                    }
                case 'l':
                    {
                        return BDecoder.DecodeBList(bytes, start, ref pos);
                    }
                case 'd':
                    {
                        return BDecoder.DecodeBDictionary(bytes, start, ref pos);
                    }
                default:
                    return null;
            }

        }

        public static BObject DecodeBInteger(byte[] bytes, int start, ref int pos)
        {
            BInteger bi = (BInteger)(new BInteger()).Decode(bytes, start, ref pos);
            //if failed to Decode as integer, the format is invalid
            if (bi == null)
                return null;

            return (BObject)bi;
        }

        public static BObject DecodeBString(byte[] bytes, int start, ref int pos)
        {
            BString bs = (BString)(new BString()).Decode(bytes, start, ref pos);
            //if failed to Decode as integer, the format is invalid
            if (bs == null)
                return null;

            return (BObject)bs;
        }

        public static BObject DecodeBList(byte[] bytes, int start, ref int pos)
        {
            BList bl = (BList)(new BList()).Decode(bytes, start, ref pos);
            if (bl == null)
                return null;

            return (BObject)bl;
        }

        public static BObject DecodeBDictionary(byte[] bytes, int start, ref int pos)
        {
            BDictionary bd = (BDictionary)(new BDictionary()).Decode(bytes, start, ref pos);
            if (bd == null)
                return null;

            return (BObject)bd;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSTorrent
{
    public class BDictionary : SortedDictionary<BString, BObject>, BObject
    {
        public static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

        public BType GetBType()
        {
            return BType.BDICTIONARY;
        }

        public object GetValue()
        {
            return (object)this;
        }

        public byte[] Encode()
        {
            //d<contents>e
            List<byte[]> byteList = new List<byte[]>();

            int totalLength = 0;

            foreach (KeyValuePair<BString, BObject> kvpair in this)
            {
                BString key = kvpair.Key;
                BObject value = kvpair.Value;

                byte[] tmp1 = key.Encode();
                byte[] tmp2 = value.Encode();

                totalLength += tmp1.Length + tmp2.Length;

                byteList.Add(tmp1);
                byteList.Add(tmp2);
            }

            byte[] bytes = new byte[totalLength + 2];
            bytes[0] = (byte)'d';
            bytes[totalLength + 1] = (byte)'e';

            int offset = 1;
            for (int i = 0; i < byteList.Count; ++i)
            {
                Buffer.BlockCopy(byteList[i], 0, bytes, offset, byteList[i].Length);
                offset += byteList[i].Length;
            }

            return bytes;
        }

        public BObject Decode(byte[] bytes, int start, ref int pos)
        {
            if (bytes[start] != (byte)'d')
                return null;

            int i = start + 1;
            for (; i < bytes.Length;)
            {
                char c = (char)bytes[i];

                //the key should always be a BString, which always begins with digit after been encoded
                switch (c)
                {
                    case 'e':
                        pos = i + 1;
                        return (BObject)this;
                    default:
                        BString key = (BString)BDecoder.DecodeBString(bytes, i, ref pos);

                        //could not parse key
                        if (key == null)
                            return null;

                        i = pos;

                        BObject value = BDecoder.DecodeOneObject(bytes, i, ref pos);
                        if (value == null)
                            return null;

                        i = pos;
                        this.Add(key, value);
                        break;
                }
            }
            return (BObject)this;
        }

        public string ToString(int indent = 0)
        {
            StringBuilder sbindent = new StringBuilder();

            for (int i = 0; i < indent; ++i)
            {
                sbindent.Append("\t");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Dictionary\n").Append(sbindent).Append("{\n");


            foreach (KeyValuePair<BString, BObject> kvpair in this)
            {

                sb.Append(sbindent).Append("\t").Append(kvpair.Key.ToString(indent + 1)).Append(" => ").Append(kvpair.Value.ToString(indent + 1)).Append("\n");
            }

            sb.Append(sbindent).Append("}");

            return sb.ToString();
        }
    }
}

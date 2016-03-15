using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSTorrent
{
    public class BList :  List<BObject>, BObject
    {
        public static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        public BType GetBType()
        {
            return BType.BLIST;
        }
        public object GetValue()
        {
            return (object)this;
        }

        public byte[] Encode()
        {
            //l<contents>e
            List<byte[]> byteList = new List<byte[]>();

            int totalLength = 0;
            for (int i = 0; i < this.Count; ++i)
            {
                BObject o = this[i];
                
                byte[] tmp = o.Encode();
                
                totalLength += tmp.Length;

                byteList.Add(tmp);
            }

            byte[] bytes = new byte[totalLength + 2];
            bytes[0] = (byte)'l';
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
            if (bytes[start] != (byte)'l')
                return null;

            int i = start + 1;
            for (; i < bytes.Length;)
            {
                char c = (char)bytes[i];

                switch (c)
                {
                    case 'e':
                        pos = i + 1;
                        return (BObject)this;
                    default:
                        BObject obj = BDecoder.DecodeOneObject(bytes, i, ref pos);
                        if (obj == null)
                            return null;

                        i = pos;
                        this.Add(obj);
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
            sb.Append("List\n").Append(sbindent).Append("{\n");

            for (int i = 0; i < this.Count; ++i)
            {
                sb.Append(sbindent).Append("\t").Append(this[i].ToString(indent + 1)).Append("\n");
            }

            sb.Append(sbindent).Append("}");
            
            return sb.ToString();
        }
    }
}

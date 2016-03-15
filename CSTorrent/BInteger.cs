using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSTorrent
{
    public class BInteger : BObject
    {
        public static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        private int mValue;

        public BInteger() { }
        
        public BInteger(int x)
        {
            mValue = x;
        }
        
        public BType GetBType()
        {
            return BType.BINTEGER;
        }
        
        public object GetValue()
        {
            return (object)this.mValue;
        }

        public int GetInt()
        {
            return this.mValue;
        }

        public byte[] Encode()
        {
            //i<number in base 10 notation>e
            string s = mValue.ToString();
            byte[] bytes = new byte[s.Length + 2];
            bytes[0] = (byte)'i';
            bytes[s.Length + 1] = (byte)'e';

            byte[] tmp = encoding.GetBytes(s);

            Buffer.BlockCopy(tmp, 0, bytes, 1, tmp.Length);

            return bytes;
        }

        public BObject Decode(byte[] bytes, int start, ref int pos)
        {
            if (bytes[start] != (byte)'i')//invalid format
            {
                return null;
            }

            int i = start + 1;
            for (; i < bytes.Length; ++i)
            {
                char c = (char)bytes[i];

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
                        continue;
                    case '-':
                        if (i == start + 1)
                            continue;
                        else
                            return null;
                    case 'e':
                        //found both start and end
                        byte[] tmp = new byte[i - start - 1];
                        int value = 0;
                        Buffer.BlockCopy(bytes, start + 1, tmp, 0, i - start - 1);
                        if(!Int32.TryParse(BInteger.encoding.GetString(tmp), out value))
                            return null;

                        pos = i + 1;
                        return new BInteger(value);
                    default:
                        return null;
                }
            }

            return null;
        }

        public string ToString(int indent = 0)
        {
            StringBuilder sbindent = new StringBuilder();

            for (int i = 0; i < indent; ++i)
            {
                sbindent.Append("\t");
            }

            return sbindent.Append("Integer: " + mValue.ToString()).ToString();
        }
    }
}

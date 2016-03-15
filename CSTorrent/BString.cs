using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSTorrent
{
    public class BString : BObject, IComparable
    {
        public static System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
        private byte[] mValue;

        public BString()
        {
        }

        public BString(string s)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            mValue = encoding.GetBytes(s);
        }

        public BString(byte[] s)
        {
            mValue = s;
        }

        public BType GetBType()
        {
            return BType.BSTRING;
        }

        public object GetValue()
        {
            return (object)mValue;
        }

        public byte[] GetBytes()
        {
            return this.mValue;
        }

        public string GetString()
        {
            return BString.encoding.GetString(mValue);
        }

        public int CompareTo(string s)
        {
            return BString.encoding.GetString(mValue).CompareTo(s);
        }

        public int CompareTo(object s)
        {
            byte[] str = (byte[])((BString)s).GetValue();

            int i = 0;
            for (; i < mValue.Length && i < str.Length; )
            {
                if(mValue[i] == str[i])
                {
                    ++ i;
                    continue;
                }
                else
                {
                    if(mValue[i] < str[i])
                        return -1;
                    else
                        return 1;
                }
            }

            if (mValue.Length == str.Length)
                return 0;
            else if (mValue.Length > str.Length)
                return 1;
            else
                return -1;
        }

        public byte[] Encode()
        {
            //<length>:<contents>
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] length = encoding.GetBytes(mValue.Length.ToString());

            byte[] bytes = new byte[length.Length + 1 + mValue.Length];
            
            Buffer.BlockCopy(length, 0, bytes, 0, length.Length);
            bytes[length.Length] = (byte)':';
            Buffer.BlockCopy(mValue, 0, bytes, length.Length + 1, mValue.Length);

            return bytes;
        }

        public BObject Decode(byte[] bytes, int start, ref int pos)
        {
            int i = start;
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
                    case ':':
                        //found seperator
                        byte[] tmp = new byte[i - start];
                        Buffer.BlockCopy(bytes, start, tmp, 0, i - start);
                        int length = 0;
                        if (!Int32.TryParse(BInteger.encoding.GetString(tmp), out length))
                            return null;
                        
                        //now we have the length.
                        //check data format
                        if (start + length.ToString().Length + 1 + length > bytes.Length)
                            return null;
                        //copy data to mValue
                        byte[] value = new byte[length];

                        Buffer.BlockCopy(bytes, start + length.ToString().Length + 1, value, 0, length);

                        pos = i + length + 1;
                        return new BString(value);
                    default:
                        return null;
                }
            }

            return null;
        }

        public string ToString(int indent = 0)
        {
            if (mValue.Length < 100)
                return "String[" + mValue.Length + "]: " + BString.encoding.GetString(mValue);
            else
                return "String[" + mValue.Length + "]: [...]";
        }
    }
}

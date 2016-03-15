using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace CSTorrent
{
    public class Utils
    {
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static string EscapeByteForURL(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; ++i)
            {
                //if a byte is 0-9, a-z, A-Z, '.', '-', '_' and '~', convert it to a char
                //else encoded it using the "%nn" format, where nn is the hexadecimal value of the byte
                byte b = bytes[i];

                if ((b >= '0' && b <= '9') || (b >= 'a' && b <= 'z') || (b >= 'A' && b <= 'Z')
                    || b == (byte)'.' || b == (byte)'-' || b == (byte)'_' || b == (byte)'~')
                    sb.Append(Convert.ToChar(b));
                else
                {
                    sb.Append("%");
                    string tmp = Convert.ToString(b, 16).ToUpper();
                    if (tmp.Length == 1)
                        tmp = "0" + tmp;
                    sb.Append(tmp);
                }
            }

            System.Diagnostics.Debug.WriteLine(sb.ToString());
            return sb.ToString();
        }
    }
}

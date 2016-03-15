using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSTorrent
{
    public class BEncoder
    {
        public static byte[] Encode(BObject o)
        {
            return o.Encode();
        }
    }
}

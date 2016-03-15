using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CSTorrent
{
    public enum BType {BINTEGER, BSTRING, BLIST, BDICTIONARY};

    public interface BObject
    {
        BType GetBType();
        object GetValue();
        byte[] Encode();
        BObject Decode(byte[] bytes, int start, ref int pos);
        string ToString(int indent = 0);
    }
}

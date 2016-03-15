using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSTorrent
{
    public class TorrentParser
    {
        public static MetaInfo ParseTorrent(byte[] content)
        {
            BObject obj = BDecoder.Decode(content);

            if (obj == null)
                return null;

            MetaInfo metaInfo = new MetaInfo();

            if (!metaInfo.Parse(obj))
                return null;

            return metaInfo;
        }

    }
}

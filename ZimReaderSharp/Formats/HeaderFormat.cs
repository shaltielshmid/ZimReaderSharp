using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZimReaderSharp.Formats {
    public class HeaderFormat : IFormat {
        public uint MagicNumber { get; private set; }
        public uint Version { get; private set; }
        public byte[]? UUIDs { get; private set; }
        public uint ArticleCount { get; private set; }
        public uint ClusterCount { get; private set; }
        public ulong UrlPtrPos { get; private set; }
        public ulong TitlePtrPos { get; private set; }
        public ulong ClusterPtrPos { get; private set; }
        public ulong MimeListPos { get; private set; }
        public uint MainPage { get; private set; }
        public uint LayoutPage { get; private set; }
        public ulong ChecksumPos { get; private set; }

        public void UnpackFromBuffer(BinaryReader br) {
            MagicNumber = br.ReadUInt32();
            Version = br.ReadUInt32();
            UUIDs = br.ReadBytes(16);
            ArticleCount = br.ReadUInt32();
            ClusterCount = br.ReadUInt32();
            UrlPtrPos = br.ReadUInt64();
            TitlePtrPos = br.ReadUInt64();
            ClusterPtrPos = br.ReadUInt64();
            MimeListPos = br.ReadUInt64();
            MainPage = br.ReadUInt32();
            LayoutPage = br.ReadUInt32();
            ChecksumPos = br.ReadUInt64();
        }
    }
}

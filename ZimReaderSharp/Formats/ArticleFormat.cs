using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZimReaderSharp.Formats {
    public abstract class ArticleFormat : IFormat {
        public ushort MimeType { get; protected set; }
        public byte ParameterLen { get; protected set; }
        public byte Namespace { get; protected set; }
        public uint Revision { get; protected set; }
        public string? Url { get; protected set; }
        public string? Title { get; protected set; }
        public string? Parameter { get; protected set; }

        public abstract void UnpackFromBuffer(BinaryReader br);
    }
}

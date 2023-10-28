using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ZimReaderSharp.Formats {
    public class RedirectEntryFormat : ArticleFormat {
        public uint RedirectIndex { get; private set; }

        public override void UnpackFromBuffer(BinaryReader br) {
            MimeType = br.ReadUInt16();
            ParameterLen = br.ReadByte();
            Namespace = br.ReadByte();
            Revision = br.ReadUInt32();
            RedirectIndex = br.ReadUInt32();
            Url = br.ReadNullTerminatedString();
            Title = br.ReadNullTerminatedString();
            Parameter = Encoding.UTF8.GetString(br.ReadBytes(ParameterLen));
        }
    }
}
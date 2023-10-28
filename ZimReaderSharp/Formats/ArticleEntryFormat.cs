using System.Text;

namespace ZimReaderSharp.Formats {
    public class ArticleEntryFormat : ArticleFormat {
        public uint ClusterNumber { get; private set; }
        public uint BlobNumber { get; private set; }

        public override void UnpackFromBuffer(BinaryReader br) {
            MimeType = br.ReadUInt16();
            ParameterLen = br.ReadByte();
            Namespace = br.ReadByte();
            Revision = br.ReadUInt32();
            ClusterNumber = br.ReadUInt32();
            BlobNumber = br.ReadUInt32();
            Url = br.ReadNullTerminatedString();
            Title = br.ReadNullTerminatedString();
            Parameter = Encoding.UTF8.GetString(br.ReadBytes(ParameterLen));
        }
    }
}
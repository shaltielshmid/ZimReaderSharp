using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZimReaderSharp.Formats {
    public class ClusterFormat : IFormat {
        public CompressionType CompressionType { get; private set; }

        public void UnpackFromBuffer(BinaryReader br) {
            CompressionType = (CompressionType)br.ReadByte();
        }
    }
}

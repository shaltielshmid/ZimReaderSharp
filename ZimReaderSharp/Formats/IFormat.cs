using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZimReaderSharp.Formats {
    interface IFormat {
        public void UnpackFromBuffer(BinaryReader br);
    }
}

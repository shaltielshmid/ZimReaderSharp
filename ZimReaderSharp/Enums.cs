using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZimReaderSharp {
    public enum CompressionType {
        None = 1,
        Zip = 2,
        Bzip2 = 3,
        Lzma = 4,
        Zstd = 5
    }
}

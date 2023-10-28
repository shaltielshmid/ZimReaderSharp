using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZimReaderSharp.Formats {
    public class MimeTypeListFormat : IFormat {
        public List<string>? MimeTypes { get; set; }

        public void UnpackFromBuffer(BinaryReader br) {
            MimeTypes = new List<string>();
            string s;
            while ((s = br.ReadNullTerminatedString()) != "")
                MimeTypes.Add(s);
        }
    }
}
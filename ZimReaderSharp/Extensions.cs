using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZimReaderSharp {
    public static class BinaryReaderExtensions {
        /// <summary>
        /// Extension method for using `BinaryReader` to read a null-terminated string as opposed to the default built-in behavior.
        /// </summary>
        public static string ReadNullTerminatedString(this BinaryReader br) {
            var bytes = new List<byte>();
            byte b;
            while ((b = br.ReadByte()) != 0)
                bytes.Add(b);

            return Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}
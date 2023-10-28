using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZimReaderSharp.Formats;

namespace ZimReaderSharp {
    internal static class Utils {
        public static int BinarySearch(Func<int, string> GetKeyFunc, string key, int min, int max) {
            while (true) {
                if (max < min)
                    return -1;

                int middle = (min + max) / 2;

                int comp = GetKeyFunc(middle).CompareTo(key);
                if (comp < 0)
                    min = middle + 1;
                else if (comp > 0)
                    max = middle - 1;
                else
                    return middle;
            }
        }

        public static T UnpackFormat<T>(BinaryReader br, long offset) where T : IFormat, new() {
            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            var ret = new T();
            ret.UnpackFromBuffer(br);
            return ret;
        }
    }
}

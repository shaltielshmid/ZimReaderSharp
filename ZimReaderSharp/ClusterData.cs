using System.Text;
using ZimReaderSharp.Formats;
using SharpCompress.Compressors.LZMA;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Compressors;

namespace ZimReaderSharp {
    internal class ClusterData {
        private readonly ClusterFormat _clusterInfo;
        private readonly CompressionType _compression;
        private readonly BinaryReader _br;
        private readonly long _offset;
        private readonly List<long> _offsets;
        private MemoryStream? _uncompressedBuffer;
        public ClusterData(BinaryReader br, long offset, int clusterSize) {
            _clusterInfo = Utils.UnpackFormat<ClusterFormat>(br, offset);
            _compression = _clusterInfo.CompressionType;

            _br = br;
            _offset = offset;
            _offsets = new();

            Decompress(clusterSize);
            ReadOffsets();
        }

        public byte[] ReadBlob(int index) {
            if (index >= _offsets.Count - 1) throw new IndexOutOfRangeException();

            long blobSize = _offsets[index + 1] - _offsets[index];

            // Get our BinaryReader (can be either the general one or a new custom one on the
            // decompressed chunk). Seek forwards to our offset. 
            var br = GetPositionedBinaryReader();
            br.BaseStream.Seek(br.BaseStream.Position + _offsets[index], SeekOrigin.Begin);

            return br.ReadBytes((int)blobSize);
        }

        public BinaryReader GetPositionedBinaryReader() {
            if (_compression != CompressionType.None) {
                _uncompressedBuffer!.Seek(0, SeekOrigin.Begin);
                return new BinaryReader(_uncompressedBuffer);
            }

            _br.BaseStream.Seek(_offset + 1, SeekOrigin.Begin);
            return _br;
        }

        private void Decompress(int clusterSize) {
            if (_compression == CompressionType.None) return;

            _uncompressedBuffer = new MemoryStream();

            var inputStream = new MemoryStream();
            inputStream.Write(_br.ReadBytes(clusterSize));
            inputStream.Seek(0, SeekOrigin.Begin);

            if (_compression == CompressionType.Zip) {
                // Not supported anymore
                throw new NotSupportedException();
            }
            else if (_compression == CompressionType.Bzip2) {
                var bzipStream = new BZip2Stream(inputStream, CompressionMode.Decompress, false);
                bzipStream.CopyTo(_uncompressedBuffer);
            }
            else if (_compression == CompressionType.Lzma) {
                var lzmaStream = new LzmaStream(new LzmaEncoderProperties(), false, inputStream);
                lzmaStream.CopyTo(_uncompressedBuffer);
            }
            else if (_compression == CompressionType.Zstd) {
                var zstdStream = new ZstdNet.DecompressionStream(inputStream);
                zstdStream.CopyTo(_uncompressedBuffer);
            }
            else throw new NotImplementedException();
        }

        private void ReadOffsets() {
            var br = GetPositionedBinaryReader();

            uint offset0 = br.ReadUInt32();
            _offsets.Add(offset0);

            uint maxBlobs = offset0 / 4;
            for (int i = 0; i < maxBlobs - 1; i++)
                _offsets.Add(br.ReadUInt32());
        }
    }
}

using DotNext.Runtime.Caching;
using ZimReaderSharp.Formats;

namespace ZimReaderSharp {
    public class ZimFile : IDisposable {
        private readonly BinaryReader _br;
        private readonly HeaderFormat _header;
        private readonly MimeTypeListFormat _mimeTypeList;
        private readonly ConcurrentCache<long, ClusterData> _clusterCache;

        /// <summary>
        /// A robust class for loading in a ZIM file with uncompressed and comrpessed entries and reading them easily
        /// </summary>
        /// <param name="filename">Path to the ZIM file</param>
        /// <param name="cacheCount">Entries within the ZIM file are stored in compressed chunks, this number reflects how many uncompressed chunks we will store in the cache at once to optimize performance.</param>
        public ZimFile(string filename, int cacheCount = 50) : this(File.OpenRead(filename), cacheCount) {
        }

        /// <summary>
        /// A robust class for loading in a ZIM file with uncompressed and comrpessed entries and reading them easily
        /// </summary>
        /// <param name="stream">A readable stream with the contents of a ZIM file</param>
        /// <param name="cacheCount">Entries within the ZIM file are stored in compressed chunks, this number reflects how many uncompressed chunks we will store in the cache at once to optimize performance.</param>
        public ZimFile(Stream stream, int cacheCount = 50) {
            _br = new BinaryReader(stream);
            _header = Utils.UnpackFormat<HeaderFormat>(_br, 0);
            _mimeTypeList = Utils.UnpackFormat<MimeTypeListFormat>(_br, (long)_header.MimeListPos);
            _clusterCache = new ConcurrentCache<long, ClusterData>(cacheCount, CacheEvictionPolicy.LRU);
        }

        /// <summary>
        /// Total number of articles in the current file
        /// </summary>
        public int TotalArticles => (int)_header.ArticleCount;

        /// <summary>
        /// Lookup an article in the file by its index, with an option to follow redirect entries through.
        /// </summary>
        /// <param name="index">Index of the article to lookup</param>
        /// <param name="fFollowRedirect">Flag whether to follow redirect entries through.</param>
        /// <returns>An article item containing all the data for that entry.</returns>
        public ArticleItem GetArticleByIndex(int index, bool fFollowRedirect) {
            var entry = ReadDirectoryEntryByIndex(index);

            // If we're at a redirect entry - either follow it through, or just return it 
            // without the content (aka data & mime).
            if (entry is RedirectEntryFormat redirectEntry) {
                return fFollowRedirect ? GetArticleByIndex((int)redirectEntry.RedirectIndex, true)
                                       : new(entry, index, true, null, null);
            }

            var articleEntry = (entry as ArticleEntryFormat)!;
            byte[] data = ReadBlob((int)articleEntry.ClusterNumber, (int)articleEntry.BlobNumber);
            string mime = _mimeTypeList.MimeTypes![articleEntry.MimeType];

            return new(articleEntry, index, false, data, mime);
        }

        /// <summary>
        /// Retrieve an article entry using the Url. The entry is found using binary search.
        /// </summary>
        /// <param name="nspace">The namespace of the Url</param>
        /// <param name="url">The Url to search for</param>
        /// <param name="fFollowRedirect">If the given Url results in a redirect entry, whether to follow through.</param>
        /// <returns>An article item containing all the data for that entry, and null if not found.</returns>
        public ArticleItem? GetEntryByUrl(byte nspace, string url, bool fFollowRedirect = true) {
            string nsurl = $"{(char)nspace}/{url}";

            // Using binary search, try to find the index for our URL
            // If there isn't a match, it will return -1.
            int index = Utils.BinarySearch(i => {
                var entry = ReadDirectoryEntryByIndex(i);
                return $"{(char)entry.Namespace}/{url}";
            }, nsurl, 0, (int)_header.ArticleCount);

            if (index == -1)
                return null;

            return GetArticleByIndex(index, fFollowRedirect);
        }

        /// <summary>
        /// Create an iterator over all the article indicies and their corresponding mime type. Skips over redirect entries.
        /// </summary>
        /// <returns>Enumerable of a tuple of every non-redirect article index and its mime type</returns>
        public IEnumerable<(int index, string mime)> IterateArticles() {
            for (int i = 0; i < _header.ArticleCount; i++) {
                var entry = ReadDirectoryEntryByIndex(i);

                // Ignore redirects, we'll get to the redirected article eventually
                if (entry is RedirectEntryFormat)
                    continue;

                yield return (i, _mimeTypeList.MimeTypes![entry.MimeType]);
            }
        }

        public void Dispose() {
            _br.Dispose();
            GC.SuppressFinalize(this);
        }

        private ulong ReadURLPointer(int index) {
            _br.BaseStream.Seek((long)_header.UrlPtrPos + 8 * index, SeekOrigin.Begin);
            return _br.ReadUInt64();
        }

        private uint ReadTitlePointer(int index) {
            _br.BaseStream.Seek((long)_header.TitlePtrPos + 4 * index, SeekOrigin.Begin);
            return _br.ReadUInt32();
        }

        private ulong ReadClusterPointer(int index) {
            if (index == _header.ClusterCount)
                return _header.ChecksumPos;

            _br.BaseStream.Seek((long)_header.ClusterPtrPos + 8 * index, SeekOrigin.Begin);
            return _br.ReadUInt64();
        }

        private ArticleFormat ReadDirectoryEntryByIndex(int index) {
            ulong ptr = ReadURLPointer(index);
            var ret = ReadDirectoryEntry((long)ptr);
            return ret;
        }

        private byte[] ReadBlob(int clusterIndex, int blobIndex) {
            long ptr = (long)ReadClusterPointer(clusterIndex);
            if (_clusterCache.TryGetValue(ptr, out var cluster))
                return cluster.ReadBlob(blobIndex);

            var clusterData = new ClusterData(_br, ptr, (int)((long)ReadClusterPointer(clusterIndex + 1) - ptr));
            _clusterCache.TryAdd(ptr, clusterData);
            return clusterData.ReadBlob(blobIndex);
        }

        private ArticleFormat ReadDirectoryEntry(long offset) {
            _br.BaseStream.Seek(offset, SeekOrigin.Begin);

            ushort fields = _br.ReadUInt16();
            return fields == 0xffff ? Utils.UnpackFormat<RedirectEntryFormat>(_br, offset)
                                    : Utils.UnpackFormat<ArticleEntryFormat>(_br, offset);
        }
    }
}
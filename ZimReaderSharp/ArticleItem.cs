using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZimReaderSharp.Formats;

namespace ZimReaderSharp {
    public record class ArticleItem(ArticleFormat Format, int Index, bool IsRedirect, byte[]? Data, string? Mime);
}
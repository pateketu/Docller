using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Docller.UI.Models
{
    public class MarkupFileUploadModel
    {
        public long FileId { get; set; }
        public long ProjectId { get; set; }
        public long FolderId { get; set; }
        public string Name { get; set; }
        public decimal FileSize { get; set; }
        public Stream FileStream { get; set; }
        public string Comments { get; set; }
        public int? Chunk { get; set; }
        public int? Chunks { get; set; }
    }
}
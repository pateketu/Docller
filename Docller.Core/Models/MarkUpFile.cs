using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Models
{

    public class FileAttachment2:BlobBase
    {
        public bool IsExistingFile { get; set; }
        public bool CreatedByCurrentUser { get; set; }
        public Guid ParentFile { get; set; }
        public long FileAttachmentId { get; set; }
        public long ParentFileAttachmentId { get; set; }
        public string Comments { get; set; }
        public bool IsFromApp { get; set; }
        public string AppDetails { get; set; }
        public long FileRevisionNumber { get; set; }
    }
    public class MarkUpFile : FileAttachment2
    {
        
    }
}

using System;
using System.Collections.Generic;

namespace Docller.Core.Models
{
    public class FileAttachment : BlobBase
    {
        public Guid ParentFile { get; set; }
        public string BaseFileName { get; set; }
        public int RevisionNumber { get; set; }
        public bool IsExistingFile { get; set; }
        public List<FileAttachmentVersion> Versions { get; set; }
        
    }
}

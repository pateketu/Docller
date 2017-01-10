using System;
using System.Collections.Generic;

namespace Docller.Core.Models
{
    public class FileVersion:FileBase
    {
        public int RevisionNumber { get; set; }
        public string VersionPath { get; set; }
        public List<FileAttachmentVersion> Attachments {get;set;}
        public long TransmittalId { get; set; }
    }
}
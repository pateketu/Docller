using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Models
{
    public class TransmittedFile : FileBase
    {
        public int RevisionNumber { get; set; }
        public string IssuedFileName { get; set; }
        public string IssuedTitle { get; set; }

        public List<FileAttachment> Attachments { get; set; }
    }

    public class TransmittedFileVersion:TransmittedFile
    {
        public string VersionPath { get; set; }
        public new List<FileAttachmentVersion> Attachments { get; set; }

    }
}

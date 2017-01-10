using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Models
{
    public class File:FileBase
    {
        public List<FileVersion> Versions { get; set; }
        public List<FileAttachment> Attachments { get; set; }
        public int CurrentRevision { get; set; }
        public int VersionCount { get; set; }
        public string BaseFileName { get; set; }
        public bool IsExistingFile { get; set; }
        public bool HasVersions { get; set; }
        public bool HasTransmittals { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PreviewUrl { get; set; }
        public long PreviewsTimeStamp { get; set; }
    }
}

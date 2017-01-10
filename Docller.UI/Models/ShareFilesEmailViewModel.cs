using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Docller.Core.Models;

namespace Docller.UI.Models
{
    public class ShareFilesEmailViewModel
    {
        public string To { get; set; }
        public string Message { get; set; }
        public IEnumerable<File> Files { get; set; } 
        public string DownloadUrl { get; set; }
        public string SharedBy { get; set; }
        public string SharedByEmail { get; set; }
        public long TransmittalId { get; set; }
        public long ProjectId { get; set; }
    }
}
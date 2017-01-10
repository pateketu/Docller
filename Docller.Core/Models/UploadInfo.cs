using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Models
{
    [Serializable]
    public class UploadInfo
    {
        public IEnumerable<File> Files { get; set; }
        public string[] ExisitingFiles { get; set; }
        public string[] ExistingAttachments { get; set; }
    }
}

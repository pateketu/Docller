using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Docller.Core.Models;
using Newtonsoft.Json;

namespace Docller.Models
{
    public class FileMetaData
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public string Id { get; set; }
    }
    public class FilesMetaInfo
    {
        public FileMetaData[] Files { get; set; }
        public long ProjectId { get; set; }
        public long FolderId { get; set; }
        public bool AttachCADFiles { get; set; }
        public bool UploadAsNewVersion { get; set; }
    }
    public class FileEditData
    {
        public string FileJson { get; set; }
        public string StatusJson { get; set; }
    }
}
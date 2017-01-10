using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Models;

namespace Docller.Core.Services
{
    public class FolderCreationStatus
    {
        public StorageServiceStatus Status { get; set; }
        public List<Folder> DuplicateFolders { get; set; }

    }
}

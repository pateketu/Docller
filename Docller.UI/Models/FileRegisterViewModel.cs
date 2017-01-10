using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Docller.Common;
using Docller.Core.Models;

namespace Docller.Models
{
    public class FileRegisterViewModel
    {
        public Files Files { get; set; }
        public Queue<Folder> FolderCrumbs { get; set; }
    }
}
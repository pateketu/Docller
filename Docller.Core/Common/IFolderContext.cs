using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Models;

namespace Docller.Core.Common
{
    public interface IFolderContext
    {
        Folders AllFolders { get; set; }
        long CurrentFolderId { get; set; }
        void Inject(Folders folders, long currentFolderId);
        
    }
}

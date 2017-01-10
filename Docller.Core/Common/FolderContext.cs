using System;
using System.Collections.Generic;
using Docller.Core.Models;

namespace Docller.Core.Common
{
    public class FolderContext : IFolderContext
    {
        #region Implementation of IFolderContext
        public Folders AllFolders { get; set; }
        public long CurrentFolderId { get; set; }
        public void Inject(Folders folders, long currentFolderId)
        {
            this.AllFolders = folders;
            this.CurrentFolderId = currentFolderId;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Docller.Core.Models
{
    public class Folders:List<Folder>
    {
        private readonly Dictionary<long, Folder> _allFolders;

        public Folders(Dictionary<long,Folder> allFolders, IEnumerable<Folder> rootLevelFolders):base(rootLevelFolders)
        {
            _allFolders = allFolders;
        }

        public Folder this[long folderId] 
        {
            get { return this._allFolders.ContainsKey(folderId) ? this._allFolders[folderId] : null; }
        }

        public bool IsAncestor(long folderId, long potentialParentFolder)
        {
            if (folderId > 0 && potentialParentFolder > 0)
            {
                Folder folder = this._allFolders[folderId];
                return folder != null && (folder.AllParents.Contains(potentialParentFolder));
            }
            return false;
        }
    }
}

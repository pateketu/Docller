using System.Collections.Generic;
using System.Data;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers.StoredProcMappers
{
    public class FoldersMapper:IResultSetMapper<Folder> 
    {
        private readonly IRowMapper<Folder> _folderMapper;
        private readonly Dictionary<long, Folder> _rootLevelFolders;
        private readonly Dictionary<long, Folder> _allFolders;
        public FoldersMapper()
        {
            _folderMapper = MapBuilder<Folder>.MapNoProperties()
                .MapByName(x => x.FolderId)
                .MapByName(x => x.ParentFolderId)
                .MapByName(x => x.FolderName)
                .MapByName(x => x.FullPath)
                .MapByName(x=>x.ProjectId)
                .Map(x=>x.CurrentUserPermissions).WithFunc(r=>r.IsDBNull(7) ? PermissionFlag.None : (PermissionFlag) r.GetInt32(7))
                .Build();

            _rootLevelFolders = new Dictionary<long, Folder>();
            _allFolders = new Dictionary<long, Folder>();
        }

      
        #region Implementation of IResultSetMapper<Folder>

        public IEnumerable<Folder> MapSet(IDataReader reader)
        {
            using(reader)
            {
                while(reader.Read())
                {
                    Folder folder = this._folderMapper.MapRow(reader);
                    
                    folder.SubFolders = new List<Folder>();
                    folder.AllParents = new List<long>();
                    this._allFolders.Add(folder.FolderId, folder);

                    if(folder.ParentFolderId == 0)
                    {
                        this._rootLevelFolders.Add(folder.FolderId,folder);
                    }
                    else
                    {
                        Folder parentFolder = this._rootLevelFolders.ContainsKey(folder.ParentFolderId)
                                                  ? this._rootLevelFolders[folder.ParentFolderId]
                                                  : this._allFolders[folder.ParentFolderId];
                        parentFolder.SubFolders.Add(folder);
                        folder.AllParents.Add(folder.ParentFolderId);
                        folder.AllParents.AddRange(parentFolder.AllParents);
                    }
                }
            }
            Folders folders = new Folders(this._allFolders,this._rootLevelFolders.Values.ToList());
            return folders;
        }

        #endregion
    }
}

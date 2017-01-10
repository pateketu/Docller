using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Docller.Core.Models;
using Microsoft.SqlServer.Server;

namespace Docller.Core.Repository.Collections
{
    public class FolderCollection : List<Folder>, IEnumerable<SqlDataRecord>
    {
        private readonly StringDictionary _dups;
        public FolderCollection(IEnumerable<Folder> folders)
            : base(folders)
        {
            _dups = new StringDictionary();
        }

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord dataRecord = new SqlDataRecord(new SqlMetaData("FolderId", SqlDbType.BigInt),
                                                         new SqlMetaData("FolderName", SqlDbType.NVarChar, 255),
                                                         new SqlMetaData("ParentFolderId", SqlDbType.NVarChar, 1000),
                                                         new SqlMetaData("FullPath", SqlDbType.NVarChar, 2000));

            for (int i = 0; i < this.Count; i++)
            {
                Folder folder = this[i];
                if (this.IsUnique(folder))
                {
                    dataRecord.SetInt64(0, folder.FolderId);
                    dataRecord.SetString(1, folder.FolderName);
                    dataRecord.SetNullableLong(2, folder.ParentFolderId);
                    dataRecord.SetNullableString(3, folder.FullPath);
                }
                yield return dataRecord;
            }

        }

        private bool IsUnique(Folder folder)
        {
            if(!this._dups.ContainsKey(folder.FolderName))
            {
                this._dups.Add(folder.FolderName, folder.FolderName);
                return true;
            }
            return false;
        }
    }
}

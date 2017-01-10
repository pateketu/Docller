using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Docller.Core.Models;
using Microsoft.SqlServer.Server;

namespace Docller.Core.Repository.Collections
{
    public abstract class BlobCollection<T> : List<T>, IEnumerable<SqlDataRecord> where T : BlobBase, new()
    {
        #region Implementation of IEnumerable<out SqlDataRecord>

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord dataRecord = this.GetDataRecord();

            //Set the values for the properties from base
            for (int i = 0; i < this.Count; i++)
            {
                T t = this[i];
                SetPrimaryColumns(dataRecord, t);
                SetAdditionalColumns(dataRecord, t);
                yield return dataRecord;
            }
        }

        #endregion

        protected BlobCollection()
        {
            
        }

        protected BlobCollection(IEnumerable<T> blob):base(blob)
        {
            
        }


        protected virtual SqlDataRecord GetDataRecord()
        {
            SqlDataRecord dataRecord = new SqlDataRecord(new SqlMetaData("FileId", SqlDbType.BigInt),
                                                       new SqlMetaData("FileInternalName", SqlDbType.UniqueIdentifier),
                                                       new SqlMetaData("FileName", SqlDbType.NVarChar, 255),
                                                       new SqlMetaData("FileSize", SqlDbType.Decimal),
                                                       new SqlMetaData("ProjectId", SqlDbType.BigInt),
                                                       new SqlMetaData("FolderId", SqlDbType.BigInt),
                                                       new SqlMetaData("DocNumber", SqlDbType.NVarChar, 255),
                                                       new SqlMetaData("Revision", SqlDbType.NVarChar, 10),
                                                       new SqlMetaData("Title", SqlDbType.NVarChar, 1000),
                                                       new SqlMetaData("Notes", SqlDbType.NVarChar, 3000),
                                                       new SqlMetaData("ParentFile", SqlDbType.UniqueIdentifier),
                                                       new SqlMetaData("BaseFileName", SqlDbType.NVarChar, 255),
                                                       new SqlMetaData("IsExistingFile", SqlDbType.Bit),
                                                       new SqlMetaData("RevisionNumber",SqlDbType.Int),
                                                       new SqlMetaData("StatusId", SqlDbType.BigInt));

            return dataRecord;
        }

        protected virtual void SetPrimaryColumns(SqlDataRecord dataRecord, T t)
        {
            dataRecord.SetNullableLong(0, t.FileId);
            dataRecord.SetNullableGuid(1, t.FileInternalName);
            dataRecord.SetNullableString(2, t.FileName);
            dataRecord.SetNullableDecimal(3, t.FileSize);
            dataRecord.SetNullableLong(4, (t.Project != null ? t.Project.ProjectId : 0));
            dataRecord.SetNullableLong(5, (t.Folder != null ? t.Folder.FolderId : 0));
            
        }
        
        protected virtual void SetAdditionalColumns(SqlDataRecord dataRecord, T t)
        {
            dataRecord.SetDBNull(6);
            dataRecord.SetDBNull(7);
            dataRecord.SetDBNull(8);
            dataRecord.SetDBNull(9);
            dataRecord.SetDBNull(10);
            dataRecord.SetDBNull(11);
            dataRecord.SetDBNull(12);
            dataRecord.SetDBNull(13);
            dataRecord.SetDBNull(14);
            
        }
    }
}

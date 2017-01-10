using System.Collections.Generic;
using Docller.Core.Models;
using Microsoft.SqlServer.Server;

namespace Docller.Core.Repository.Collections
{
    public class FileCollection : BlobCollection<File>
    {
        public FileCollection()
        {
            
        }

        public FileCollection(IEnumerable<File> files):base(files)
        {
            
        }
        protected override void SetAdditionalColumns(SqlDataRecord dataRecord, File t)
        {
            dataRecord.SetNullableString(6, t.DocNumber);
            dataRecord.SetNullableString(7, t.Revision);
            dataRecord.SetNullableString(8, t.Title);
            dataRecord.SetNullableString(9, t.Notes);
            dataRecord.SetDBNull(10);
            dataRecord.SetNullableString(11, t.BaseFileName);
            dataRecord.SetBoolean(12,t.IsExistingFile);
            dataRecord.SetDBNull(13);
            dataRecord.SetNullableLong(14,t.StatusId);
        }
        
    }
}
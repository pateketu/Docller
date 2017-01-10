using System.Collections.Generic;
using Docller.Core.Models;
using Microsoft.SqlServer.Server;

namespace Docller.Core.Repository.Collections
{
    public class FileAttachmentCollection : BlobCollection<FileAttachment>
    {
        public FileAttachmentCollection()
        {
            
        }

        public FileAttachmentCollection(IEnumerable<FileAttachment> attachments)
            : base(attachments)
        {
            
        }
        protected override void SetAdditionalColumns(SqlDataRecord dataRecord, FileAttachment fileAttachment)
        {
            dataRecord.SetDBNull(6);
            dataRecord.SetDBNull(7);
            dataRecord.SetDBNull(8);
            dataRecord.SetDBNull(9);
            dataRecord.SetNullableGuid(10,fileAttachment.ParentFile);
            dataRecord.SetNullableString(11, fileAttachment.BaseFileName);
            dataRecord.SetBoolean(12,fileAttachment.IsExistingFile);
            dataRecord.SetDBNull(13);
            dataRecord.SetDBNull(14);
        }
    }
}
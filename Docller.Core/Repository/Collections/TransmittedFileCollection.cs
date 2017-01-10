using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Models;

namespace Docller.Core.Repository.Collections
{
    public class TransmittedFileCollection : BlobCollection<TransmittedFile>
    {

        public TransmittedFileCollection(IEnumerable<TransmittedFile> files):base(files)
        {
            
        }
        protected override void SetAdditionalColumns(Microsoft.SqlServer.Server.SqlDataRecord dataRecord, TransmittedFile t)
        {
            dataRecord.SetDBNull(6);
            dataRecord.SetDBNull(7);
            dataRecord.SetDBNull(8);
            dataRecord.SetDBNull(9);
            dataRecord.SetDBNull(10);
            dataRecord.SetDBNull(11);
            dataRecord.SetDBNull(12);
            dataRecord.SetInt32(13,t.RevisionNumber);
            dataRecord.SetDBNull(14);
        }
    }
}

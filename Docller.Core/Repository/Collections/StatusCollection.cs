using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Docller.Core.Models;
using Microsoft.SqlServer.Server;

namespace Docller.Core.Repository.Collections
{
    public class StatusCollection : List<Status>, IEnumerable<SqlDataRecord>
    {
        public StatusCollection(IEnumerable<Status> statuses):base(statuses)
        {
            
        }
        #region Implementation of IEnumerable<out SqlDataRecord>

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord dataRecord = new SqlDataRecord(new SqlMetaData("Id",SqlDbType.BigInt),
                                                         new SqlMetaData("Id2",SqlDbType.BigInt),
                                                            new SqlMetaData("StringValue", SqlDbType.NVarChar, 500));
            for (int i = 0; i < this.Count; i++)
            {
                Status status = this[i];
                dataRecord.SetInt64(0,status.StatusId);
                dataRecord.SetInt64(1, status.ProjectId);
                dataRecord.SetNullableString(2, status.StatusText);
                yield return dataRecord;
            }

        }

        #endregion
    }
}

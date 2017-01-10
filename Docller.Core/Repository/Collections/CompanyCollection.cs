using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Docller.Core.Models;
using Microsoft.SqlServer.Server;

namespace Docller.Core.Repository.Collections
{
    public class CompanyCollection : List<Company>, IEnumerable<SqlDataRecord>
    {
        public CompanyCollection(IEnumerable<Company> companies)
            : base(companies)
        {
            
        }
        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord dataRecord = new SqlDataRecord(new SqlMetaData("CompanyId", SqlDbType.BigInt),
                new SqlMetaData("CompanyName", SqlDbType.NVarChar, 255),
                new SqlMetaData("PermissionFlag", SqlDbType.Int));

            for (int i = 0; i < this.Count; i++)
            {
                Company company = this[i];
                dataRecord.SetNullableLong(0, company.CompanyId);
                dataRecord.SetNullableString(1, company.CompanyName);
                dataRecord.SetNullableInt32(2, (int)company.Permission);
                yield return dataRecord;
            }
        }
    }
}

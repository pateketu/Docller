using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Docller.Core.Models;
using Microsoft.SqlServer.Server;

namespace Docller.Core.Repository.Collections
{
    public class UserCollection : List<User>, IEnumerable<SqlDataRecord>
    {
        public UserCollection(IEnumerable<User> users)
            : base(users)
        {
            
        }

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            SqlDataRecord dataRecord = new SqlDataRecord(new SqlMetaData("UserId", SqlDbType.Int),
                new SqlMetaData("UserName", SqlDbType.NVarChar, 400),
                new SqlMetaData("FirstName", SqlDbType.NVarChar, 255),
                new SqlMetaData("LastName", SqlDbType.NVarChar, 255),
                new SqlMetaData("Email", SqlDbType.NVarChar, 400),
                new SqlMetaData("Password", SqlDbType.NVarChar, 128),
                new SqlMetaData("PasswordSalt", SqlDbType.NVarChar, 128),
                new SqlMetaData("CompanyId", SqlDbType.BigInt),
                new SqlMetaData("CompanyName", SqlDbType.NVarChar, 255),
                new SqlMetaData("CCed", SqlDbType.Bit),
                new SqlMetaData("PermissionFlag", SqlDbType.Int));
            ;

            for (int i = 0; i < this.Count; i++)
            {
                User user = this[i];
                SetColumns(dataRecord,user);
                yield return dataRecord;
            }

        }

       protected virtual void SetColumns(SqlDataRecord dataRecord, User user)
       {
           dataRecord.SetNullableInt32(0, user.UserId);
           dataRecord.SetNullableString(1, user.UserName);
           dataRecord.SetNullableString(2, user.FirstName);
           dataRecord.SetNullableString(3, user.LastName);
           dataRecord.SetNullableString(4, user.Email);
           dataRecord.SetNullableString(5, user.Password);
           dataRecord.SetNullableString(6, user.PasswordSalt);
           dataRecord.SetNullableLong(7, user.Company != null ? user.Company.CompanyId : 0);
           dataRecord.SetNullableString(8, user.Company != null ? user.Company.CompanyName : null);
           //dataRecord.SetBoolean(9, user.Cced);
           dataRecord.SetDBNull(9);
           dataRecord.SetNullableInt32(10, (int) user.CustomerPermissions);
       }
    }
}

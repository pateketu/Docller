using System.Collections.Generic;
using Docller.Core.Models;
using Microsoft.SqlServer.Server;

namespace Docller.Core.Repository.Collections
{
    public class TransmittalUserCollection : UserCollection
    {
        public TransmittalUserCollection(IEnumerable<TransmittalUser> users) : base(users)
        {
        }

        protected override void SetColumns(SqlDataRecord dataRecord, User user)
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
            dataRecord.SetBoolean(9, ((TransmittalUser)user).IsCced);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers
{
    public class UserSubscriptionInfoResultSetMapper:IResultSetMapper<User>
    {
        private readonly IRowMapper<Project> _projectMapper; 
        public UserSubscriptionInfoResultSetMapper()
        {
            _projectMapper = MapBuilder<Project>.MapNoProperties()
                                                .MapByName(x => x.ProjectId)
                                                .MapByName(x => x.ProjectName)
                                                .MapByName(x => x.ProjectImage)
                                                .MapByName(x => x.ProjectCode)
                                                .MapByName(x => x.BlobContainer)
                                                .Map(x => x.CurrentUserPermissions)
                                                .WithFunc(
                                                    r =>
                                                    r.IsDBNull(6) ? PermissionFlag.None : (PermissionFlag) r.GetInt32(6))
                                                .Build();

        }

        public IEnumerable<User> MapSet(IDataReader reader)
        {
            User user = new User
                {
                    Company = new Company(),
                    Projects = new List<Project>()
                };

            using (reader)
            {
                if (reader.Read())
                {
                    user.CustomerPermissions = (PermissionFlag)reader.GetInt32(0);
                }
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        user.Projects.Add(_projectMapper.MapRow(reader));
                    }
                }


                if (reader.NextResult())
                {
                    reader.Read();
                    user.Company.CompanyId = reader.GetInt64(0);
                    user.Company.CompanyName = reader.GetString(1);
                    user.Company.CompanyAlias = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                }
            }
            return new List<User>() {user};
        }
    }
}

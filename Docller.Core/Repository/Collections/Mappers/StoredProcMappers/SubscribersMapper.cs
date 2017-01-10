using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Docller.Core.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers.StoredProcMappers
{
    public class SubscribersMapper:IResultSetMapper<Company> 
    {
        private readonly IRowMapper<Company> _companyMapper;
        private readonly IRowMapper<User> _userMapper;

        public SubscribersMapper()
        {
            _companyMapper =
                MapBuilder<Company>.MapNoProperties()
                                   .MapByName(x => x.CompanyName)
                                   .MapByName(x => x.CompanyAlias)
                                   .MapByName(x => x.CompanyId)
                                   .Build();

            _userMapper =
                MapBuilder<User>.MapNoProperties()
                                .MapByName(x => x.Email)
                                .MapByName(x => x.FirstName)
                                .MapByName(x => x.LastName)
                                .MapByName(x => x.UserId)
                                .Map(x => x.Company)
                                .WithFunc(record => new Company() {CompanyId = record.GetInt64(4)})
                                .Build();
        }
        public IEnumerable<Company> MapSet(IDataReader reader)
        {
            Dictionary<long,Company> companies = new Dictionary<long, Company>();
            using (reader)
            {
                while (reader.Read())
                {
                    Company company = this._companyMapper.MapRow(reader);
                    companies.Add(company.CompanyId,company);
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        User user = this._userMapper.MapRow(reader);
                        companies[user.Company.CompanyId].Users.Add(user);
                    }

                }

            }
            return companies.Values.ToList();
        }
    }
}

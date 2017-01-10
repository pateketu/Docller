using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers
{
    public class GenericResultSetMapper<T>:IResultSetMapper<T>
    {
        private readonly IRowMapper<T> _rowMapper;

        public GenericResultSetMapper(IRowMapper<T> rowMapper)
        {
            _rowMapper = rowMapper;
        }

        public IEnumerable<T> MapSet(IDataReader reader)
        {
            List<T> list = new List<T>();
            using(reader)
            {
                while (reader.Read())
                {
                    list.Add(this._rowMapper.MapRow(reader));
                }
            }
            return list;
        }
    }
}

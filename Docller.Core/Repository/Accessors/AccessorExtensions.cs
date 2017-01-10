using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Accessors
{
    public static class AccessorExtensions
    {
        /// <summary>
        /// Creates the sproc accessor.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="database">The database.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <param name="rowMapper">The row mapper.</param>
        /// <returns></returns>
        public static StoredProcAccessor<TResult> CreateStoredProcAccessor<TResult>(this Database database, string procedureName, IParameterMapper parameterMapper, IRowMapper<TResult> rowMapper) where TResult : new()
        {
            return new StoredProcAccessor<TResult>(database, procedureName, parameterMapper, rowMapper);
        }

        public static StoredProcAccessor<TResult> CreateStoredProcAccessor<TResult>(this Database database, string procedureName, IParameterMapper parameterMapper) where TResult : new()
        {
            return new StoredProcAccessor<TResult>(database, procedureName, parameterMapper);
        }

        public static StoredProcAccessor<TResult> CreateStoredProcAccessor<TResult>(this Database database, string procedureName, IParameterMapper parameterMapper, IResultSetMapper<TResult> resultSetMapper) where TResult : new()
        {
            return new StoredProcAccessor<TResult>(database, procedureName, parameterMapper,resultSetMapper);
        }
    }
}

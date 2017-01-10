using System;
using System.Collections.Generic;
using System.Linq;
using Docller.Core.Models;
using Docller.Core.Repository.Accessors;
using Docller.Core.Repository.Mappers;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository
{
    
    internal static class SqlDataRepositoryHelper
    {

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">The db.</param>
        /// <param name="sproc">The sproc.</param>
        /// <param name="item">The item.</param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery<T>(Database db, string sproc, T item, IReturnParameterMapper parameterMapper) where T : BaseModel, new()
        {
            return ExecuteNonQuery<T>(db, sproc, item, MapBuilder<T>.MapAllProperties().Build(), (IParameterMapper)parameterMapper);
        }

        /// <summary>
        /// Adds the specified db.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">The db.</param>
        /// <param name="sproc">The sproc.</param>
        /// <param name="item">The item.</param>
        /// <param name="rowMapper">The row mapper.</param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        /// <returns></returns>
        public static int ExecuteNonQuery<T>(Database db, string sproc, T item, IRowMapper<T> rowMapper, IParameterMapper parameterMapper) where T : BaseModel, new()
        {
            StoredProcAccessor<T> accessor = db.CreateStoredProcAccessor<T>(sproc, parameterMapper, rowMapper);
            int val =  accessor.ExecuteNonQuery(item);
            
            IReturnParameterMapper returnParameterMapper = parameterMapper as IReturnParameterMapper;
            if (returnParameterMapper != null && returnParameterMapper.ReturnValue.HasValue)
            {
                val = returnParameterMapper.ReturnValue.Value;
            }
            return val;

        }

        public static T Get<T>(Database db, IRowMapper<T> rowMapper, params object[] parameterValues) where T : BaseModel, new()
        {
            T item = new T();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            return db.ExecuteSprocAccessor(item.GetProc, parameterMapper, rowMapper, parameterValues).FirstOrDefault();

        }

    }
}

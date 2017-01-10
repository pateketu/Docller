using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Docller.Core.Repository.Mappers;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Accessors
{
    public class StoredProcAccessor<TResult>:SprocAccessor<TResult> where TResult : new()
    {
        readonly IParameterMapper _parameterMapper;
        readonly string _procedureName;
        private IResultSetMapper<TResult> _resultSetMapper;


        public StoredProcAccessor(Database database, string procedureName, IParameterMapper parameterMapper)
            : base(database, procedureName, parameterMapper, MapBuilder<TResult>.MapAllProperties().Build())
        {
            _parameterMapper = parameterMapper;
            _procedureName = procedureName;
            _resultSetMapper = new GenericResultSetMapper<TResult>(MapBuilder<TResult>.MapAllProperties().Build());
        }

        /// <summary>
        /// Creates a new instance of <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.SprocAccessor`1"/> that works for a specific <paramref name="database"/>
        /// and uses <paramref name="rowMapper"/> to convert the returned rows to clr type <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="database">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.Database"/> used to execute the Transact-SQL.</param>
        /// <param name="procedureName">The stored procedure that will be executed.</param>
        /// <param name="rowMapper">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.IRowMapper`1"/> that will be used to convert the returned data to clr type <typeparamref name="TResult"/>.</param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        public StoredProcAccessor(Database database, string procedureName, IRowMapper<TResult> rowMapper, IParameterMapper parameterMapper) : base(database, procedureName, rowMapper)
        {
            _parameterMapper = parameterMapper;
            _procedureName = procedureName;
            _resultSetMapper = new GenericResultSetMapper<TResult>(rowMapper);
        }

        /// <summary>
        /// Creates a new instance of <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.SprocAccessor`1"/> that works for a specific <paramref name="database"/>
        /// and uses <paramref name="resultSetMapper"/> to convert the returned set to an enumerable of clr type <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="database">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.Database"/> used to execute the Transact-SQL.</param>
        /// <param name="procedureName">The stored procedure that will be executed.</param>
        /// <param name="resultSetMapper">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.IResultSetMapper`1"/> that will be used to convert the returned set to an enumerable of clr type <typeparamref name="TResult"/>.</param>
        /// <param name="parameterMapper">The parameter mapper.</param>
        public StoredProcAccessor(Database database, string procedureName, IResultSetMapper<TResult> resultSetMapper, IParameterMapper parameterMapper) : base(database, procedureName, resultSetMapper)
        {
            _parameterMapper = parameterMapper;
            _procedureName = procedureName;
            _resultSetMapper = resultSetMapper;
        }

        /// <summary>
        /// Creates a new instance of <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.SprocAccessor`1"/> that works for a specific <paramref name="database"/>
        ///             and uses <paramref name="rowMapper"/> to convert the returned rows to clr type <typeparamref name="TResult"/>.
        ///             The <paramref name="parameterMapper"/> will be used to interpret the parameters passed to the Execute method.
        /// </summary>
        /// <param name="database">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.Database"/> used to execute the Transact-SQL.</param><param name="procedureName">The stored procedure that will be executed.</param><param name="parameterMapper">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.IParameterMapper"/> that will be used to interpret the parameters passed to the Execute method.</param><param name="rowMapper">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.IRowMapper`1"/> that will be used to convert the returned data to clr type <typeparamref name="TResult"/>.</param>
        public StoredProcAccessor(Database database, string procedureName, IParameterMapper parameterMapper, IRowMapper<TResult> rowMapper) : base(database, procedureName, parameterMapper, rowMapper)
        {
            _parameterMapper = parameterMapper;
            _procedureName = procedureName;
            _resultSetMapper = new GenericResultSetMapper<TResult>(rowMapper);
        }

        /// <summary>
        /// Creates a new instance of <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.SprocAccessor`1"/> that works for a specific <paramref name="database"/>
        ///             and uses <paramref name="resultSetMapper"/> to convert the returned set to an enumerable of clr type <typeparamref name="TResult"/>.
        ///             The <paramref name="parameterMapper"/> will be used to interpret the parameters passed to the Execute method.
        /// </summary>
        /// <param name="database">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.Database"/> used to execute the Transact-SQL.</param><param name="procedureName">The stored procedure that will be executed.</param><param name="parameterMapper">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.IParameterMapper"/> that will be used to interpret the parameters passed to the Execute method.</param><param name="resultSetMapper">The <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.IResultSetMapper`1"/> that will be used to convert the returned set to an enumerable of clr type <typeparamref name="TResult"/>.</param>
        public StoredProcAccessor(Database database, string procedureName, IParameterMapper parameterMapper, IResultSetMapper<TResult> resultSetMapper)
            : base(database, procedureName, parameterMapper, resultSetMapper)
        {
            _parameterMapper = parameterMapper;
            _procedureName = procedureName;
            _resultSetMapper = resultSetMapper;
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(params object[] parameterValues)
        {
            int val;
            using (DbCommand command = Database.GetStoredProcCommand(_procedureName))
            {
                _parameterMapper.AssignParameters(command, parameterValues);
                val = Database.ExecuteNonQuery(command);
            }
            if (this._parameterMapper is IOutputParameterMapper)
            {
                ((IOutputParameterMapper)this._parameterMapper).AssignOutpurParameters();
            }
            return val;
        }


        public IEnumerable<TResult> Execute(DbTransaction transaction,  params object[] parameterValues)
        {
            IEnumerable<TResult> results;
            using (DbCommand command = Database.GetStoredProcCommand(_procedureName))
            {
                _parameterMapper.AssignParameters(command, parameterValues);
                IDataReader reader = Database.ExecuteReader(command,transaction);
                results = this._resultSetMapper.MapSet(reader);
            }
            return results;
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        public override IEnumerable<TResult> Execute(params object[] parameterValues)
        {
            IEnumerable<TResult> results;
            using (DbCommand command = Database.GetStoredProcCommand(_procedureName))
            {
                _parameterMapper.AssignParameters(command, parameterValues);
                IDataReader reader = Database.ExecuteReader(command);
                results = this._resultSetMapper.MapSet(reader);
            }
            return results;
        }

        public TResult ExecuteSingle(params object[] paramterValues)
        {
            IEnumerable<TResult> results = this.Execute(paramterValues);
            return results != null ? results.FirstOrDefault() : default(TResult);
        }

        public TResult ExecuteSingle(DbTransaction dbTransaction, params object[] paramterValues)
        {
            IEnumerable<TResult> results = this.Execute(dbTransaction, paramterValues);
            return results != null ? results.FirstOrDefault() : default(TResult);
        }

    }
}

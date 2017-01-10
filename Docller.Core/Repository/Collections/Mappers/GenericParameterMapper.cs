using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.SqlServer.Server;

namespace Docller.Core.Repository.Mappers
{
    public class GenericParameterMapper : IParameterMapper, IReturnParameterMapper
    {
        private readonly Database _database;
        private DbParameter _returnParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParameterMapper"/> class.
        /// </summary>
        /// <param name="db">The db.</param>
        public GenericParameterMapper(Database db)
        {
            this._database = db;
        }

        /// <summary>
        /// Assigns Table Valued Parameter correctly
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterValues"></param>
        public void AssignParameters(DbCommand command, object[] parameterValues)
        {
            if (parameterValues.Length > 0)
            {
                _database.AssignParameters(command, parameterValues);
                DbParameterCollection assignedParams = command.Parameters;
                foreach (DbParameter assignedParam in assignedParams)
                {
                    if (assignedParam.Direction == ParameterDirection.ReturnValue)
                    {
                        this._returnParameter = assignedParam;
                        continue;

                    }
                    if (assignedParam.DbType != DbType.Object)
                        continue;

                    if (assignedParam is SqlParameter)
                    {
                        SqlParameter sqlParm = (SqlParameter)assignedParam;
                        // param.TypeName will be database.schema.typename
                        string typeName = sqlParm.TypeName;

                        // Trim off the database name to get schema.typename
                        typeName = typeName.Substring(typeName.IndexOf(".") + 1);

                        // If Microsoft fix this in a future release and only return
                        // schema.typename, we would end up with just the typename (no dot)
                        // So only change the TypeName if we still have a dot in our text
                        if (typeName.Contains("."))
                            sqlParm.TypeName = typeName;

                        if(assignedParam.Value is IEnumerable<SqlDataRecord>
                            && ((IEnumerable<SqlDataRecord>)assignedParam.Value).Count() ==0)
                        {

                            assignedParam.Value = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the return value.
        /// </summary>
        public int? ReturnValue
        {
            get
            {
                if (this._returnParameter != null)
                {
                    return Convert.ToInt32(this._returnParameter.Value);
                }
                else
                {
                    return null;
                }
            }
        }


    }
}

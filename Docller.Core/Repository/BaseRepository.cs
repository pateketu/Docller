using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Docller.Core.Common;
using Docller.Core.DB;
using Docller.Core.Resources;
using Microsoft.Practices.EnterpriseLibrary.Data;

using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling;
using StructureMap;

namespace Docller.Core.Repository
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public abstract class BaseRepository
    {
        private readonly string _connectionString;
        private readonly FederationType _federation;
        private readonly long _federationKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository"/> class.
        /// </summary>
        /// <param name="federation">The federation.</param>
        /// <param name="federationKey">The federation key.</param>
        /// <remarks></remarks>
        protected BaseRepository(FederationType federation, long federationKey)
        {
            this._connectionString =Config.GetConnectionString();
            this._federation = federation;
            this._federationKey = federationKey;
            ValidateFederation();
        }

        
        /// <summary>
        /// Gets the DB.
        /// </summary>
        /// <returns></returns>
        protected Database GetDb()
        {
            this.ValidatedConnectionStringName();
            Database db = _federation == FederationType.None
                              ? ObjectFactory.GetInstance<Database>()
                              : ObjectFactory.With("connectionString").EqualTo(this._connectionString).With(
                                  RetryPolicyFactory.GetDefaultSqlConnectionRetryPolicy()).With(
                                      this._federation).With("federationName").EqualTo(Config.GetValue<string>(ConfigKeys.FederatioName)).With(
                                          "distributionName").EqualTo(Config.GetValue<string>(ConfigKeys.DistributionName)).With("federationKey").
                                    EqualTo(this._federationKey).GetInstance
                                    <Database>();
            return db;
        }

        /// <summary>
        /// Gets or sets the federation key. (CustomerId)
        /// </summary>
        /// <value>
        /// The federation key.
        /// </value>
        protected long FederationKey { get; set; }

        protected  FederationType Federation
        {
            get { return _federation; }
        }

        /// <summary>
        /// Gets the return value.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        protected static int GetReturnValue(DbCommand cmd)
        {
            foreach (DbParameter dbParameter in cmd.Parameters)
            {
                if (dbParameter.Direction == ParameterDirection.ReturnValue && dbParameter.Value != null && dbParameter.Value is int)
                    return (int)dbParameter.Value;
            }
            return -1;
        }

        /// <summary>
        /// Validateds the name of the connection string.
        /// </summary>
        private void ValidatedConnectionStringName()
        {
            if (string.IsNullOrEmpty(this._connectionString))
            {
                throw new ArgumentException(StringResources.Repository_NoConnectionString);
            }
        }

        private void ValidateFederation()
        {
            if(this._federation == FederationType.Member && this._federationKey == 0)
            {
                throw new ArgumentException(StringResources.Repository_InvalidMemberFederation);
            }
        }
    }
}

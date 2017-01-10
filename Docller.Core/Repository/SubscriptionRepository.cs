using System;
using System.Dynamic;
using Docller.Core.DB;
using Docller.Core.Infrastructure;
using Docller.Core.Models;
using Docller.Core.Repository.Mappers;
using Microsoft.Practices.EnterpriseLibrary.Data;


namespace Docller.Core.Repository
{
    public class SubscriptionRepository : BaseRepository, ISubscriptionRepository
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionRepository"/> class.
        /// </summary>
        /// <param name="federation">The federation.</param>
        /// <param name="federationKey">The federation key.</param>
        /// <remarks></remarks>
        public SubscriptionRepository(FederationType federation, long federationKey):base(federation,federationKey)
        {

        }

        
        /// <summary>
        /// Subscribes the specified customer.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <returns></returns>
        public CustomerSubInfo Subscribe(Customer customer)
        {
            Database db = this.GetDb();
            ModelParameterMapper<Customer> parameterMapper = new ModelParameterMapper<Customer>(db, customer);
            parameterMapper.Map(x => x.AdminUser, x => x.Email).ToParameter("@AdminEmail").MapByName(
                x => x.AdminUser, x => x.Password).MapByName(x=>x.AdminUser, x => x.PasswordSalt);

            int returnVal = SqlDataRepositoryHelper.ExecuteNonQuery(db, customer.InsertProc, customer, parameterMapper);
            CustomerSubInfo spOutput = new CustomerSubInfo
                                           {
                                               ReturnVal = returnVal,
                                               IsExistingUser =
                                                   Convert.ToBoolean(
                                                       parameterMapper.GetOutputParamValue("@IsExistingUser"))
                                           };
            customer.AdminUser.UserId = Convert.ToInt32(parameterMapper.GetOutputParamValue("@AdminUserId"));
            customer.CustomerId = Convert.ToInt64(parameterMapper.GetOutputParamValue("@NewCustomerId"));
            return spOutput;
        }

        //public int Subscribe(Company company)
        //{
        //    Database db = this.GetDb();
        //    ModelParameterMapper<Company> parameterMapper = new ModelParameterMapper<Company>(db, company);
        //    return SqlDataRepositoryHelper.ExecuteNonQuery(db, company.InsertProc, company, parameterMapper);
        //}

        /// <summary>
        /// Determines whether [is domain URL in use] [the specified domain URL].
        /// </summary>
        /// <param name="domainUrl">The domain URL.</param>
        /// <returns>
        ///   <c>true</c> if [is domain URL in use] [the specified domain URL]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDomainUrlInUse(string domainUrl)
        {
            return Convert.ToBoolean(this.GetDb().ExecuteScalar(StoredProcs.IsDomainUrlExists, domainUrl));
        }

        /// <summary>
        /// Gets the customer id.
        /// </summary>
        /// <param name="domainUrl">The domain URL.</param>
        /// <returns></returns>
        public Customer GetCustomer(string domainUrl)
        {
            Customer result = SqlDataRepositoryHelper.Get(this.GetDb(), DefaultMappers.ForCustomer,  domainUrl, null);
            return result;
        }

        public Customer GetCustomer(long customerId)
        {
            Customer result = SqlDataRepositoryHelper.Get(this.GetDb(), DefaultMappers.ForCustomer, null, customerId);
            return result;
        }

        public void UpdateCustomer(Customer customer)
        {
            Database db = GetDb();
            ModelParameterMapper<Customer> modelParameterMapper = new ModelParameterMapper<Customer>(db, customer);
            SqlDataRepositoryHelper.ExecuteNonQuery(db, StoredProcs.UpdateCustomerDetails, customer, modelParameterMapper);
        }
    }
}
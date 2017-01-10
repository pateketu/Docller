using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.DB;
using Docller.Core.Models;
using Docller.Core.Repository.Accessors;
using Docller.Core.Repository.Collections;
using Docller.Core.Repository.Mappers;
using Docller.Core.Repository.Mappers.StoredProcMappers;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository
{
    public class CustomerSubscriptionRepository : BaseRepository, ICustomerSubscriptionRepository
    {
        public CustomerSubscriptionRepository(FederationType federation, long federationKey) : base(federation, federationKey)
        {
        }


        public RepositoryStatus SubscribeCompanies(long customerId,  long projectId, PermissionFlag projectPermissions, bool addAllProjectFolders,long folderId, PermissionFlag folderPermissions, IEnumerable<Company> companies, out IEnumerable<UserInvitationError> userWithErrors)
        {
            Database db = this.GetDb();
            CompanyCollection companyCollection = new CompanyCollection(companies);
            List<User> users = new List<User>();
            //prepare user collection to send to stoed proc
            foreach (Company company in companies)
            {
               users.AddRange(company.Users);     
            }
            UserCollection userCollection = new UserCollection(users);
            GenericParameterMapper mapper = new GenericParameterMapper(db);
            StoredProcAccessor<UserInvitationError> accessor =
                db.CreateStoredProcAccessor(StoredProcs.SubscribeCompanies,
                                           mapper,
                                            MapBuilder<UserInvitationError>.BuildAllProperties());

            userWithErrors = accessor.Execute(customerId, projectId, projectPermissions, addAllProjectFolders,folderId, folderPermissions, userCollection, companyCollection);

            if (mapper.ReturnValue != null)
            {
                return (RepositoryStatus) mapper.ReturnValue;
            }
            return RepositoryStatus.Unknown;
        }

        /// <summary>
        /// Subsribes the new customer.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public int UpdateSubscription(Customer customer, long companyId)
        {
            Database db = this.GetDb();
            ModelParameterMapper<Customer> parameterMapper = new ModelParameterMapper<Customer>(db, customer);
            parameterMapper.Map(x => x.AdminUser, x => x.UserId)
                .ToParameter("@UserId")
                .Map(x => x.AdminUser, x => x.Email)
                .
                ToParameter("@AdminEmail")
                .Map(x => x.AdminUser, x => x.CustomerPermissions)
                .ToParameter("@UserPermissions")
                .Map(companyId)
                .ToParameter("@CompanyId");

            int returnVal = SqlDataRepositoryHelper.ExecuteNonQuery(db, StoredProcs.UpdateCompanyAndUserForNewCustomer,
                                                                    customer, parameterMapper);
            return returnVal;
        }

        public IEnumerable<Company> GetSubscribers(long projectId)
        {
            Database db = this.GetDb();
            StoredProcAccessor<Company> accessor = db.CreateStoredProcAccessor<Company>(StoredProcs.GetSubscribers,
                                                                                        new GenericParameterMapper(db),
                                                                                        new SubscribersMapper());
            return accessor.Execute(projectId);

        }

        public IEnumerable<Company> GetSubscribers()
        {
            return GetSubscribers(0);
        }

        public User GetUserSubscriptionInfo(string userName, bool loadCompanyInfo)
        {
            Database db = this.GetDb();

            IEnumerable<User> users = db.ExecuteSprocAccessor(StoredProcs.GetUserSubscriptionInfo, new UserSubscriptionInfoResultSetMapper(),
                                                              userName, loadCompanyInfo);
            return users.FirstOrDefault();
        }
    }
}
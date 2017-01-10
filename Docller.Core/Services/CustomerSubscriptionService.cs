using System;
using System.Collections.Generic;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Repository;

namespace Docller.Core.Services
{
    public class CustomerSubscriptionService : ServiceBase<ICustomerSubscriptionRepository>, ICustomerSubscriptionService
    {
        public CustomerSubscriptionService(ICustomerSubscriptionRepository repository) : base(repository)
        {

        }

        public IEnumerable<User> SubscribeCompanies(long projectId, IEnumerable<Company> companies,  out IEnumerable<UserInvitationError> errors)
        {
            return SubscribeCompanies(projectId, PermissionFlag.DefaultFlag, companies, out errors);
        }

        public IEnumerable<User> SubscribeCompanies(long projectId, PermissionFlag permissionFlag, IEnumerable<Company> companies,
            out IEnumerable<UserInvitationError> errors)
        {

            return SubscribeCompanies(projectId, permissionFlag, permissionFlag != PermissionFlag.DefaultFlag, 0,
                PermissionFlag.None, companies, out errors);
        }

        public IEnumerable<User> SubscribeCompanies(long projectId, long folderId, PermissionFlag permissionFlag, IEnumerable<Company> companies,
            out IEnumerable<UserInvitationError> errors)
        {
            return SubscribeCompanies(projectId, PermissionFlag.DefaultFlag, false, folderId,permissionFlag, companies, out errors);
        }

        public void UpdateSubscription(Customer customer)
        {
            long companyId = IdentityGenerator.Create(IdentityScope.Company,customer.CustomerId);
            this.Repository.UpdateSubscription(customer,companyId);
        }

        public IEnumerable<SubscriberItem> Search(long projectId, string input)
        {
            IEnumerable<SubscriberItem> subscribers = CacheHelper.GetOrSet(
                Utils.GetKeyForProject(SessionAndCahceKeys.AllSubscribers, this.Context.CustomerId, projectId),
                CacheDurationHours.Default,
                () => GetSubscribes(projectId));

            return (input.Trim().Length <= 3
                               ? subscribers.Where(subscriberItem => subscriberItem.IsStartsWith(input))
                               : subscribers.Where(subscriberItem => subscriberItem.Contains(input)));

            
        }

        public IEnumerable<Company> GetSubscribedCompanies()
        {
            return this.Repository.GetSubscribers();
            /*IEnumerable<Company> companies = CacheHelper.GetOrSet(
                    Utils.GetKeyForCustomer(SessionAndCahceKeys.AllCustomerSubscribers, this.Context.CustomerId),
                    CacheDurationHours.Default,
                    () => this.Repository.GetSubscribers());
            return companies;*/
        }

        public IEnumerable<Company> GetSubscribedCompanies(long projectId)
        {
            IEnumerable<Company> companies = this.Repository.GetSubscribers(projectId);
            return companies;
        }

        public virtual SubscriberItemCollection GetSubscribers(long projectId, string[] ids)
        {
            SubscriberItemCollection items = new SubscriberItemCollection();
            if (ids != null && ids.Length > 0)
            {
                IEnumerable<SubscriberItem> subscribers = CacheHelper.GetOrSet(
                    Utils.GetKeyForProject(SessionAndCahceKeys.AllSubscribers, this.Context.CustomerId, projectId),
                    CacheDurationHours.Default,
                    () => GetSubscribes(projectId));

                items.AddRange(ids.Select(id => subscribers.FirstOrDefault(s => s.Id.Equals(id))));
            }
            return items;
        }

        public User GetUserSubscriptionInfo(string userName, bool loadCompanyInfo)
        {
            return this.Repository.GetUserSubscriptionInfo(userName, loadCompanyInfo);
        }



        protected virtual IEnumerable<SubscriberItem> GetSubscribes(long projectId)
        {
            IEnumerable<Company> companies = this.Repository.GetSubscribers(projectId);
            List<SubscriberItem> subscribers = new List<SubscriberItem>();
            foreach (Company company in companies)
            {
                subscribers.Add(new SubscriberCompany(company));

                subscribers.AddRange(company.Users.Select(user => new SubscriberUser(user)));
            }
            return subscribers;
        }

        private static void PopulateUserInfo(IEnumerable<Company> companies, IEnumerable<User> allUsers)
        {
            foreach (Company company in companies)
            {
                if (company.CompanyId <= 0)
                {
                    company.CompanyId = IdentityGenerator.Create(IdentityScope.Company);
                }

                for (int index = 0; index < company.Users.Count; index++)
                {
                    User user = company.Users[index];
                    User u = (from usr in allUsers
                              where usr.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase)
                              select usr).FirstOrDefault();
                    u.Company = company;
                    company.Users[index] = u;
                }
            }

        }

        private List<Company> PreProcess(IEnumerable<Company> companies, out IEnumerable<User> allUsers)
        {
            IUserService userService = ServiceFactory.GetUserService();

            List<Company> uniqueCompanies = companies.GroupBy(x => x.CompanyName).Select(g => g.First()).ToList();

            //Get all users
            IEnumerable<User> users = uniqueCompanies.SelectMany(company => company.Users);

            //Add any new users to the Users table
            allUsers = userService.EnsureUsers((from u in users
                                                                  select u.Email));
            List<Company> uCompanies = new List<Company>(uniqueCompanies);
            PopulateUserInfo(uCompanies, allUsers);
            return uCompanies;
        }

         private IEnumerable<User> SubscribeCompanies(long projectId, PermissionFlag projectPermissions, bool addPermissionsAllProjectFolders, long folderId, PermissionFlag folderPermissions,
                                                     IEnumerable<Company> companies,
                                                     out IEnumerable<UserInvitationError> errors)
         {
             IEnumerable<User> allUsers;
             IEnumerable<Company> uCompanies = PreProcess(companies, out allUsers);
             //Add the users to UserCache table and also add companies
             //Call a Stored proc which inserts all companies and users into Companies, UserCache and ProjectsUser table
             this.Repository.SubscribeCompanies(this.Context.CustomerId, projectId, projectPermissions,
                 addPermissionsAllProjectFolders, folderId, folderPermissions, uCompanies, out errors);

             /*IEnumerable<User> newUsers = from all in allUsers
                                          where all.IsNew
                                          select all;
             return newUsers;*/
             return allUsers;
         }
    }
}
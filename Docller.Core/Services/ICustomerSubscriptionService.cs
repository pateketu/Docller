using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.Core.Services
{
    public interface ICustomerSubscriptionService
    {

        /// <summary>
        /// Subscribes the users to the Project
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="companies">The companies.</param>
        /// <returns>List of new users who needs to be send an intial welcome email</returns>
        IEnumerable<User> SubscribeCompanies(long projectId, IEnumerable<Company> companies, out IEnumerable<UserInvitationError> errors);
        IEnumerable<User> SubscribeCompanies(long projectId, PermissionFlag permissionFlag, IEnumerable<Company> companies, out IEnumerable<UserInvitationError> errors);
        IEnumerable<User> SubscribeCompanies(long projectId, long folderId, PermissionFlag permissionFlag, IEnumerable<Company> companies, out IEnumerable<UserInvitationError> errors);

        /// <summary>
        /// Subsribes the new customer.
        /// </summary>
        /// <param name="customer">The customer.</param>
        void UpdateSubscription(Customer customer);

        IEnumerable<SubscriberItem> Search(long projectId, string input);

        IEnumerable<Company> GetSubscribedCompanies();

        IEnumerable<Company> GetSubscribedCompanies(long projectId);

        SubscriberItemCollection GetSubscribers(long projectId, string[] ids);

        User GetUserSubscriptionInfo(string userName, bool loadCompanyInfo);
    }
}

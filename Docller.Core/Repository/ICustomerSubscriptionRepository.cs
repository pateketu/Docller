using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.Core.Repository
{
    public interface ICustomerSubscriptionRepository: IRepository
    {
       /// <summary>
       /// 
       /// </summary>
       /// <param name="customerId"></param>
       /// <param name="projectId"></param>
       /// <param name="projectPermissions"></param>
       /// <param name="folderId"></param>
       /// <param name="folderPermissions"></param>
       /// <param name="companies"></param>
       /// <param name="userWithErrors"></param>
       /// <returns></returns>
        RepositoryStatus SubscribeCompanies(long customerId, long projectId, PermissionFlag projectPermissions, bool addAllProjectFolders, long folderId, PermissionFlag folderPermissions, IEnumerable<Company> companies, out IEnumerable<UserInvitationError> userWithErrors);

        /// <summary>
        /// Subsribes the new customer.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <param name="companyId">The company id.</param>
        /// <returns></returns>
        int UpdateSubscription(Customer customer, long companyId);


        IEnumerable<Company> GetSubscribers(long projectId);

        IEnumerable<Company> GetSubscribers();

        User GetUserSubscriptionInfo(string userName, bool loadCompanyInfo);
    }
}

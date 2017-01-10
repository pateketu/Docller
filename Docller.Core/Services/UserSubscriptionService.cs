using Docller.Core.Models;
using Docller.Core.Repository;

namespace Docller.Core.Services
{
    public class UserSubscriptionService :ServiceBase<IUserSubscriptionRepository>, IUserSubscriptionService
    {
        public UserSubscriptionService(IUserSubscriptionRepository repository) : base(repository)
        {
        }

        #region Implementation of IUserSubscriptionService

        public void UpdateUser(User user)
        {
            this.Repository.UpdateUser(user);
        }

        #endregion

       
    }
}
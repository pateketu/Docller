using Docller.Core.DB;
using Docller.Core.Models;
using Docller.Core.Repository.Mappers;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository
{
    public class UserSubscriptionRepository :BaseRepository, IUserSubscriptionRepository
    {
        public UserSubscriptionRepository(FederationType federation, long federationKey)
            : base(federation, federationKey)
        {
        }
        
        #region Implementation of IUserSubscriptionsRepository
        
        public void UpdateUser(User user)
        {
            Database db = this.GetDb();
            ModelParameterMapper<User> parameterMapper = new ModelParameterMapper<User>(db, user);
            int returnVal = SqlDataRepositoryHelper.ExecuteNonQuery(db, StoredProcs.UpdateUserCache, user,
                                                                    parameterMapper);
            }

        #endregion
    }
}
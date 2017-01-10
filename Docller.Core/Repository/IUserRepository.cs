using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Models;

namespace Docller.Core.Repository
{
    public interface IUserRepository : IRepository
    {
        User GetUserForLogOn(string userName);
        int Update(User user);
        User GetUserLogOnInfo(string userName, long customerId);
        int UpdateUserFailedLoginAttempt(User user);
        IEnumerable<User> EnsureUsers(IEnumerable<User> usersToCheck);
    }
}

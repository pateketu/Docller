using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Models;

namespace Docller.Core.Services
{
    public interface IUserService
    {
        UserServiceStatus LogOn(string userName, string password);
        UserServiceStatus Update(User user);
        UserServiceStatus UpdateUserFailedLoginAttempt(User user);
        User GetUserInfo(string userName, long customerId);
        User GetUserInfo(string userName, long customerId, bool loadCompanyInfo);
        bool IsUserExists(string username);
        string ResetPassword(string userName);
        IEnumerable<User> EnsureUsers(IEnumerable<string> userNames);
    }

    public enum UserServiceStatus
    {
        Unknown = 0,
        InvalidUserNameOrPassword = 1,
        UserAccountLocked=2,
        LoginSuccessAndForcePasswordChange=3,
        LoginSuccess=4,
        Success=5,
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Repository;

namespace Docller.Core.Services
{
    public class UserService : ServiceBase<IUserRepository>, IUserService
    {

        public UserService(IUserRepository repository)
            : base(repository)
        {
        }

        /// <summary>
        /// Logs the on.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public UserServiceStatus LogOn(string userName, string password)
        {
            User user = this.Repository.GetUserForLogOn(userName);
            if (user == null)
            {
                return UserServiceStatus.InvalidUserNameOrPassword;
            }
            if (user.IsLocked)
            {
                return UserServiceStatus.UserAccountLocked;
            }

            if (ValidatePassword(password, user))
            {
                if (user.ForcePasswordChange)
                {
                    return UserServiceStatus.LoginSuccessAndForcePasswordChange;
                }
                else
                {
                    return UserServiceStatus.LoginSuccess;
                }
            }
            return UserServiceStatus.InvalidUserNameOrPassword;
        }

        /// <summary>
        /// Updates the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public UserServiceStatus Update(User user)
        {
            Security.PopulatePassword(user);
            int returnVal = this.Repository.Update(user);

            //Update the user details in each federation
            ServiceFactory.GetUserSubscriptionService().UpdateUser(user);
            
            if (returnVal == 0)
            {
                return UserServiceStatus.Success;
            }
            return UserServiceStatus.Unknown;
        }

        public User GetUserInfo(string userName, long customerId)
        {
            return GetUserInfo(userName, customerId, true);
        }

        public User GetUserInfo(string userName, long customerId, bool loadCompanyInfo)
        {
            User user = this.Repository.GetUserLogOnInfo(userName, customerId);
            if (user != null)
            {
                //Get Customer Specific info from customer federation
                ICustomerSubscriptionService customerSubscriptionService =
                    ServiceFactory.GetCustomerSubscriptionService(customerId);
                User userInfo = customerSubscriptionService.GetUserSubscriptionInfo(userName, loadCompanyInfo);

                user.Company = userInfo.Company;
                user.Projects = userInfo.Projects;
                user.HasMultipleProjects = userInfo.HasMultipleProjects;
                user.CustomerPermissions = userInfo.CustomerPermissions;
            }
            return user;
        }

        /// <summary>
        /// to check if user exists or not
        /// </summary>
        /// <param name="username">username email</param>
        /// <returns> exists or not</returns>
        public bool IsUserExists(string username)
        {
            User user = this.Repository.GetUserForLogOn(username);
            return user != null ? true : false;
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="userName">username email</param>
        /// <returns>return Reset Password operation status</returns>
        public string ResetPassword(string userName)
        {
            User user = Security.GetUserWithTempPassword(userName);
            
            string password = user.Password;
            user.ForcePasswordChange = true;
            user.FailedLogInAttempt = 0;
            user.IsLocked = false;

            UserServiceStatus status = this.Update(user);
            UserServiceStatus resetFailedAttemptStatus = this.UpdateUserFailedLoginAttempt(user);
            
            return status == UserServiceStatus.Success && resetFailedAttemptStatus == UserServiceStatus.Success ? password : String.Empty;
        }

        /// <summary>
        /// Checks if the users exists in the user table and if not than adds the users
        /// </summary>
        /// <param name="emails">The emails.</param>
        /// <returns></returns>
        public IEnumerable<User> EnsureUsers(IEnumerable<string > emails)
        {
            List<User> usersToCheck = new List<User>(emails.Count());
            StringDictionary passwordCache = new StringDictionary();
            foreach (string email in emails)
            {
                User user = Security.GetUserWithTempPassword(email);
                user.Email = email;
                passwordCache.Add(user.UserName, user.Password);
                Security.PopulatePassword(user);
                usersToCheck.Add(user);
            }
            IEnumerable<User> newUsers = this.Repository.EnsureUsers(usersToCheck);

            foreach (User newUser in newUsers)
            {
                if (newUser.IsNew)
                {
                    newUser.Password = passwordCache[newUser.Email];
                }
            }

            return newUsers;
        }

        #region "Private methods"

        /// <summary>
        /// Validates the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private static bool ValidatePassword(string password, User user)
        {
            string passwordHash = Security.CreatePasswordHash(password, user.PasswordSalt);
            return (passwordHash.Equals(user.Password, StringComparison.Ordinal));
        }

        #endregion


        public UserServiceStatus UpdateUserFailedLoginAttempt(User user)
        {
            int returnVal = this.Repository.UpdateUserFailedLoginAttempt(user);

            if (returnVal == 0)
            {
                return UserServiceStatus.Success;
            }
            return UserServiceStatus.Unknown;
        }
    }
}
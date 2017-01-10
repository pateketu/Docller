using System.Collections.Generic;
using System.Data;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Services;
using Docller.Tests;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace Docller.UnitTests
{
    /// <summary>
    /// Subscription repo
    /// </summary>
    [TestClass]
    public class UserServiceFixture : FixtureBase
    {
        /// <summary>
        /// Inits the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            FixtureBase.RegisterMappings();
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        /// <remarks></remarks>
        [ClassCleanup]
        public static void CleanUp()
        {
            ObjectFactory.EjectAllInstancesOf<Database>();
        }

        
        
        [TestMethod]
        public void Verify_LogOn()
        {
            string password = "byXT4.R";
            string userName = "UnitTestUser@Docller.com";
            string salt = Security.CreateSalt(5);
            string passwordHashed = Security.CreatePasswordHash(password, salt);
            string insertUser =
                string.Format(
                    "INSERT INTO Users (Email,[Password],PasswordSalt, UserName, FirstName, LastName) VALUES('{0}',  '{1}', '{2}', '{0}', 'First','Last')", userName, passwordHashed, salt);
         
            GetDb().ExecuteNonQuery(CommandType.Text, insertUser);
            
            IUserService service = ObjectFactory.GetInstance<IUserService>();

            UserServiceStatus status = service.LogOn(userName, password);
            Assert.IsTrue(status == UserServiceStatus.LoginSuccessAndForcePasswordChange);
        }

        [TestMethod]
        public void Verify_UpdatedUser()
        {
            string password = "byXT4.R";
            string userName = "UnitTestUser@Docller.com";
            string salt = Security.CreateSalt(5);
            string passwordHashed = Security.CreatePasswordHash(password, salt);
            string insertUser =
                string.Format(
                    "INSERT INTO Users (Email,[Password],PasswordSalt, UserName, FirstName, LastName) VALUES('{0}',  '{1}', '{2}', '{0}', 'First','Last')", userName, passwordHashed, salt);
         
            GetDb().ExecuteNonQuery(CommandType.Text, insertUser);
            int userId = (int) this.GetValue("Users", "UserId");

            string insertUserToCache =
             string.Format(
                 "INSERT INTO UserCache (UserId, CustomerId, UserName, FirstName, LastName, Email) VALUES('{0}',  '1', '{1}', 'First','Last','{1}')", userId, userName);
            GetDb().ExecuteNonQuery(CommandType.Text, insertUserToCache);
         

            IUserService service = ObjectFactory.GetInstance<IUserService>();
            User u = new User() {UserName=userName,Password = "password_123",Email = userName,FirstName = "NewFirst", LastName = "NewLast"};
            service.Update(u);

            string fName = this.GetValue("Users", "FirstName").ToString();
            Assert.IsTrue(fName.Equals("NewFirst"));

            fName = this.GetValue("UserCache", "FirstName").ToString();
            Assert.IsTrue(fName.Equals("NewFirst"));




        }

        [TestMethod]
        public void Verify_EnsureUsers()
        {
            List<string> emailsAddresses = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                emailsAddresses.Add(string.Format("User{0}@Docller.com", i));
            }
            IUserService userService = ObjectFactory.GetInstance<IUserService>();
            IEnumerable<User>  users = userService.EnsureUsers(emailsAddresses);
            //Ensure we have 10 records in users table
            int count = this.GetCount("Users");
            Assert.IsTrue(count == 10, "10 users were expected");
            //Check we have 10 new users
            var newUsers = from u in users
                           where u.IsNew
                           select u;
            
            Assert.IsTrue(newUsers.Count() == 10, "10 new users were expected");
            //try one of the user to login
            User user = newUsers.FirstOrDefault();
            UserServiceStatus status = userService.LogOn(user.UserName, user.Password);
            Assert.IsTrue(status == UserServiceStatus.LoginSuccessAndForcePasswordChange, "User was not able to login");
            
            //Add extra users
            for (int i = 20; i < 25; i++)
            {
                emailsAddresses.Add(string.Format("User{0}@Docller.com", i));
            }

            users = userService.EnsureUsers(emailsAddresses);
            count = this.GetCount("Users");
            Assert.IsTrue(count == 15, "15 users were expected");
            
            newUsers = from u in users
                       where u.IsNew
                       select u;
            
            Assert.IsTrue(newUsers.Count() == 5, "5 new users were expected");

        }

       
    }
}

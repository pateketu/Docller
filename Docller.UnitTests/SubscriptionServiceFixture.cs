using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Services;
using Docller.Tests;
using Docller.UI.Common;
using Docller.UI.Models;
using Docller.UnitTests.Mocks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace Docller.UnitTests
{
    /// <summary>
    /// Subscription repo
    /// </summary>
    [TestClass]
    public class SubscriptionServiceFixture : FixtureBase
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

        /// <summary>
        /// Verifies the customer added.
        /// </summary>
        /// <remarks></remarks>
        [TestMethod]
        public void Verify_Customer_Is_Subscribed()
        {
            ISubscriptionService service = ServiceFactory.GetSubscriptionService();
            Customer customer = new Customer()
                                    {
                                        CustomerName = "Test Customer",
                                        IsTrial = true,
                                        DomainUrl = "Customer1",
                                        ImageUrl = "/_layouts/image.jpg",
                                        AdminUser =
                                            new User() { Email = "UnitTest@Docller.com", Password = "TeampPassword", PasswordSalt = Security.CreateSalt(5) }
                                    };
            service.Subscribe(customer);
            
            bool verify = Verify("Customers", "CustomerName", "Test Customer");
            Assert.IsTrue(verify);

            verify = Verify("Users", "UserName", AdminUserName);
            Assert.IsTrue(verify);

            verify = Verify("Companies", "CompanyName", "Test Customer");
            Assert.IsTrue(verify);

            
            verify = VerifyExists("CompanyUsers", "CompanyId");
            Assert.IsTrue(verify);

            verify = Verify("Customers", "DomainUrl", "Customer1");
            Assert.IsTrue(verify);

            verify = Verify("UserCache", "UserName", AdminUserName);
            Assert.IsTrue(verify);

            //try dup customer
            SubscriptionServiceStatus results = service.Subscribe(new Customer()
                               {
                                   CustomerName = "Test Customer",
                                   IsTrial = true,
                                   DomainUrl = "Customer2",
                                   ImageUrl = "/_layouts/image.jpg",
                                   AdminUser =
                                       new User() {Email = "UnitTest@Docller.com", Password = "TeampPassword", PasswordSalt = Security.CreateSalt(5)}
                               });

            Assert.IsTrue(results == SubscriptionServiceStatus.ExistingCustomer);

            //try dup domain url
            results = service.Subscribe(new Customer()
                                       {
                                           CustomerName = "Random",
                                           IsTrial = true,
                                           ImageUrl = "/_layouts/image.jpg",
                                           DomainUrl = "Customer1",
                                           AdminUser =
                                               new User()
                                                   {
                                                       Email = "Random@Docller.com",
                                                       Password = "TeampPassword",
                                                       PasswordSalt = Security.CreateSalt(5)
                                                   }
                                       });
            Assert.IsTrue(results == SubscriptionServiceStatus.DomainUrlInUse);
        }

        [TestMethod]
        public void Verify_Subscribing_SameUsers_To_Different_Company()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = this.AddProject(customerId);

            ICustomerSubscriptionService customerSubscriptionService =
                ObjectFactory.GetInstance<ICustomerSubscriptionService>();
            Company company = new Company() { CompanyName = "Acme Ltd", Users = new List<User>() };
            company.Users.Add(new User() { Email = "Test@Docller.com" });
            IEnumerable<UserInvitationError> errros;
            customerSubscriptionService.SubscribeCompanies(projectId, new List<Company>() {company}, out errros);
            Assert.IsTrue(!errros.Any(), "Errors found while no errors were expected");

            Company company1 = new Company() { CompanyName = "Widget LLC", Users = new List<User>() };
            company1.Users.Add(new User() { Email = "Test@Docller.com" });
            customerSubscriptionService.SubscribeCompanies(projectId, new List<Company>() { company1 }, out errros);
            Assert.IsTrue(errros.Count() == 1, "Errors were expected");
            Assert.IsTrue(errros.First().ExistingCompany.Equals("Acme Ltd"));
           
        }

        [TestMethod]
        public void Verify_Subscribing_SameUser_To_Same_Company()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = this.AddProject(customerId);

            ICustomerSubscriptionService customerSubscriptionService =
                ObjectFactory.GetInstance<ICustomerSubscriptionService>();
            Company company = new Company() { CompanyName = "Acme Ltd", Users = new List<User>() };
            company.Users.Add(new User() { Email = "Test@Docller.com" });
            IEnumerable<UserInvitationError> errros;
            customerSubscriptionService.SubscribeCompanies(projectId, new List<Company>() { company }, out errros);
            Assert.IsTrue(!errros.Any(), "Errors found while no errors were expected");

            Company company1 = new Company() { CompanyName = "Acme Ltd", Users = new List<User>() };
            company1.Users.Add(new User() { Email = "Test@Docller.com" });
            customerSubscriptionService.SubscribeCompanies(projectId, new List<Company>() { company1 }, out errros);
            Assert.IsTrue(errros.Count() == 0, "No Errors were expected");
        }


        [TestMethod]
        public void Verify_SubscribeCompanies()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = this.AddProject(customerId);
            
            ICustomerSubscriptionService customerSubscriptionService =
                ObjectFactory.GetInstance<ICustomerSubscriptionService>();
            List<Company> companies =new List<Company>();
            IEnumerable<UserInvitationError> errros;
            Company fubarCompany = new Company() {CompanyName = "Fubar",Users =new List<User>()};
            for (int i = 0; i < 5; i++)
            {
                fubarCompany.Users.Add(new User() { Email = string.Format("User{0}@Docller.com", i) });
            }
            companies.Add(fubarCompany);
            //Ensure Users and upate first/last name
            IUserService userService = ObjectFactory.GetInstance<IUserService>();

            userService.EnsureUsers(new List<string>() {"User0@Docller.com"});
            userService.Update(new User()
                                   {
                                       UserName = "User0@Docller.com",
                                       Email = "User0@Docller.com",
                                       FirstName = "User",
                                       LastName = "LAst NAme",
                                       Password="password_123"
                                   });

            Company acmeCompany = new Company() {CompanyName = "AcmeCompany", Users = new List<User>()};
            
            for (int i = 5; i < 10; i++)
            {
                acmeCompany.Users.Add(new User() { Email = string.Format("User{0}@Docller.com", i) });
            }
            companies.Add(acmeCompany);
            
            customerSubscriptionService.SubscribeCompanies(projectId, companies, out errros);

            int userCacheCount = this.GetCount("UserCache");
            Assert.IsTrue(userCacheCount == 11, "11 Users were expected in UserCache table");
            object firstName = this.GetValue("UserCache", "FirstName", "WHERE UserName='User0@Docller.com'");
            Assert.IsTrue(firstName != null, "First Name for User0 was null");
            int fubarCompanyId = int.Parse(this.GetValue("Companies", "CompanyId", "WHERE CompanyName='Fubar'").ToString());
            int fubarCompanyCount = this.GetCount("CompanyUsers", "WHERE CompanyId=" + fubarCompanyId);
            Assert.IsTrue(fubarCompanyCount == 5, "5 Users were expected for Fubar company");
            int projectUserscount = this.GetCount("ProjectUsers");
            Assert.IsTrue(projectUserscount == 11, "11 Users were expected in ProjectUsers count");

        }

        [TestMethod]
        public void Verify_GetSubscribers()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = this.AddProject(customerId);
            
            this.SubscribeCompanies(projectId);
            MockCustomerSubscriptionService subscriptionService = new MockCustomerSubscriptionService();
            IEnumerable<SubscriberItem> subscriberItems = subscriptionService.GetAllSubscribes(projectId);

           //we ar expecting two companies
            Assert.IsTrue(subscriberItems.Count() == 8, "8 items were expected");

            Assert.IsTrue(subscriberItems.OfType<SubscriberCompany>().Count() == 2, "2 Companies were expected");

            Assert.IsTrue(subscriberItems.OfType<SubscriberUser>().Count() == 6, "6 users were expected");

            IEnumerable<SubscriberItem> search = subscriptionService.Search(projectId, "acm");
           
            Assert.IsTrue(search.Count() == 1, "Only one item was expected");

            search = subscriptionService.Search(projectId, "user");

            Assert.IsTrue(search.Count() == 5, "Only 5 items were expected");
        }

        [TestMethod]
        public void Verify_InviteUsers_Validation()
        {
            List<InviteUsersViewModel> usersModal = new List<InviteUsersViewModel>();
            InviteUsersViewModel usersView  = new InviteUsersViewModel
                {
                    CompanyName = "VIATUNT",
                    Users = "pateket@gmail.com\nketul.patel@docller.com"
                };
            usersModal.Add(usersView);

            CompaniesModelBinder binder = new CompaniesModelBinder();
            ModelStateDictionary modelState = new ModelStateDictionary();
            IEnumerable<Company> companies;
            binder.TryBuildValidateModal(usersModal, modelState, out companies);

            Assert.IsTrue(modelState.IsValid);
            Assert.IsTrue(companies.Count() == 1, "One company was expected");
            Assert.IsTrue(companies.FirstOrDefault().Users.Count == 2, "Two users were expected");

            //Try duplicate companies
            usersView = new InviteUsersViewModel
            {
                CompanyName = "VIATUNT",
                Users = "pateket@gmail.com\nketul.patel@docller.com"
            };
            usersModal.Add(usersView);
            modelState = new ModelStateDictionary();
            binder.TryBuildValidateModal(usersModal, modelState, out companies);

            
            Assert.IsFalse(modelState.IsValid);
            usersModal.RemoveAt(1);
            //try duplicate emails
            usersView = new InviteUsersViewModel
            {
                CompanyName = "Faubar",
                Users = "pateket@gmail.com"
            };
            usersModal.Add(usersView);
            modelState = new ModelStateDictionary();
            binder.TryBuildValidateModal(usersModal, modelState, out companies);

            
            Assert.IsFalse(modelState.IsValid);
            
        }

        private void Add_LotsOfCompaniesAndUsers()
        {
            long customerId = 1;
            SetDocllerContext(customerId, "pateketu@gmail.com");
            int projectId = 1;

            string[] randomCompanyNames =
                {
                    "VIATUNT",
                    "APHEPHIA",
                    "DEPA",
                    "KARMALIMA",
                    "LOVELOCK",
                    "CELLICA",
                    "SPEWICA",
                    "APHEDAX",
                    "REPTIMA",
                    "DELERYGUE"
                };

            ICustomerSubscriptionService customerSubscriptionService =
                ObjectFactory.GetInstance<ICustomerSubscriptionService>();
            List<Company> companies = new List<Company>();

            for (int i = 0; i < 10; i++)
            {
                Company company  = new Company() { CompanyName = randomCompanyNames[i], Users = new List<User>() };

                for (int j = 0; j < 10; j++)
                {
                    company.Users.Add(new User() { Email = string.Format("User_{0}_{1}@Docller.com", j, randomCompanyNames[i]) });
                }
                companies.Add(company);            
            }
            IEnumerable<UserInvitationError> errros;
            customerSubscriptionService.SubscribeCompanies(projectId, companies, out errros);
        
        }


        protected override void ClearTables()
        {
            base.ClearTables();
        }
       
        //[TestMethod]
        //public void Verify_Company_Is_Added()
        //{
        //    SubscriptionRepository repo = new SubscriptionRepository(FederationType.None, 1);
        //    Company company = new Company() {CompanyName = "Company"};
        //    repo.Subscribe(company);
        //    bool verify = Verify("Companies", "CompanyName", "Company");

        //    Assert.IsTrue(verify);
        //}

       
    }
}

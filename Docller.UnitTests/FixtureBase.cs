using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.DB;
using Docller.Core.Images;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Services;
using Docller.Core.Storage;
using Docller.Tests.Mocks;
using Docller.UnitTests;
using Docller.UnitTests.Mocks;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using SnowMaker;
using StructureMap;


namespace Docller.Tests
{
    public class FixtureBase
    {
        public const string AdminUserName = "UnitTest@Docller.com";
        [TestCleanup]
        public void TestCleanup()
        {
            ClearTables();
        }

        /// <summary>
        /// Tests the initialize.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            ClearTables();
        }

        public static void RegisterMappings()
        {

            //EnterpriseLibraryContainer.Current = EnterpriseLibraryContainer.CreateDefaultContainer();

            FederationType rooFederationType = FederationType.None;
            FederationType memberFederationType = FederationType.None;
            //ObjectFactory.With("connectionString").EqualTo(someValueAtRunTime).GetInstance<IProductProvider>();
            ObjectFactory.Initialize(
                x =>
                    {
                        x.For<Database>().Use<SqlAzureDatabase>()
                            .Ctor<string>("connectionString").Is(Config.GetConnectionString())
                            .Ctor<FederationType>().Is(FederationType.None)
                            .Ctor<string>("federationName").Is(Config.GetValue<string>(ConfigKeys.FederatioName))
                            .Ctor<string>("distributionName").Is(Config.GetValue<string>(ConfigKeys.DistributionName))
                            .Ctor<object>("federationKey").Is((object) null);

                        x.For<RetryPolicy>().Use(RetryPolicyFactory.GetDefaultSqlConnectionRetryPolicy());

                       
                        x.For<ISubscriptionRepository>()
                            .Use<SubscriptionRepository>().Ctor<FederationType>().Is(rooFederationType)
                            .Ctor<long>().Is(0);
                        x.For<ISubscriptionService>().Use<SubscriptionService>();

                        x.For<IUserRepository>().Use<UserRepository>().Ctor<FederationType>().Is(rooFederationType).Ctor
                            <long>().Is(0);
                        x.For<IUserService>().Use<UserService>();

                        x.For<IProjectRepository>().Use<ProjectRepository>().Ctor<FederationType>().Is(
                            memberFederationType).Ctor<long>().Is(0);
                        x.For<IProjectService>().Use<ProjectService>();

                        x.For<IBlobStorageProvider>().Use<MockBlobStorageProvider>();

                        x.For<ISecurityRepository>().Use<SecurityRepository>().Ctor<FederationType>().Is(
                            memberFederationType).Ctor<long>().Is(0);
                        x.For<ISecurityService>().Use<SecurityService>();

                        x.For<ICustomerSubscriptionRepository>().Use<CustomerSubscriptionRepository>().Ctor
                            <FederationType>().Is(
                                memberFederationType).Ctor<long>().Is(0);
                        x.For<ICustomerSubscriptionService>().Use<CustomerSubscriptionService>();

                        x.For<IStorageRepository>().Use<StorageRepository>().Ctor<FederationType>().Is(
                            memberFederationType).Ctor<long>().Is(0);
                        x.For<IStorageService>().Use<StorageService>();

                        x.For<IUserSubscriptionRepository>().Use<UserSubscriptionRepository>().Ctor<FederationType>().Is(
                          memberFederationType).Ctor<long>().Is(0);
                        x.For<IUserSubscriptionService>().Use<UserSubscriptionService>();
                        

                        x.For<IUniqueIdGenerator>().Singleton().Use<UniqueIdGenerator>();
                        x.For<IOptimisticDataStore>().Singleton().Use(new FileOptimisticDataStore("C:\\Temp"));

                        x.For<ITransmittalRepository>().Use<TransmittalRepository>().Ctor<FederationType>().Is(
                         memberFederationType).Ctor<long>().Is(0);
                        x.For<ITransmittalService>().Use<TransmittalService>();
                        x.For<ILocalStorage>()
                         .Singleton()
                         .Use<LocalStorage>();

                        
                        x.For<IIssueSheetProvider>().Use<IssueSheetProvider>();
                        x.For<ITransmittalNotification>().Use<MockTransmittalNotification>();
                        x.For<IPathMapper>().Use<MockPathMapper>();
                        x.For<IGhostscriptLibraryLoader>().Use<MockGhostscriptLibraryLoader>();
                    }
                );


        }

        /// <summary>
        /// Gets the DB.
        /// </summary>
        /// <returns></returns>
        protected static Database GetDb()
        {
            Database db = ObjectFactory.GetInstance<Database>();
            return db;
        }

        protected void SetDocllerContext(long customerId, string userName)
        {
            ObjectFactory.Configure(
                x =>
                x.For<IDocllerContext>().HybridHttpOrThreadLocalScoped().Use(new MockContext(userName, customerId)));
        }
        /// <summary>
        /// Cleans up.
        /// </summary>
        protected virtual void ClearTables()
        {
            Database db = GetDb();
            db.ExecuteNonQuery(CommandType.Text, "delete from TransmittedFiles delete from TransmittalDistribution delete from Transmittals delete from CompanyFolderPermissions delete from FileAttachments delete from FileVersions delete from Files  delete from Status delete from UserCache delete from Folders delete from Customers delete from Users delete from CompanyUsers delete from Companies delete from ProjectUsers delete from Projects");
        }

        /// <summary>
        /// Verifies the first recoud's value in a column in a table
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <param name="expectedValue">The expected value.</param>
        /// <returns></returns>
        protected bool Verify(string table, string column, object expectedValue)
        {
            Database db = GetDb();
            object value = db.ExecuteScalar(CommandType.Text,
                                            string.Format(CultureInfo.InvariantCulture, "SELECT TOP 1 {0} FROM {1}",
                                                          column, table));
            if(expectedValue != null)
            {
                return expectedValue.Equals(value);
            }
            return false;
        }

        protected object GetValue(string table, string column)
        {
            return GetValue(table, column, string.Empty);
        }

        protected object GetValue(string table, string column, string whereClause)
        {
            StringBuilder queryBuidler = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "SELECT TOP 1 {0} FROM {1} ",
                                                          column, table));
            queryBuidler.Append(whereClause);
            Database db = GetDb();
            object value = db.ExecuteScalar(CommandType.Text, queryBuidler.ToString());
            return value;
        }

        protected int GetCount(string table)
        {
            return GetCount(table, string.Empty);
        }
        protected int GetCount(string table, string whereClause)
        {
            StringBuilder queryBuilder = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "SELECT COUNT(*) FROM {0} ",
                                                          table));
            queryBuilder.Append(whereClause);
            Database db = GetDb();
            object value = db.ExecuteScalar(CommandType.Text,queryBuilder.ToString().Trim());
            return Convert.ToInt32(value);
        }



        /// <summary>
        /// Verifies a column value exists
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected bool VerifyExists(string table, string column)
        {
            Database db = GetDb();
            object value = db.ExecuteScalar(CommandType.Text,
                                            string.Format(CultureInfo.InvariantCulture, "SELECT TOP 1 {0} FROM {1}",
                                                          column, table));
            return (value != null);
        }

        protected long AddCustomer()
        {
            ISubscriptionService service = ServiceFactory.GetSubscriptionService();
            Customer customer = new Customer()
                                    {
                                        CustomerName = "Test Customer",
                                        IsTrial = true,
                                        DomainUrl = "Customer1",
                                        ImageUrl = "/_layouts/image.jpg",
                                        AdminUser =
                                            new User()
                                                {
                                                    Email = "UnitTest@Docller.com",
                                                    UserName = "UnitTest@Docller.com",
                                                    Password = "TeampPassword",
                                                    PasswordSalt = Security.CreateSalt(5),
                                                    FirstName = "Test",
                                                    LastName = "TestLastName"
                                                }
                                    };
            service.Subscribe(customer);
            long customerid= long.Parse(this.GetValue("Customers", "CustomerId").ToString());
            IUserService userService = ServiceFactory.GetUserService();
            userService.Update(customer.AdminUser);
            //ServiceFactory.GetUserSubscriptionService().UpdateUser(customer.AdminUser);
            
            return customerid;

        }

        protected int AddProject(long customerId)
        {
            ProjectRepository projectRepository = new ProjectRepository(FederationType.None, 1);
            long id = IdentityGenerator.Create(IdentityScope.Project);
            projectRepository.Create(AdminUserName, PermissionFlag.Admin,
                                     new Project()
                                         {
                                             ProjectId =  id,
                                             BlobContainer = string.Format(StorageHelper.BlobContainerFormat, customerId, id),
                                             CustomerId = customerId,
                                             ProjectCode = "Code",
                                             ProjectName = "Project"
                                         },GetDefaultStatus());

            return Convert.ToInt32(id);
        }
        private static List<Status> GetDefaultStatus()
        {
            string[] allStatus = Config.GetValue<string>(ConfigKeys.DefaultStatus).Split(',');
            return allStatus.Select((allStatu, index) => new Status() {StatusText = allStatu, StatusId = index}).ToList();
        }
        protected long AddProjectViaService(long customerId)
        {
            IProjectService projectService = ServiceFactory.GetProjectService(customerId);
            Project project = new Project()
                                  {
                                      
                                      CustomerId = customerId,
                                      ProjectCode = "Code",
                                      ProjectName = "Project"
                                  };
            projectService.Create(AdminUserName, project);
            return project.ProjectId;
        }
        
        protected int AddFolder(long customerId, int projectId, int parentFolderId, string folderName)
        {
            Folder folder = new Folder() {FolderName = folderName};
            IStorageService storageService = ServiceFactory.GetStorageService(customerId);
            FolderCreationStatus status = storageService.CreateFolders(AdminUserName, projectId, parentFolderId,
                                         new List<Folder>() {folder});
            
            if(status.Status ==StorageServiceStatus.Success)
            {
                return (int) folder.FolderId;
            }
            return -1;
        }

        protected Company SubscribeCompanies(long projectId)
        {
            ICustomerSubscriptionService customerSubscriptionService =
                ObjectFactory.GetInstance<ICustomerSubscriptionService>();
            List<Company> companies = new List<Company>();

            Company acmeCompany = new Company() { CompanyName = "AcmeCompany", Users = new List<User>() };

            for (int i = 5; i < 10; i++)
            {
                acmeCompany.Users.Add(new User() { Email = string.Format("User{0}@Docller.com", i) });
            }
            companies.Add(acmeCompany);
            IEnumerable<UserInvitationError> errros;
            customerSubscriptionService.SubscribeCompanies(projectId, companies, out errros);
            return companies.First();
        }


      

    }

    
}
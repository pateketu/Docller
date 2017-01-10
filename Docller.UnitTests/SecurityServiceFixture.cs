using Docller.Common;
using Docller.Tests;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;

namespace Docller.UnitTests
{
    [TestClass]
    public class SecurityServiceFixture:FixtureBase
    {

        /// <summary>
        /// Inits the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Registry.RegisterMappings();
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
        public void Verify_User_ProjectCreationRights()
        {
            //long custmerId = base.AddCustomer();
            //int projectId = base.AddProject(custmerId);

            //ISecurityService securityService = ServiceFactory.GetSecurityService(custmerId);
            //bool canCreateProject = securityService.CanCreateProject(new ContextInfo()
            //                                   {
            //                                       CurrentCustomerId = custmerId,
            //                                       CurrentProjectId = projectId,
            //                                       CurrentUserName = AdminUserName
            //                                   });

            //Assert.IsTrue(canCreateProject, "AdminUser should have right to create projects");
        }
    }
}

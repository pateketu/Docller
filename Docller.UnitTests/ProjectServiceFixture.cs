using System;
using System.Collections.Generic;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Services;
using Docller.Core.Storage;
using Docller.Tests;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using Telerik.JustMock;

namespace Docller.UnitTests
{
    [TestClass]
    public class ProjectServiceFixture : FixtureBase
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
        public void Verify_AddingProject()
        {
            long customerId = this.AddCustomer();

            SetDocllerContext(customerId, AdminUserName);
            Project project = new Project() { ProjectName = "Test", ProjectCode = "Test", CustomerId = customerId };
            IBlobStorageProvider blobStorageProvider = Mock.Create<IBlobStorageProvider>();
            Mock.Arrange(() => blobStorageProvider.CreateContainer(project)).DoNothing().OccursOnce();

            IProjectService projectService = new ProjectService(ObjectFactory.GetInstance<IProjectRepository>(),
                                                                blobStorageProvider);
            ProjectServiceStatus status = projectService.Create(AdminUserName, project);
            Assert.IsTrue(status == ProjectServiceStatus.Success, "Status should have been success");

            //Ensure blob storage was called 
            Mock.Assert(blobStorageProvider);

            int projectCount = this.GetCount("Projects");

            int projectId = Convert.ToInt32(this.GetValue("Projects", "ProjectId"));
            string blobContainerName = string.Format(StorageHelper.BlobContainerFormat, customerId, projectId);

            bool verify = this.Verify("Projects", "BlobContainer", blobContainerName);
            Assert.IsTrue(verify);

            Assert.IsTrue(projectCount == 1);
            int projectPermissions = this.GetCount("ProjectUsers");
            Assert.IsTrue(projectPermissions == 1);


            int foldercount = this.GetCount("Folders");
            int configfoldercount = Config.GetValue<string>("DefaultFolders").Split(',').Length;
            Assert.IsTrue(foldercount == configfoldercount);

            //Try inserting duplicate project name
            status = projectService.Create(AdminUserName, project);

            Assert.IsTrue(status == ProjectServiceStatus.ExistingProject, "Status should have been ExistingProject");

            //make sure another project is added
            project.ProjectName = "NewProject";
            status = projectService.Create(AdminUserName, project);
            Assert.IsTrue(status == ProjectServiceStatus.Success, "Status should have been success");
            projectCount = this.GetCount("Projects");
            Assert.IsTrue(projectCount == 2);
        }


        [TestMethod]
        public void Verify_GetProjectDetails()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId,AdminUserName);
            long projectId = this.AddProject(customerId);
            IProjectService projectService = ObjectFactory.GetInstance<IProjectService>();
            Project project = projectService.GetProjectDetails(AdminUserName, projectId);
            Assert.IsNotNull(project, "Project is null");
            Assert.IsTrue(!string.IsNullOrEmpty(project.ProjectName),"Project Name should not be null");

        }

        [TestMethod]
        public void Verify_GetProjectStatuses()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            long projectId = this.AddProject(customerId);
            IProjectService projectService = ServiceFactory.GetProjectService(customerId);
            IEnumerable<Status> allStatus = projectService.GetProjectStatuses(projectId);
            Assert.IsTrue(allStatus.Count() == 8,"Expected number of statues were 8");

        }
    }
}

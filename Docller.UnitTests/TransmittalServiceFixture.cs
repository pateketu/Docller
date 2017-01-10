using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Services;
using Docller.Core.Storage;
using Docller.Tests;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using File = Docller.Core.Models.File;

namespace Docller.UnitTests
{
    [TestClass]
    public class TransmittalServiceFixture:FixtureBase
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
        /// 
        [ClassCleanup]
        public static void CleanUp()
        {
            ObjectFactory.EjectAllInstancesOf<Database>();
        }

        [TestMethod]
        public void Verify_CreatingTransmittal()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId,AdminUserName);
            long projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, (int)projectId, 0, "Test");
            int companyId = Convert.ToInt32(this.GetValue("Companies", "CompanyId"));
            IEnumerable<File> files = AddFiles(projectId, folderId, customerId);
            Company company = SubscribeCompanies(projectId);
            Transmittal transmittal = new Transmittal
                {
                    Files = new List<TransmittedFile>(),
                    Distribution = new List<TransmittalUser>(),
                    CreatedBy = new User() {UserName = AdminUserName},
                    Message = "message",
                    TransmittalNumber = "12222",
                    Subject = "fubar",
                    IsDraft =  true,
                    ProjectId = projectId
                };

            foreach (File file in files)
            {
                transmittal.Files.Add(new TransmittedFile() {FileId = file.FileId});
            }
            SubscriberItem subscriberItem = new SubscriberCompany() { CompanyId = company.CompanyId }; 
            ITransmittalService transmittalService = ServiceFactory.GetTransmittalService(customerId);

            var serviceStatus = transmittalService.CreateTransmittal(transmittal, new List<SubscriberItem>() { subscriberItem }, null);
            Assert.IsTrue(serviceStatus.Status == TransmittalServiceStatus.Success);
            //at this point there should be no entried in FolderPermissions table fopr new company
            int count = this.GetCount("CompanyFolderPermissions", "WHERE CompanyId=" + company.CompanyId);
            Assert.IsTrue(count == 0, string.Format("CompanyId {0} should not have access to folder id {1}", company.CompanyId,folderId));

            transmittal.IsDraft = false;
            serviceStatus =  transmittalService.CreateTransmittal(transmittal,new List<SubscriberItem>() {subscriberItem}, null);
            Assert.IsTrue(serviceStatus.Status == TransmittalServiceStatus.RequiredFieldsMissing);
            //at this point there should be no entried in FolderPermissions table fopr new company
            count = this.GetCount("CompanyFolderPermissions", "WHERE CompanyId=" + company.CompanyId);
            Assert.IsTrue(count == 0, string.Format("CompanyId {0} should not have access to folder id {1}", company.CompanyId, folderId));


            IProjectService projectService = ServiceFactory.GetProjectService(customerId);
            IEnumerable<Status> status = projectService.GetProjectStatuses(projectId);
            transmittal.TransmittalStatus = status.Where(x => x.StatusId == 1).FirstOrDefault();
            
            serviceStatus = transmittalService.CreateTransmittal(transmittal, new List<SubscriberItem>() { subscriberItem }, null);
            Assert.IsTrue(serviceStatus.Status == TransmittalServiceStatus.Success);

            count = this.GetCount("CompanyFolderPermissions", "WHERE CompanyId=" + company.CompanyId);
            Assert.IsTrue(count == 1, string.Format("CompanyId {0} should not have access to folder id {1}", company.CompanyId, folderId));



            //check to ensure status of the file is update
            object value = GetValue("Files", "StatusId", string.Format("WHERE FileId={0}", transmittal.Files[0].FileId));
            Assert.IsTrue(value.Equals(transmittal.TransmittalStatus.StatusId), "Status of the files does not seem to be updated");

            transmittal.TransmittalId = 9999; //brand new transmittal
            serviceStatus = transmittalService.CreateTransmittal(transmittal, new List<SubscriberItem>() { subscriberItem }, null);
            Assert.IsTrue(serviceStatus.Status == TransmittalServiceStatus.Success);



        }

        [TestMethod]
        public void Veirfy_Updating_Transmittal()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            long projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, (int)projectId, 0, "Test");
            IEnumerable<File> files = AddFiles(projectId, folderId, customerId);
            Company company = SubscribeCompanies(projectId);
            Transmittal transmittal = new Transmittal
            {
                Files = new List<TransmittedFile>(),
                Distribution = new List<TransmittalUser>(),
                CreatedBy = new User() { UserName = AdminUserName },
                Message = "Message",
                TransmittalNumber = "1",
                Subject = "fubar",
                IsDraft = true,
                ProjectId = projectId
            };

            foreach (File file in files)
            {
                transmittal.Files.Add(new TransmittedFile() { FileId = file.FileId });
            }
            SubscriberItem subscriberItem = new SubscriberCompany() { CompanyId = company.CompanyId };
            ITransmittalService transmittalService = ServiceFactory.GetTransmittalService(customerId);

            var serviceStatus = transmittalService.CreateTransmittal(transmittal, new List<SubscriberItem>() { subscriberItem }, null);

            //Try and update the transmittal
            long removedFileId = transmittal.Files[0].FileId;
            transmittal.Files.RemoveAt(0);
            SubscriberItem userSubscriberItem = new SubscriberUser() {UserId = company.Users.FirstOrDefault().UserId};
            
            serviceStatus = transmittalService.CreateTransmittal(transmittal,
                                                                 new List<SubscriberItem>() {userSubscriberItem}, null);

            Assert.IsTrue(serviceStatus.Status== TransmittalServiceStatus.Success, "Status of success was expected");

            int count = this.GetCount("TransmittalDistribution");
            Assert.IsTrue(count == 1, "Only one Distribution was expected after update");
            object userId = this.GetValue("TransmittalDistribution", "UserId");
            Assert.IsTrue(int.Parse(userId.ToString()) == ((SubscriberUser)userSubscriberItem).UserId,
                          "UserId was unaccepteable");
            count = this.GetCount("TransmittedFiles");
            Assert.IsTrue(count == 31, "31 files were accepted");
            count = this.GetCount("TransmittedFiles", "WHERE FileId=" + removedFileId);
            Assert.IsTrue(count == 0, "Expected file was deleted");
        }

        [TestMethod]
        public void Verify_Get_Transmittal()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            long projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, (int)projectId, 0, "Test");
            long transmittalId = CreateTransmittal(customerId, projectId, folderId);

            ITransmittalService transmittalService = ServiceFactory.GetTransmittalService(customerId);
            Transmittal transmittal = transmittalService.GetTransmittal(projectId, transmittalId);

            //Assert.IsTrue(transmittal.Subject == null, "Transmittal should have been null");
            transmittal = transmittalService.GetTransmittal(projectId, transmittalId);
            Assert.IsTrue(transmittal != null, "Transmittal  is null");
            
            Assert.IsTrue(transmittal.Distribution.Count == 5);
            Assert.IsTrue(!transmittal.Files.OfType<TransmittedFileVersion>().Any());

            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            File f =
                storageService.GetPreUploadInfo(projectId, folderId, new[] { transmittal.Files.FirstOrDefault().FileName }, false,
                                                false).First();
            f.Project.ProjectId = projectId;
            f.Folder.FolderId = folderId;
            f.CustomerId = customerId;
            storageService.AddFile(f, true, "newVersionPath");

            transmittal = transmittalService.GetTransmittal(projectId, transmittalId);
            Assert.IsTrue(transmittal.Files.OfType<TransmittedFileVersion>().Count() == 1);
        }

        [TestMethod]
        public void Verify_Basic_DistributionExtraction()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            long projectId = this.AddProject(customerId);
            Company company = SubscribeCompanies(projectId);
            SubscriberItem subscriberItem = new SubscriberCompany() {CompanyId = company.CompanyId};
            ICustomerSubscriptionService customerSubscription = ServiceFactory.GetCustomerSubscriptionService(customerId);
            IEnumerable<Company> companies = customerSubscription.GetSubscribedCompanies(projectId);
            var acme = (from c in companies
                       where c.CompanyName != "AcmeCompany"
                       select c).FirstOrDefault();
            SubscriberItem  userCCItem = new SubscriberUser() {UserId = acme.Users.First().UserId};
            var noAcme = (from c in companies
                        where c.CompanyId != acme.CompanyId
                        select c).FirstOrDefault();
            SubscriberItem dupUserItem = new SubscriberUser() {UserId = noAcme.Users.First().UserId};
            DistributionExtractor extractor = new DistributionExtractor(customerId, projectId,
                                                                        new List<SubscriberItem>()
                                                                            {
                                                                                subscriberItem,
                                                                                dupUserItem
                                                                            },
                                                                        new List<SubscriberItem>() {userCCItem});

            List<TransmittalUser> distribution = extractor.Extract();
            Assert.IsTrue(distribution.Count == 6, "6 TransmittalUsers where excepted");
            var cced = from d in distribution
                       where d.IsCced 
                       select d;

            Assert.IsTrue(cced.Count() == 1, "One CCed user was accepted");
        }

        private long CreateTransmittal(long customerId, long projectId, int folderId)
        {
            IEnumerable<File> files = AddFiles(projectId, folderId, customerId);
            Company company = SubscribeCompanies(projectId);
            Transmittal transmittal = new Transmittal
            {
                Files = new List<TransmittedFile>(),
                Distribution = new List<TransmittalUser>(),
                CreatedBy = new User() { UserName = AdminUserName },
                Message = "Message",
                TransmittalNumber = "1",
                Subject = "fubar",
                ProjectId = projectId
            };

            foreach (File file in files)
            {
                transmittal.Files.Add(new TransmittedFile() { FileId = file.FileId });
            }
            SubscriberItem subscriberItem = new SubscriberCompany() { CompanyId = company.CompanyId };
            ITransmittalService transmittalService = ServiceFactory.GetTransmittalService(customerId);

            IProjectService projectService = ServiceFactory.GetProjectService(customerId);
            IEnumerable<Status> status = projectService.GetProjectStatuses(projectId);
            transmittal.TransmittalStatus = status.Where(x => x.StatusId == 1).FirstOrDefault();
            transmittalService.CreateTransmittal(transmittal, new List<SubscriberItem>() { subscriberItem }, null);

            
            return transmittal.TransmittalId;

        }

        private IEnumerable<File> AddFiles(long projectId, long folderId, long customerId)
        {
            List<string> fileNames = new List<string>();

            using (StreamReader reader = new StreamReader(@"..\..\..\Docller.UnitTests\FileList.txt"))
            {
                while (!reader.EndOfStream)
                {
                    fileNames.Add(reader.ReadLine());
                }
            }
            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            IEnumerable<Core.Models.File> files = storageService.GetPreUploadInfo(projectId, folderId, fileNames.ToArray(), false, false);

            foreach (Core.Models.File file in files)
            {
                file.Project.ProjectId = projectId;
                file.Folder.FolderId = folderId;
                file.CustomerId = customerId;
                storageService.AddFile(file, true, null);
            }
            return files;
        }
    }
}

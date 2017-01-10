using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Common.DataStructures;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Services;
using Docller.Core.Storage;
using Docller.Serialization;
using Docller.Tests;
using Docller.Tests.Mocks;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using StructureMap;
using File = Docller.Core.Models.File;

namespace Docller.UnitTests
{
    [TestClass]
    public class StorageServiceFixture : FixtureBase
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

        #region FileUpload Unit Test
        [TestMethod]
        public void Verify_GetFilesWithAttachments()
        {
            //Get the test filenames
            List<string> fileNames = new List<string>();
            using(StreamReader reader = new StreamReader(@"..\..\..\Docller.UnitTests\FileList.txt"))
            {
                while(!reader.EndOfStream)
                {
                    fileNames.Add(reader.ReadLine());
                    
                }
            }
            IStorageService storageService = new MockStorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance < IBlobStorageProvider>());
            IEnumerable<Core.Models.File> fileResults = storageService.GetPreUploadInfo(0,0,fileNames.ToArray(), false, false);

            Assert.IsTrue(fileResults.Count() == 32, "Getting Files without attachments failed");

            fileResults = storageService.GetPreUploadInfo(0, 0, fileNames.ToArray(), true, false);
            Assert.IsTrue(fileResults.Count() == 20, "20 Files where expected");

            var filesWithAttachments = from f in fileResults
                                       where f.Attachments != null
                                       select f;
            Assert.IsTrue(filesWithAttachments.Count() == 12, "Exactly 12 files with attachment were expected");

            foreach (var file in filesWithAttachments)
            {
                FileInfo fileInfo = new FileInfo(file.FileName);
                FileInfo attachmentInfo = new FileInfo(file.Attachments[0].FileName);
                Assert.IsTrue(
                    fileInfo.Name.Replace(fileInfo.Extension, string.Empty).Equals(
                        attachmentInfo.Name.Replace(attachmentInfo.Extension, string.Empty),
                        StringComparison.InvariantCultureIgnoreCase),"File names did not match");

            }
        }

        [TestMethod]
        public void Verify_GetPreUploadInfo()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            
            int projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, projectId, 0, "Test");
            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            //Get the test filenames
            List<string> fileNames = new List<string>();
            using (StreamReader reader = new StreamReader(@"..\..\..\Docller.UnitTests\FileList.txt"))
            {
                while (!reader.EndOfStream)
                {
                    fileNames.Add(reader.ReadLine());
                    
                }
            }
            IEnumerable<Core.Models.File> files = storageService.GetPreUploadInfo(projectId, folderId, fileNames.ToArray(), false, false);
            
            Assert.IsTrue(files.Count() == 32, "Getting Files without attachments failed");

            //make sure we don't get anyy attachments
            var noAttachments = from f in files
                                where f.Attachments == null || f.Attachments.Count == 0
                                select f;
            Assert.IsTrue(noAttachments.Count() == 32, "No Attachment was expcted");

            files = storageService.GetPreUploadInfo(projectId, folderId, fileNames.ToArray(), true, false);
            Assert.IsTrue(files.Count() == 20, "20 Files where expected");

            var filesWithAttachments = from f in files
                                       where f.Attachments != null && f.Attachments.Count > 0
                                       select f;
            Assert.IsTrue(filesWithAttachments.Count() == 12, "Exactly 12 files with attachment were expected");

            foreach (var filesWithAttachment in filesWithAttachments)
            {
                Core.Models.File attachment = filesWithAttachment;
                var attachmednts = from fa in filesWithAttachment.Attachments
                                   where fa.ParentFile != attachment.FileInternalName
                                   select fa;

                Assert.IsTrue(attachmednts.Count() == 0,"Found an attachment whose ParentFile is not same as FileInternalName");
            }
        }

        [TestMethod]
        public void Verify_AddingFilesAndAttachments_With_No_Revision_Extraction()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, projectId, 0, "Test");
            int companyId = Convert.ToInt32(this.GetValue("Companies", "CompanyId"));
            
            Database db = GetDb();
            db.ExecuteNonQuery(CommandType.Text,
                               string.Format(
                                   @"UPDATE Companies SET RevisionRegEx='\s*-\s*REV\s*-\s*(?<revision>[a-zA-Z0-9]*)$' WHERE CompanyId={0}",
                                   companyId));
            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            string[] fileNames = { "63-WHR-001-03-REV-A.pdf", "63-WHR-001-03-REV-A.dwg" };
            IEnumerable<Core.Models.File> files = storageService.GetPreUploadInfo(projectId, folderId, fileNames, true, false);
            foreach (Core.Models.File file in files)
            {
                file.Project.ProjectId = projectId;
                file.Folder.FolderId = folderId;
                file.CustomerId = customerId;
                file.ContentType = "";
                storageService.AddFile(file, true, null);

                //Mae sure base file name is same as filename
                string baseFileName = this.GetValue("Files", "BaseFileName").ToString();
                Assert.IsTrue(baseFileName.Equals(file.FileName,StringComparison.InvariantCultureIgnoreCase), "Base Filename was expected to be same as filename");
                if (file.Attachments != null)
                {
                    foreach (FileAttachment attachment in file.Attachments)
                    {
                        attachment.Project.ProjectId = projectId;
                        attachment.Folder.FolderId = folderId;
                        attachment.ParentFile = file.FileInternalName;
                        storageService.AddAttachment(attachment, true, null);
                        baseFileName = this.GetValue("FileAttachments", "BaseFileName").ToString();
                        Assert.IsTrue(baseFileName.Equals(attachment.FileName, StringComparison.InvariantCultureIgnoreCase), "Base Filename was expected to be same as filename");
                    }
                }
            }

        }

        [TestMethod]
        public void Verify_AddingFilesAndAttachments_With_Revision_In_FileName()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, projectId, 0, "Test");
            int companyId = Convert.ToInt32(this.GetValue("Companies", "CompanyId"));
            
            Database db = GetDb();
            db.ExecuteNonQuery(CommandType.Text,
                               string.Format(
                                   @"UPDATE Companies SET RevisionRegEx='\s*-\s*REV\s*-\s*(?<revision>[a-zA-Z0-9]*)$' WHERE CompanyId={0}",
                                   companyId));
            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            string[] fileNames = { "63-WHR-001-03-REV-A.pdf", "63-WHR-001-03-REV-A.dwg" };
            IEnumerable<Core.Models.File> files = storageService.GetPreUploadInfo(projectId, folderId, fileNames, true, true);
            foreach (Core.Models.File file in files)
            {
                file.Project.ProjectId = projectId;
                file.Folder.FolderId = folderId;
                file.CustomerId = customerId;
                storageService.AddFile(file, true, null);
                if(file.Attachments != null)
                {
                    foreach (FileAttachment attachment in file.Attachments)
                    {
                        attachment.Project.ProjectId = projectId;
                        attachment.Folder.FolderId = folderId;
                        attachment.ParentFile = file.FileInternalName;
                        storageService.AddAttachment(attachment, true, null);
                    }
                }
            }
            int count = this.GetCount("Files");
            Assert.IsTrue(count == 1, "Only one file was expected");
            string[] versions = {"B", "C", "D", "E", "F", "G"};

            for(int index = 0; index < versions.Length; index++)
            {
                string version = versions[index];
                string[] newVersionfileName = { "63-WHR-001-03-REV- A.pdf".Replace("A", version), "63-WHR-001-03-REV- A.dwg".Replace("A", version) };
                files = storageService.GetPreUploadInfo(projectId, folderId, newVersionfileName, true, true );
                foreach (Core.Models.File file in files)
                {
                    file.Project.ProjectId = projectId;
                    file.Folder.FolderId = folderId;
                    file.CustomerId = customerId;
                    storageService.AddFile(file, true, "fubar");
                    int rev = Convert.ToInt32(this.GetValue("Files", "CurrentRevision",
                                                  string.Format("WHERE FileInternalName = '{0}'", file.FileInternalName)));
                    Assert.IsTrue(rev == index + 2, "Revision for FileName" + file.FileName +  " did not have currect revision number");

                    if (file.Attachments != null)
                    {
                        foreach (FileAttachment attachment in file.Attachments)
                        {
                            attachment.Project.ProjectId = projectId;
                            attachment.Folder.FolderId = folderId;
                            attachment.ParentFile = file.FileInternalName;
                            StorageServiceStatus status = storageService.AddAttachment(attachment, true, null);
                            Assert.IsTrue(status == StorageServiceStatus.VersionPathNull, "VersionPathNull was expected");
                            storageService.AddAttachment(attachment, true, "fubar");

                            rev = Convert.ToInt32(this.GetValue("FileAttachments", "RevisionNumber",
                                                  string.Format("WHERE FileId = {0} ORDER BY CreatedDate DESC", file.FileId)));
                            Assert.IsTrue(rev == index + 2, "Revision for FileName" + file.FileName + " did not have currect revision number");
                        }
                    }

                }
            }
        }

        [TestMethod]
        public void Verify_AddingFiles_With_NoAttachments_And_SameNames()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, projectId, 0, "Test");
            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            //Get the test filenames
            List<string> fileNames = new List<string>();
            using (StreamReader reader = new StreamReader(@"..\..\..\Docller.UnitTests\FileList.txt"))
            {
                while (!reader.EndOfStream)
                {
                    fileNames.Add(reader.ReadLine());
                }
            }
            IEnumerable<Core.Models.File> files = storageService.GetPreUploadInfo(projectId, folderId, fileNames.ToArray(), false, true);

            foreach (Core.Models.File file in files)
            {
                file.Project.ProjectId = projectId;
                file.Folder.FolderId = folderId;
                file.CustomerId = customerId;
                file.ContentType = "fubar";
                storageService.AddFile(file, true, null);
            }
            int count = this.GetCount("Files");
            Assert.IsTrue(count == 32, "Exactly 32 files were expected to be added to the DB");
            files = storageService.GetPreUploadInfo(projectId, folderId, fileNames.ToArray(), false, true);
            foreach (Core.Models.File file in files)
            {
                file.Project.ProjectId = projectId;
                file.Folder.FolderId = folderId;
                file.CustomerId = customerId;
                //Try adding the file with the same name but version as false
                file.ContentType = "fubar";
                StorageServiceStatus status = storageService.AddFile(file, false, null);
                Assert.IsTrue(status==StorageServiceStatus.ExistingFile,"Status was expected to be Exisiting file");
                storageService.AddFile(file, true, "fubar");
            }

            Assert.IsTrue(count == 32, "Exactly 32 files were expected to be added to the DB");

            foreach (Core.Models.File file in files)
            {
                int rev =
                    Convert.ToInt32(this.GetValue("Files", "CurrentRevision",
                                                  string.Format("WHERE FileInternalName = '{0}'", file.FileInternalName)));

                Assert.IsTrue(rev == 2, string.Format("CurrentRevision for {0} is not 2",file.FileName));
            }
            
        }

        [TestMethod]
        public void Verify_AddingFiles_And_Attachments()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, projectId, 0, "Test");
            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            //Get the test filenames
            List<string> fileNames = new List<string>();
            using (StreamReader reader = new StreamReader(@"..\..\..\Docller.UnitTests\FileList.txt"))
            {
                while (!reader.EndOfStream)
                {
                    fileNames.Add(reader.ReadLine());
                }
            }
            IEnumerable<Core.Models.File> files = storageService.GetPreUploadInfo(projectId, folderId, fileNames.ToArray(), true, false);

            foreach (Core.Models.File file in files)
            {
                file.Project.ProjectId = projectId;
                file.Folder.FolderId = folderId;
                file.CustomerId = customerId;
                file.ContentType = "fubar";
                storageService.AddFile(file, true, null);
                if(file.Attachments != null)
                {
                    foreach (FileAttachment attachment in file.Attachments)
                    {
                        Assert.IsFalse(attachment.ParentFile.Equals(Guid.Empty), "ParentFile is null in " + attachment.FileName);
                        attachment.Project.ProjectId = projectId;
                        attachment.Folder.FolderId = folderId;
                        attachment.ParentFile = file.FileInternalName;
                        attachment.ContentType = "fubar";
                        storageService.AddAttachment(attachment,true,null);

                        int revision =
                            Convert.ToInt32(this.GetValue("FileAttachments", "RevisionNumber",
                                                          string.Format("WHERE FileId={0} ORDER BY CreatedDate DESC", file.FileId)));
                        Assert.IsTrue(revision == 1, "Number for Attachments were expected to be 1");
                        
                        //Add a Version of the Same File
                        storageService.AddFile(file, true, "fubar");

                        //Add another version of the attachment
                        StorageServiceStatus storageServiceStatus = storageService.AddAttachment(attachment, false, null);
                        Assert.IsTrue(storageServiceStatus == StorageServiceStatus.ExistingFile,"Status was expected to be existing file");
                        
                        storageService.AddAttachment(attachment, true, "Fubar");

                        int count = this.GetCount("FileAttachments",
                                                  string.Format("WHERE FileId={0}", file.FileId));
                        
                        Assert.IsTrue(count == 2, "Two attachments were expected");
                        revision =
                            Convert.ToInt32(this.GetValue("FileAttachments", "RevisionNumber",
                                                          string.Format("WHERE FileId={0} ORDER BY CreatedDate DESC", file.FileId)));

                        Assert.IsTrue(revision == 2, "Number for Attachments were expected to be 2 for " + attachment.FileName);
                        
                    }
                }
            }
        }

        [TestMethod]
        public void Verify_Add_File_DocNumber()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, projectId, 0, "Test");
            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            //Get the test filenames
            List<string> fileNames = new List<string>();
            using (StreamReader reader = new StreamReader(@"..\..\..\Docller.UnitTests\FileList.txt"))
            {
                while (!reader.EndOfStream)
                {
                    fileNames.Add(reader.ReadLine());
                    break;
                }
            }
            Core.Models.File fileToAdd = storageService.GetPreUploadInfo(projectId, folderId, fileNames.ToArray(), true, false).First();

            fileToAdd.Project.ProjectId = projectId;
            fileToAdd.Folder.FolderId = folderId;
            fileToAdd.CustomerId = customerId;
            fileToAdd.ContentType = "fubar";
            storageService.AddFile(fileToAdd, true, null);

            string documber = this.GetValue("Files", "DocNumber").ToString();
            FileInfo fileInfo = new FileInfo(fileToAdd.FileName);
            string expectedDocNum = fileToAdd.BaseFileName.Replace(fileInfo.Extension, string.Empty);
            Assert.IsTrue(documber.Equals(expectedDocNum, StringComparison.InvariantCultureIgnoreCase), "DocNUmber is not what was expected");

        }




        #endregion

                

        #region Folders Unit Test
        [TestMethod]
        public void Verify_TreeStructure()
        {
            Tree<Folder> tree = new Tree<Folder>(new Folder() { FolderInternalName = "Root" });
            tree.AddChild(new Folder() { FolderInternalName = "Architecture" });
            tree.AddChild(new Folder() { FolderInternalName = "Services" });
            tree.AddChild(new Folder() { FolderInternalName = "Submittals" });


            Tree<Folder> architecture = tree.GetChild(1);
            architecture.AddChild(new Folder() { FolderInternalName = "HV" });
            architecture.AddChild(new Folder() { FolderInternalName = "Saousorus" });

            // Tree<Folder>.Traverse(tree, new Action<Folder>(OnAction));
        }
        
       
        [TestMethod]
        public void Verify_AddingFolder()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = this.AddProject(customerId);

            IStorageService storageService = ServiceFactory.GetStorageService(customerId);

            List<Folder> folders = new List<Folder>();

            for (int i = 0; i < 10; i++)
            {
                folders.Add(new Folder() { FolderName = "Folder" + i });
            }

            FolderCreationStatus creationStatus = storageService.CreateFolders(AdminUserName, projectId, folders);

            Assert.IsTrue(creationStatus.Status == StorageServiceStatus.Success, "Should have been success");
            Assert.IsTrue(creationStatus.DuplicateFolders == null || creationStatus.DuplicateFolders.Count() == 0, "Should not have duplicate folders");

            int folderCount = this.GetCount("Folders");
            Assert.IsTrue(folderCount == 10, "10 Folders were expected");
            int permissionsCount = this.GetCount("CompanyFolderPermissions");
            Assert.IsTrue(permissionsCount == 10, "10 Folder Permissions were expected");

            //Get Id for a folder
            int parentFolderId = Convert.ToInt32(this.GetValue("Folders", "FolderId", "WHERE FolderName = 'Folder0'"));

            //Check the full path
            string fullPath =
                this.GetValue("Folders", "FullPath",
                              string.Format("Where FolderId = {0}", parentFolderId)).ToString();
            Assert.IsTrue(fullPath.Equals("f-" + parentFolderId, StringComparison.CurrentCultureIgnoreCase),
                          "Folder Full path should be same as Folder internal name");


            List<Folder> childFolders = new List<Folder>() { new Folder() { FolderName = "ChildFolder", FullPath = "FullPath" } };

            storageService.CreateFolders(AdminUserName, projectId, parentFolderId, childFolders);
            int childFolderCount = this.GetCount("Folders", "WHERE ParentFolderId = " + parentFolderId);
            Assert.IsTrue(childFolderCount == 1);

            permissionsCount = this.GetCount("CompanyFolderPermissions");
            Assert.IsTrue(permissionsCount == 11, "11 Folder Permissions were expected");
            
            //check the full path of the child folder
            int childFolderId = Convert.ToInt32(this.GetValue("Folders", "FolderId", "WHERE FolderName='ChildFolder'"));

            string expectedFullPath = string.Format("{0}{1}f-{2}", fullPath,
                                                    Config.GetValue<string>(ConfigKeys.AzureFolderPathSeperator),
                                                    childFolderId);
            fullPath =
               this.GetValue("Folders", "FullPath",
                             string.Format("Where FolderId = {0}", childFolderId)).ToString();
            Assert.IsTrue(expectedFullPath.Equals(fullPath, StringComparison.CurrentCultureIgnoreCase),
                          "Folder Full path is not what was expected");

            //Try adding all of them as duplicate at root level
            foreach (Folder folder in folders)
            {
                folder.FolderId = 0;
            }
            creationStatus = storageService.CreateFolders(AdminUserName, projectId, folders);
            Assert.IsTrue(creationStatus.Status == StorageServiceStatus.ExistingFolder, "Should have been Duplicate folders");
            Assert.IsTrue(creationStatus.DuplicateFolders != null && creationStatus.DuplicateFolders.Count() == 10);

            //Just ensure we still only have 10 top level folders!
            folderCount = this.GetCount("Folders", "WHERE ParentFolderId IS NULL");
            Assert.IsTrue(folderCount == 10, "10 Folders were expected");

            //Now try and insert the same child folder into the same parent
            foreach (Folder childFolder in childFolders)
            {
                childFolder.FolderId = 0;
            }
            creationStatus = storageService.CreateFolders(AdminUserName, projectId, parentFolderId, childFolders);
            Assert.IsTrue(creationStatus.Status == StorageServiceStatus.ExistingFolder);
            Assert.IsTrue(creationStatus.DuplicateFolders.Count == 1);

            //Now try and insert the same child folder into some other parent
            parentFolderId = Convert.ToInt32(this.GetValue("Folders", "FolderId", "WHERE FolderName = 'Folder1'"));
            foreach (Folder childFolder in childFolders)
            {
                childFolder.FolderId = 0;
            }
            creationStatus = storageService.CreateFolders(AdminUserName, projectId, parentFolderId, childFolders);
            Assert.IsTrue(creationStatus.Status == StorageServiceStatus.Success);
            Assert.IsTrue(creationStatus.DuplicateFolders.Count == 0);

            folderCount = this.GetCount("Folders", "WHERE ParentFolderId = " + parentFolderId);
            Assert.IsTrue(folderCount == 1, "1 Folder was expected");
        }

        [TestMethod]
        public void Verify_RenameFolder()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int testProjectId = this.AddProject(customerId);

            IStorageService storageService = ServiceFactory.GetStorageService(customerId);

            // Test For Success
            int testfolderId = this.AddFolder(customerId, testProjectId, 0, "TestFolder");
            StorageServiceStatus serviceStatus = storageService.RenameFolder(new Folder { ProjectId = testProjectId, ParentFolderId = 0, FolderId = testfolderId, FolderName = "RenameTest" });
            Assert.IsTrue(serviceStatus == StorageServiceStatus.Success, "Status should have been success");

            // Test for Parent Folder
            int testChildFolderId = this.AddFolder(customerId, testProjectId, testfolderId, "ParentTestFolder");
            serviceStatus = storageService.RenameFolder(new Folder { ProjectId = testProjectId, ParentFolderId = testfolderId, FolderId = testChildFolderId, FolderName = "RenameChildFolderTest" });
            Assert.IsTrue(serviceStatus == StorageServiceStatus.Success, "Status should have been success");

            // Rename Folder Test For Existing Folder
            int folderId1 = this.AddFolder(customerId, testProjectId, 0, "Folder1");
            this.AddFolder(customerId, testProjectId, 0, "Folder2");
            serviceStatus = storageService.RenameFolder(new Folder { ProjectId = testProjectId, ParentFolderId = 0, FolderId = folderId1, FolderName = "Folder2" });
            Assert.IsTrue(serviceStatus == StorageServiceStatus.ExistingFolder, "Status should have been ExistingFolder");

            // Rename Folder Test of Existing Folder For Parent Folder
            int childfolderId1 = this.AddFolder(customerId, testProjectId, testfolderId, "ChildFolder1");
            this.AddFolder(customerId, testProjectId, testfolderId, "ChildFolder2");
            serviceStatus = storageService.RenameFolder(new Folder { ProjectId = testProjectId, ParentFolderId = testfolderId, FolderId = childfolderId1, FolderName = "ChildFolder2" });
            Assert.IsTrue(serviceStatus == StorageServiceStatus.ExistingFolder, "Status should have been ExistingFolder");
        }

        [TestMethod]
        public void Verify_GetDefaultFolders()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            long testProjectId = this.AddProjectViaService(customerId);

            IStorageService storageService = ServiceFactory.GetStorageService(customerId);

           Folders folders = storageService.GetFolders(AdminUserName, testProjectId);

           Assert.IsTrue(folders.Count == 11, "11 Folders were expected");

        }

        [TestMethod]
        public void Verify_FolderStructureAncestor()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            long testProjectId = this.AddProjectViaService(customerId);
            long folderId = Convert.ToInt64(this.GetValue("Folders", "FolderId", "WHERE FolderName='Structural'"));
            int childfolderId = this.AddFolder(customerId, (int)testProjectId, (int)folderId, "ChildFolder");
            int grandChild = this.AddFolder(customerId, (int)testProjectId, childfolderId,"GrandChildFolder");
            int greatGrandChild = AddFolder(customerId, (int)testProjectId, grandChild,"GreatGrandChildFolder");

            IStorageService storageService = ServiceFactory.GetStorageService(customerId);

            Folders folders = storageService.GetFolders(AdminUserName, testProjectId);

            Assert.IsTrue(folders.IsAncestor(greatGrandChild, grandChild), "GrandChildFolder was expected to be a an ancestor");
            Assert.IsTrue(folders.IsAncestor(greatGrandChild, childfolderId), "ChildFolder was expected to be a an ancestor");
            Assert.IsTrue(folders.IsAncestor(greatGrandChild, folderId), "Structural Folder was expected to be a an ancestor");

            long anotherFolderId = Convert.ToInt64(this.GetValue("Folders", "FolderId", "WHERE FolderName='MEP'"));
            Assert.IsFalse(folders.IsAncestor(greatGrandChild, anotherFolderId), "MEPS Folder was not expected to be a an ancestor");
        }

        [TestMethod]
        public void Verify_GetDefaulFoldersWithChildren()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            long testProjectId = this.AddProjectViaService(customerId);

            IStorageService storageService = ServiceFactory.GetStorageService(customerId);

            Folders folders = storageService.GetFolders(AdminUserName, testProjectId);
            
            foreach (Folder folder in folders)
            {
                for(int i =0;i<10;i++)
                {
                  int childfolderId =  this.AddFolder(customerId, (int) testProjectId, (int) folder.FolderId,
                                   string.Format("ChildFolder_{0}", i));

                    for(int j=0;j<5;j++)
                    {
                        int grandChild = this.AddFolder(customerId, (int)testProjectId, childfolderId,
                                                         string.Format("GrandChildFolder_{0}", j));
                        for(int k=0;k<2;k++)
                        {
                            AddFolder(customerId, (int)testProjectId, grandChild,
                                                         string.Format("GreatGrandChildFolder_{0}", k));
                        }
                    }
                }
                
            }

            folders = storageService.GetFolders(AdminUserName, testProjectId);
            Assert.IsTrue(folders.Count() == 11);

            foreach (Folder folder in folders)
            {
                Assert.IsTrue(folder.AllParents.Count == 0, "No parent folders were expected");
                Assert.IsTrue(folder.SubFolders.Count == 10, "Exactly 10 child folders were expected");
                IEnumerable<Folder> childFolders = folder.SubFolders;
                foreach (Folder childFolder in childFolders)
                {
                    Assert.IsTrue(childFolder.SubFolders.Count() == 5, "Exactly 5 Grand child were expected");
                    Assert.IsTrue(childFolder.AllParents.Count == 1, "One parent folders were expected");
                    IEnumerable<Folder> grandKids = childFolder.SubFolders;
                    foreach (Folder grandKid in grandKids)
                    {
                        Assert.IsTrue(grandKid.SubFolders.Count() == 2, "Exactly 2 Great Grand child were expected");
                        Assert.IsTrue(grandKid.AllParents.Count == 2, "two parent folders were expected");
                        IEnumerable<Folder> greatGrandKids = grandKid.SubFolders;
                        foreach (Folder greatGrandKid in greatGrandKids)
                        {
                            Assert.IsTrue(greatGrandKid.AllParents.Count == 3, "3 parent folders were expected");    
                        }
                       
                    }
                }

            }

        }


        #endregion


        #region File Edit Test
        [TestMethod]
        public void Verify_Get_FilesToEditUpload()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            
            int projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, projectId, 0, "Test");
            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            //Get the test filenames
            List<string> fileNames = new List<string>();
            using (StreamReader reader = new StreamReader(@"..\..\..\Docller.UnitTests\FileList.txt"))
            {
                while (!reader.EndOfStream)
                {
                    fileNames.Add(reader.ReadLine());
                }
            }
            IEnumerable<Core.Models.File> files = storageService.GetPreUploadInfo(projectId, folderId, fileNames.ToArray(), true, false);
            int attachmentCounts=0;
            foreach (File file in files)
            {
                file.Project.ProjectId = projectId;
                file.Folder.FolderId = folderId;
                file.CustomerId = customerId;
                file.ContentType = "fubar";
                storageService.AddFile(file, true, null);
                if (file.Attachments != null && file.Attachments.Count == 1)
                {
                    storageService.AddAttachment(file.Attachments[0], false, null);
                    attachmentCounts++;
                }
            }

            List<File> filesToSend =
                files.Select(file => new File() {FileInternalName = file.FileInternalName}).ToList();

            IEnumerable<File> filesToEdit = storageService.GetFilesForEdit(new List<File>(filesToSend));
            Assert.IsTrue(filesToEdit.Count() == 20, "20 Files were expected");
            var attacmnents = from a in filesToEdit
                              where a.Attachments != null && a.Attachments.Count() > 0
                              select a;
            Assert.IsTrue(attacmnents.Count() == attachmentCounts, "Attachment counts result is unexpected");

            
        }

        [TestMethod]
        public void Verify_Update_Files()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);

            int projectId = this.AddProject(customerId);
            int folderId = this.AddFolder(customerId, projectId, 0, "Test");
            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            //Get the test filenames
            List<string> fileNames = new List<string>();
            using (StreamReader reader = new StreamReader(@"..\..\..\Docller.UnitTests\FileList.txt"))
            {
                while (!reader.EndOfStream)
                {
                    fileNames.Add(reader.ReadLine());
                }
            }
            IEnumerable<Core.Models.File> files = storageService.GetPreUploadInfo(projectId, folderId, fileNames.ToArray(), false, false);

            foreach (File file in files)
            {
                file.Project.ProjectId = projectId;
                file.Folder.FolderId = folderId;
                file.CustomerId = customerId;
                file.ContentType = "fubar";
                storageService.AddFile(file, true, null);
            }

            List<File> filesToSend =
                files.Select(file => new File() { FileInternalName = file.FileInternalName }).ToList();

            IEnumerable<File> filesToEdit = storageService.GetFilesForEdit(new List<File>(filesToSend));

            IProjectService projectService = ServiceFactory.GetProjectService(customerId);
            Status[] allStatus = projectService.GetProjectStatuses(projectId).ToArray();
            Random random = new Random();
            filesToEdit.First().FileName = "New FileName 123";
            
            foreach (File file in filesToEdit)
            {
                file.StatusId = allStatus[random.Next(0, 7)].StatusId;
            }
            IEnumerable<File> dupFiles;
            StorageServiceStatus status = storageService.TryUpdateFiles(filesToEdit.ToList(), out dupFiles);

            Assert.IsTrue(status == StorageServiceStatus.Success, "Update status of success  was expected");
            Assert.IsTrue(dupFiles == null || dupFiles.Count() == 0, "No duplicate files were expected");
            File firstFile = filesToEdit.First();
            object fileName = this.GetValue("Files", "FileName", string.Format("WHERE FileId={0}", firstFile.FileId));
            Assert.IsTrue(fileName.ToString().Equals(string.Concat("New FileName 123", firstFile.FileExtension)), "FileName of New FileName 123 was expected");

            object baseFileName = this.GetValue("Files", "BaseFileName", string.Format("WHERE FileId={0}", firstFile.FileId));

            Assert.IsTrue(baseFileName.ToString().Equals(string.Concat("New FileName 123", firstFile.FileExtension)), "Basefile of New FileName 123 was expected");

            //Check with duplicate files names
            File dupFile = filesToEdit.ToList()[1];
            dupFile.FileName = "New FileName 123.dwg";
            status = storageService.TryUpdateFiles(filesToEdit.ToList(), out dupFiles);

            Assert.IsTrue(status == StorageServiceStatus.ExistingFile, "Update status of exisiting file  was expected");
            Assert.IsTrue(dupFiles == null || dupFiles.Count() == 1, "No duplicate files were expected");
            Assert.IsTrue(dupFiles.First().FileInternalName == dupFile.FileInternalName, "Duplicate FileId was not correct");


        }
#endregion

        #region GetFiles
        [TestMethod]
        public void Verify_GetFiles()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = AddProject(customerId);
            int folderId = AddFolder(customerId, projectId, 0, "Test");

            AddFiles(customerId,projectId,folderId, null);
            AddFiles(customerId, projectId, folderId, 1);

            IStorageService storageService = ServiceFactory.GetStorageService(customerId);
            Files files = storageService.GetFiles(projectId, folderId, AdminUserName, FileSortBy.Date, SortDirection.Descending, 1, 10);

            Assert.IsTrue(files.Count == 10, "10 files were expected");
            Assert.IsTrue(files.TotalCount == 20, "Total 20 files were expected");
            files = storageService.GetFiles(projectId, folderId, AdminUserName, FileSortBy.Date, SortDirection.Descending, 1, files.TotalCount);
            
            Assert.IsTrue(files.Count == 20, "Total 20 files were expected");
            var withVersion = from v in files
                              where v.HasVersions
                              select v;
            Assert.IsTrue(withVersion.Count() == 1, "Only one file with version was expected");

            //check for attachments

            var withAttachments = from a in files
                                  where a.Attachments.Count == 1
                                  select a;

            Assert.IsTrue(withAttachments.Count() == 12, "12 Files with Attachments were expected");

        }

        [TestMethod]
        public void Verify_GetFiles_Sorting()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId, AdminUserName);
            int projectId = AddProject(customerId);
            int folderId = AddFolder(customerId, projectId, 0, "Test");

            AddFiles(customerId, projectId, folderId, null);
            
            IStorageService storageService = ServiceFactory.GetStorageService(customerId);
            Files files = storageService.GetFiles(projectId, folderId, AdminUserName, FileSortBy.Date, SortDirection.Ascending, 1, 10);

            
            //test out sorting by date desc
            long fileId = long.Parse(this.GetValue("Files", "FileId", "ORDER BY CreatedDate ASC").ToString());

            Assert.IsTrue(files.FirstOrDefault().FileId == fileId, "Sort By Date ASC, does not seem to be right");


            files = storageService.GetFiles(projectId, folderId, AdminUserName, FileSortBy.FileName, SortDirection.Descending, 1, 10);
            fileId = long.Parse(this.GetValue("Files", "FileId", "ORDER BY FileName DESC").ToString());
            Assert.IsTrue(files.FirstOrDefault().FileId == fileId, "Sort By FileName DESC does not seem to be right");

            files = storageService.GetFiles(projectId, folderId, AdminUserName, FileSortBy.FileName, SortDirection.Ascending, 1, 10);
            fileId = long.Parse(this.GetValue("Files", "FileId", "ORDER BY FileName ASC").ToString());
            Assert.IsTrue(files.FirstOrDefault().FileId == fileId, "Sort By FileName ASC does not seem to be right");
        }


        #endregion

        #region Misc
        [TestMethod]
        public void Verify_CompanyPref()
        {
            long customerId = this.AddCustomer();
            SetDocllerContext(customerId,AdminUserName);
            int companyId = Convert.ToInt32(this.GetValue("Companies", "CompanyId"));

            Database db = GetDb();
            db.ExecuteNonQuery(CommandType.Text,
                               string.Format(
                                   @"UPDATE Companies SET RevisionRegEx='RegEx', AttributeMappingsXml='TestXml' WHERE CompanyId={0}",
                                   companyId));

            IStorageRepository repository = ObjectFactory.GetInstance<IStorageRepository>();
            Company company = repository.GetFilePreferences(AdminUserName);
            Assert.IsNotNull(company,"Company name cannot be null");
            Assert.IsTrue(!string.IsNullOrEmpty(company.AttributeMappingsXml) || !string.IsNullOrEmpty(company.RevisionRegEx), "Either Attribute Mappings or RegEx is null");
        }

        [TestMethod]
        public void Verify_JSONTree()
        {
            SetDocllerContext(122, "pateketu@gmail.com");
            IStorageService storageService = ServiceFactory.GetStorageService(122);
            Folders folders = storageService.GetFolders("pateketu@gmail.com", 1);
            string jscon = JsonConvert.SerializeObject(folders, Formatting.Indented,
                                                           new JsonSerializerSettings()
                                                           {
                                                               ContractResolver = new TreeContractResolver(),
                                                               NullValueHandling = NullValueHandling.Ignore

                                                           });
        }

#endregion

        #region version history
        //TODO Needs unit testing
        [TestMethod]
        public void Verify_GetFileHistroy()
        {
            //IStorageService storageService = ServiceFactory.GetStorageService();
            //IEnumerable<BlobBase> history = storageService.GetFileHistory(1801);

        }
        #endregion

        #region attachment 
        //TODO 
        [TestMethod]
        public void Verify_GetFileAttachments()
        {
           //IStorageService storageService = ServiceFactory.GetStorageService(1);
            //IEnumerable<BlobBase> history = storageService.GetFileHistory(1801);
        }
        //TODO 
        [TestMethod]
        public void Verify_DeleteAttachment()
        {
            //IStorageService storageService = ServiceFactory.GetStorageService(1);
            //IEnumerable<BlobBase> history = storageService.GetFileHistory(1801);

        }

        #endregion
        private void AddFiles(long customerId, int projectId, int folderId, int? number)
        {
            IStorageService storageService = new StorageService(ObjectFactory.GetInstance<IStorageRepository>(), ObjectFactory.GetInstance<IBlobStorageProvider>());
            //Get the test filenames
            List<string> fileNames = new List<string>();
            int i = 0;
            using (StreamReader reader = new StreamReader(@"..\..\..\Docller.UnitTests\FileList.txt"))
            {
                while (!reader.EndOfStream)
                {
                    fileNames.Add(reader.ReadLine());
                    i++;
                    if (number != null)
                    {
                        if (i == number.Value)
                        {
                            break;
                        }
                    }
                }
            }

            IEnumerable<Core.Models.File> files = storageService.GetPreUploadInfo(projectId, folderId, fileNames.ToArray(), true, false);
            int attachmentCounts = 0;
            foreach (File file in files)
            {
                file.Project.ProjectId = projectId;
                file.Folder.FolderId = folderId;
                file.CustomerId = customerId;
                file.ContentType = "fubar";
                file.Title = file.FileName[0].ToString();
                storageService.AddFile(file, true, "fubar");
                if (file.Attachments != null && file.Attachments.Count == 1)
                {
                    storageService.AddAttachment(file.Attachments[0], false, null);
                    attachmentCounts++;
                }
            }
        }

    }
}

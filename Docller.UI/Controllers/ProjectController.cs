using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Docller.Common;
using Docller.Core.Common;
using Docller.Core.Images;
using Docller.Core.Models;
using Docller.Core.Services;
using Docller.Models;
using Docller.UI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StructureMap;
using Docller.UI.Models;
using File = Docller.Core.Models.File;

namespace Docller.Controllers
{
    public class ProjectController : DocllerControllerBase
    {
        /// <summary>
        /// Sets the default selected link 
        /// </summary>
        private void SetTopNaArea()
        { 
            ViewBagWrapper.TopNavArea = Const.FileRegisterArea;
        }
        private void EnsureFolderInfo(long? folderId)
        {
            if (this.DocllerContext.FolderContext == null)
            {
                Folders folders =
                    SessionHelper.GetOrSet(
                        Utils.GetKeyForProject(SessionAndCahceKeys.AllFolders, this.DocllerContext.CustomerId, this.DocllerContext.ProjectId),
                        delegate()
                            {
                                IStorageService storageService =
                                    ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
                                return
                                    storageService.GetFolders(this.DocllerContext.UserName,
                                                              this.DocllerContext.ProjectId);

                            });
                IFolderContext folderContext = Factory.GetInstance<IFolderContext>();
                folderContext.Inject(folders, folderId != null ? folderId.Value : 0);
                this.DocllerContext.InjectFolder(folderContext);
                this.DocllerContext.Security.Refresh(folderId != null ? folderContext.AllFolders[folderId.Value] : null);
            }
        }

        private void EnsureProjectInfo()
        {
            if (this.DocllerContext.ProjectId > 0)
            {
                CacheHelper.GetOrSet(Utils.GetProjectKey(this.DocllerContext), CacheDurationHours.Long,
                                     delegate()
                                         {
                                             IProjectService projectService =
                                                 ServiceFactory.GetProjectService(this.DocllerContext.CustomerId);
                                         return
                                             projectService.GetProjectDetails(
                                                 this.DocllerContext.UserName,
                                                 this.DocllerContext.ProjectId);
                                     });

            }
        }

        private ActionResult UploadFile(BlobBase blobBase, int? chunk, int? chunks, bool uploadAsNewVersion)
        {
            if (Request.Files.Count == 1)
            {
                HttpPostedFileBase fileUpload = Request.Files[0];
                if (fileUpload != null)
                {
                    IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
                    
                    if (blobBase is FileAttachment)
                    {
                        FileAttachment attachment = (FileAttachment) blobBase;
                        storageService.Upload(attachment, fileUpload.InputStream, chunk.HasValue ? chunk.Value : 0,
                                              chunks.HasValue ? chunks.Value : 0, uploadAsNewVersion);
                       
                    }
                    else
                    {
                        File file = (File) blobBase;
                        storageService.Upload(file, fileUpload.InputStream, chunk.HasValue ? chunk.Value : 0,
                                              chunks.HasValue ? chunks.Value : 0, uploadAsNewVersion);
                        
                    }

                }
            }
            return Content("Success");
        }

        [HttpPost]
        public ActionResult UploadAttachment(FileAttachmentViewModel attachmentViewModel)
        {

            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            
            StorageServiceStatus serviceStatus = storageService.UploadAttachment(attachmentViewModel.GetFileAttachment(),
                                            attachmentViewModel.FileStream, attachmentViewModel.CurrentChunk,
                                            attachmentViewModel.TotalChunks);
            if (serviceStatus == StorageServiceStatus.Success)
            {
                return GetAttachments(attachmentViewModel.FileId);
            }
            return serviceStatus == StorageServiceStatus.Unknown
                       ? new HttpStatusCodeResult(HttpStatusCode.InternalServerError)
                       : new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult UploadFile(long folderId)
        {
            //long folderId;
            //long.TryParse(this.Request.Form[RequestKeys.FolderId], out folderId);
            EnsureProjectInfo();
            EnsureFolderInfo(folderId);
            
            
            int? chunk = !string.IsNullOrEmpty(Request.Form[RequestKeys.Chunk])
                             ? new int?(int.Parse(Request.Form[RequestKeys.Chunk]))
                             : null;
            int? chunks = !string.IsNullOrEmpty(Request.Form[RequestKeys.Chunks])
                             ? new int?(int.Parse(Request.Form[RequestKeys.Chunks]))
                             : null;

            bool uploadAsNewVersion = !string.IsNullOrEmpty(Request.Form[RequestKeys.UploadAsNewVersion])
                                          ? true
                                          : false;
            return this.UploadFile(Utils.DeserializeBlobDetails(this.Request), chunk, chunks, uploadAsNewVersion);
        }

        [HttpPost]
        public JsonNetResult GetMetaDataInfo(FilesMetaInfo fileUploadInfo)
        {
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            IEnumerable<File> fileMetaData = storageService.GetPreUploadInfo(fileUploadInfo.ProjectId, fileUploadInfo.FolderId, fileUploadInfo.Files.Select(x => x.Name).ToArray(), fileUploadInfo.AttachCADFiles,
                                            false);

            //Copy the size and Id info just for the display purpose
            Dictionary<string, FileMetaData> allMetaData =
                fileUploadInfo.Files.ToDictionary(x => x.Name.ToLowerInvariant(),
                                                  StringComparer.InvariantCultureIgnoreCase);

            foreach (File file in fileMetaData)
            {
                file.FileSize = allMetaData[file.FileName].Size;
                file.UniqueIdentifier = allMetaData[file.FileName].Id;
                
                foreach (FileAttachment fileAttachment in file.Attachments)
                {
                    fileAttachment.FileSize = allMetaData[fileAttachment.FileName].Size;
                    fileAttachment.UniqueIdentifier = allMetaData[fileAttachment.FileName].Id;
                }
            }

            JsonNetResult jsonResult = new JsonNetResult()
                                           {
                                               Data = fileMetaData,
                                               JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                           };

            return jsonResult;
        }

        public ActionResult FileRegister(long projectId, long? folderId, FileSortBy? sortBy, SortDirection? sortDirection, int? pageNumber, bool? showAsPicker, bool? showFileArea, bool? thView)
        {

            SetTopNaArea();
            EnsureProjectInfo();
            EnsureFolderInfo(folderId);
            IFolderContext folderContext = ObjectFactory.GetInstance<IFolderContext>();
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            FileRegisterViewModel model = new FileRegisterViewModel() {FolderCrumbs = new Queue<Folder>()};
            if (folderId != null)
            {
                if (sortBy == null)
                {
                    sortBy = FileSortBy.Date;
                    sortDirection = SortDirection.Descending;
                }
                
                if (sortDirection == null)
                {
                    sortDirection = SortDirection.Ascending;
                }

                if (pageNumber == null || pageNumber.Value <= 0)
                {
                    pageNumber = 1;
                }

                Files files = storageService.GetFiles(projectId, folderId.Value, this.DocllerContext.UserName, sortBy.Value,
                                                                   sortDirection.Value, pageNumber.Value, Const.DefaultPageSize);
                model.Files = files;
            }
            
            if (folderContext.CurrentFolderId > 0)
            {
                Folder currentFolder = folderContext.AllFolders[folderContext.CurrentFolderId];
                model.FolderCrumbs.Enqueue(currentFolder);
                if (currentFolder.AllParents != null && currentFolder.AllParents.Count > 0)
                {
                    for (int i = currentFolder.AllParents.Count - 1; i >= 0; i--)
                    {
                        model.FolderCrumbs.Enqueue(folderContext.AllFolders[currentFolder.AllParents[i]]);
                    }
                }
            }

            ViewBagWrapper.ShowAsPicker = showAsPicker.HasValue && showAsPicker.Value;
            ViewBagWrapper.ThumbnailView = thView != null && thView.Value;
            if (showAsPicker.HasValue && showAsPicker.Value)
            {
                if (showFileArea.HasValue && showFileArea.Value)
                {
                    return PartialView("_FileArea", model);
                }
                return PartialView("_FilePicker", model);
            }
            
            return View(model);
        }

        public ActionResult ZoomablePreview(long projectId, long fileId, string internalName, long pTag, string tile)
        {
            IPreviewImageProvider previewImageProvider = Factory.GetInstance<IPreviewImageProvider>();
            File f = new File()
                {
                    FileId = fileId,
                    FileInternalName =
                        new Guid(internalName),
                    Project =
                        new Project()
                            {
                                ProjectId = projectId
                            }
                };
            if (string.IsNullOrEmpty(tile))
            {
                string zoomableFile = previewImageProvider.GetZoomablePreview(this.DocllerContext.CustomerId,f);
                return File(zoomableFile, "text/xml");
            }
            else
            {
                return File(previewImageProvider.GetTile(this.DocllerContext.CustomerId,f,tile), "image/png");
            }
            
        }
        [IfPreviewModifiedSince()]
        public ActionResult FilePreview(long projectId, long fileId, string internalName, long pTag, Docller.Core.Images.PreviewType previewType)
        {
            if (pTag > 0)
            {
                IPreviewImageProvider previewImageProvider = Factory.GetInstance<IPreviewImageProvider>();
                string filePath =
                    previewImageProvider.GetPreview(this.DocllerContext.CustomerId,
                                                    new File()
                                                        {
                                                            FileId = fileId,
                                                            FileInternalName = new Guid(internalName),
                                                            Project = new Project() {ProjectId = projectId}
                                                        }, previewType);
                return new FilePreviewResult(filePath, "image/png", pTag);
            }
            string noPreviewFile = string.Format("~/Images/NoPreview_{0}Thumb.png", previewType == PreviewType.PThumb ? "P" : "S");
            IPathMapper mapper = Factory.GetInstance<IPathMapper>();
            return new FilePreviewResult(mapper.MapPath(noPreviewFile), "image/png", new DateTime(1970, 1, 1).Ticks);

        }
        [HttpPost]
        public ActionResult EditFiles(long projectId, long? folderId, string uploadedFiles)
        {
            SetTopNaArea();
            EnsureProjectInfo();
            EnsureFolderInfo(folderId);
           
            string[] internalNames = uploadedFiles.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<File> files = new List<File>();
            foreach (string internalName in internalNames)
            {
                Guid guidName;
                if (Guid.TryParse(internalName, out guidName))
                {
                    files.Add(new File() {FileInternalName = guidName});
                }
            }

            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            IEnumerable<File> filesToEdit = storageService.GetFilesForEdit(files);
            JsonNetResult jsonResult = new JsonNetResult()
                                           {
                                               Data = filesToEdit,
                                               JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                           };
            string fileDataJson = jsonResult.GetRawJson();

            //IEnumerable<Status> projectStatues = GetProjectStatus();

            //jsonResult.Data = projectStatues;
            //string statusJson = jsonResult.GetRawJson();

            return View("EditUploadedFiles", new FileEditData() {FileJson = fileDataJson/*, StatusJson = statusJson*/});
        }

        [HttpPost]
        public JsonNetResult UpdateFiles(File[] files)
        {
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            IEnumerable<File> erroredFiles;
            JsonNetResult jsonResult;
            if (storageService.TryUpdateFiles(new List<File>(files), out erroredFiles) == StorageServiceStatus.Success)
            {
                jsonResult = new JsonNetResult()
                    {
                        Data = new {Success = true},
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet
                    };
            }
            else
            {
                jsonResult = new JsonNetResult()
                    {
                        Data = new { Success = false, Files = erroredFiles },
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet
                    };
            }
            return jsonResult;
        }

        [HttpPost]
        public ActionResult UploadVersion(long projectId, long folderId, long fileId, string name, decimal fileSize, int? chunk, int? chunks)
        {
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            StorageServiceStatus status = storageService.UploadVersion(fileId, folderId,projectId, name, fileSize,
                                                                       this.Request.Files[0].InputStream,
                                                                       chunk.HasValue ? chunk.Value : 0,
                                                                       chunks.HasValue ? chunks.Value : 0);
            if (status == StorageServiceStatus.Success)
            {
                var data = new {success = true};
                return new JsonNetResult() {Data = data};
            }
            if(status == StorageServiceStatus.UploadInProgress)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);    
            }
            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
        }
        [HttpPost]
        public ActionResult UploadComment(MarkupFileUploadModel model)
        {
            
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            StorageServiceStatus status = storageService.UploadComment(model.ProjectId, model.FolderId, model.FileId, model.Name, model.FileSize,
                                                                       model.FileStream,
                                                                       model.Chunk != null ? model.Chunk.Value : 0,
                                                                       model.Chunks != null ? model.Chunks.Value : 0,
                                                                       model.Comments, false, null);
            if (status == StorageServiceStatus.Success)
            {
                var data = new { success = true };
                return new JsonNetResult() { Data = data };
                
            }
            if (status == StorageServiceStatus.UploadInProgress)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

        }

        [HttpPost]
        public ActionResult GetCommentMetaInfo(long projectId, long folderId, long fileId, string name)
        {
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            MarkUpFile markUpFile = storageService.GetMarkUpMetadataInfo(name, fileId, folderId, projectId);
            if (markUpFile != null)
            {
                bool isByOtherUser = markUpFile.IsExistingFile && !markUpFile.CreatedByCurrentUser;
                var data =
                    new
                        {
                            success = !isByOtherUser,
                            existingMarkUpFileByAnotherUser = isByOtherUser,
                            existingFile = markUpFile.IsExistingFile
                        };
                return new JsonNetResult() {Data = data};

            }
            return new JsonNetResult() {Data = new {success = false}};
        }

        public ActionResult Comments(long projectId, long folderId, long fileId, string name)
        {
            return View();
        }

        public ActionResult Lists()
        {
            SetTopNaArea();
            return View("_ProjectList");
        }

       
        
        /*public ActionResult Index(long projectId)
        {
            EnsureProjectInfo();
            return View();
        }*/

        public ActionResult Create()
        {
            SetTopNaArea();
            return View();
        }


        [HttpPost]
        public ActionResult Create(Project project)
        {
            SetTopNaArea();
            if (ModelState.IsValid)
            {
                IProjectService projectService = ServiceFactory.GetProjectService(this.CurrentCookieData.CustomerId);
                project.CustomerId = this.CurrentCookieData.CustomerId;
                projectService.Create(this.User.Identity.Name, project);
                return RedirectToAction("FileRegister", new { ProjectId = project.ProjectId });
            }

            return View(project);
        }

        public JsonNetResult GetProjectSubscribers(long projectId, string searchString)
        {
            ICustomerSubscriptionService customerSubscriptionService = ServiceFactory.GetCustomerSubscriptionService(this.DocllerContext.CustomerId);
            IEnumerable<SubscriberItem> subscribers = customerSubscriptionService.Search(projectId, searchString);
            JsonNetResult jsonResult = new JsonNetResult(new JsonSerializerSettings()
                                                            {
                                                                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                                                NullValueHandling = NullValueHandling.Ignore
                                                            })
                {
                    Data = subscribers,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            return jsonResult;

        }

        public ActionResult InviteUsers(long projectId)
        {

            string rawJson = GetSubscribersJson();

            return PartialView("_InviteUsers", rawJson);
        }

        public ActionResult ProjectInvitation(long projectId)
        {
            EnsureProjectInfo();
            string rawJson = GetSubscribersJson();

            return PartialView("_ProjectInvitation", rawJson);
        }

        [HttpPost]
        public ActionResult ProjectInvitation(long projectId, string perm, IEnumerable<Company> companies)
        {
            if (this.ModelState.IsValid)
            {
                EnsureProjectInfo();
                PermissionFlag permissionFlag;
                if (!Enum.TryParse(perm, false, out permissionFlag))
                    return null;

                ICustomerSubscriptionService customerSubscriptionService =
                    ServiceFactory.GetCustomerSubscriptionService(this.DocllerContext.CustomerId);
                IEnumerable<UserInvitationError> errors;
                IEnumerable<User> users = customerSubscriptionService.SubscribeCompanies(projectId, 
                                                                                         permissionFlag, companies,
                                                                                         out errors);
                if (errors.Any())
                {
                    return GetUserInvitationsErrors(errors);
                }
                EnsureProjectInfo();
                ProjectInvitationResults invitationResults = new ProjectInvitationResults(users, this.DocllerContext,
                                                                                        this.CurrentCookieData, permissionFlag)
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet
                };
                var data = new { Success = true };
                invitationResults.Data = data;
                ClearSubscribersCache(projectId);

                return invitationResults;
            }
            return GetModelErrorsJson();
        }

        [HttpPost]
        public ActionResult ShareFiles(ShareFilesViewModel files)
        {
            if (!this.ModelState.IsValid)
            {
                return GetModelErrorsJson();
            }
            ITransmittalService service = ServiceFactory.GetTransmittalService(this.DocllerContext.CustomerId);
            SharedFilesInfo sharedFilesInfo =  service.ShareFiles(files.ProjectId, files.FolderId, files.FileIds, files.Emails, files.EmailMessage);

            var data = new { Success = true};
            return new ShareFilesResults(sharedFilesInfo, files.Emails, files.EmailMessage, this.DocllerContext,
                this.CurrentCookieData) {JsonRequestBehavior = JsonRequestBehavior.DenyGet, Data = data};
            
        }

        
        public ActionResult ShareFiles(long projectId, long folderId)
        {
            EnsureProjectInfo();
            EnsureFolderInfo(folderId);
            
            return PartialView("_ShareFiles");
        }


        public ActionResult ShareFolder(long projectId, long folderId)
        {
            EnsureFolderInfo(folderId); 
            string rawJson = GetSubscribersJson();

            return PartialView("_ShareFolder", rawJson);
        }

        [HttpPost]
        public ActionResult ShareFolder(long projectId, long folderId, string perm, IEnumerable<Company> companies)
        {
            if (this.ModelState.IsValid)
            {
                EnsureFolderInfo(folderId); 
                PermissionFlag permissionFlag;
                if (!Enum.TryParse(perm, false, out permissionFlag))
                    return null;

                ICustomerSubscriptionService customerSubscriptionService =
                    ServiceFactory.GetCustomerSubscriptionService(this.DocllerContext.CustomerId);
                IEnumerable<UserInvitationError> errors;
                IEnumerable<User> users = customerSubscriptionService.SubscribeCompanies(projectId, folderId,
                                                                                         permissionFlag, companies,
                                                                                         out errors);
                if (errors.Any())
                {
                    return GetUserInvitationsErrors(errors);
                }
                EnsureProjectInfo();
                ShareFolderResults shareFolderResults = new ShareFolderResults(users, this.DocllerContext,
                                                                                        this.CurrentCookieData, permissionFlag)
                    {
                        JsonRequestBehavior = JsonRequestBehavior.DenyGet
                    };
                var data = new { Success = true };
                shareFolderResults.Data = data;
                ClearSubscribersCache(projectId);

                return shareFolderResults;
            }
            return GetModelErrorsJson();
        }

        [HttpPost]
        public ActionResult FolderPermCompare(long projectId, long folderId, string companyName, string perm)
        {
            PermissionFlag newPermFlag;
            JsonResult result = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                
            };
            if (Enum.TryParse(perm, false, out newPermFlag))
            {
                ISecurityService securityService = ServiceFactory.GetSecurityService(this.DocllerContext.CustomerId);
                PermissionFlag currentPermissionFlag = securityService.GetFolderPermission(companyName, folderId);
                if (newPermFlag == PermissionFlag.Read &&
                    currentPermissionFlag.HasPermissions(PermissionFlag.ReadWrite))
                {
                    result.Data = new { downgradingPermissions = true };
                }
              
            }
            return result;
        }

        [HttpPost]
        public ActionResult Subscribe(long projectId, IEnumerable<Company> companies)
        {

            if (this.ModelState.IsValid)
            {
                ICustomerSubscriptionService customerSubscriptionService =
                    ServiceFactory.GetCustomerSubscriptionService(this.DocllerContext.CustomerId);
                IEnumerable<UserInvitationError> errors;
                IEnumerable<User> users = customerSubscriptionService.SubscribeCompanies(projectId, companies, out errors);
                if (errors.Any())
                {
                    return GetUserInvitationsErrors(errors);
                }
                EnsureProjectInfo();
                SubscribeUsersResults subscribeUsersResults = new SubscribeUsersResults(users.Where(x=>x.IsNew), this.DocllerContext,
                                                                                        this.CurrentCookieData)
                {
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet
                };
                var data = new { Success = true };
                subscribeUsersResults.Data = data;
                ClearSubscribersCache(projectId);

                return subscribeUsersResults;
            }
            return GetModelErrorsJson();
        }

        [HttpPost]
        public JsonNetResult CreateFolder(FolderViewModel viewModel)
        {
            JsonNetResult jsonResult = new JsonNetResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            if (ModelState.IsValid)
            {
                IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
                List<Folder> folders =new List<Folder>();
                string[] f = viewModel.FolderName.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (f.Length > 0)
                {
                    folders.AddRange(
                        f.Select(
                            s =>
                            new Folder()
                                {
                                    FolderName = s,
                                    CustomerId = this.DocllerContext.CustomerId
                                }));
                   FolderCreationStatus status = storageService.CreateFolders(this.DocllerContext.UserName, viewModel.ProjectId,
                                                 viewModel.ParentFolderId, folders);
                    if (status.Status == StorageServiceStatus.Success)
                    {
                        jsonResult.Data =
                            new
                                {
                                    success = true,
                                    url =
                                        string.Format("/Project/FileRegister/{0}?{1}={2}", this.DocllerContext.ProjectId,
                                                      QueryStrings.FolderId,
                                                      folders.First().FolderId)
                                };

                    }
                    if (status.Status == StorageServiceStatus.ExistingFolder)
                    {
                        StringBuilder dupFolderMsg = new StringBuilder();
                        for (int i = 0; i < status.DuplicateFolders.Count; i++)
                        {
                            if (i < status.DuplicateFolders.Count - 1)
                            {
                                dupFolderMsg.AppendFormat("{0}, ", status.DuplicateFolders[i].FolderName);
                            }
                            else
                            {
                                dupFolderMsg.Append(status.DuplicateFolders[i].FolderName);
                            }
                        }
                        dupFolderMsg.Append(status.DuplicateFolders.Count == 1
                                                ? " is an exisiting folder."
                                                : " are exisiting folder, however other folders were created");
                        jsonResult.Data = new { success = false, duplicatefolderMessage=dupFolderMsg.ToString() };
                    }
                    InvalidatedAllFoldersCache();
                }
                
            }
            return jsonResult;
        }

        [HttpPost]
        public ActionResult RenameFolder(long projectId, long folderId, string folderName)
        {
            EnsureProjectInfo();
            EnsureFolderInfo(folderId);
            Folder folder = new Folder { FolderId = folderId, ProjectId = projectId, ParentFolderId = 0, FolderName = folderName };
            IStorageService strorageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            StorageServiceStatus status = strorageService.RenameFolder(folder);
            if (status == StorageServiceStatus.Success)
            {
                InvalidatedAllFoldersCache();
            }
            return new JsonNetResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data =
                    new
                    {
                        success = status == StorageServiceStatus.Success,
                        existingFolder = status == StorageServiceStatus.ExistingFolder
                    }
            };

        }
        
        public ActionResult EditTransmital(long projectId, long transmittalId)
        {
            SetTopNaArea();

            ITransmittalService service = ServiceFactory.GetTransmittalService(this.DocllerContext.CustomerId);
            Transmittal transmittal = service.GetTransmittal(projectId, transmittalId);
            if (transmittal.IsDraft)
            {
                TransmittalViewModel viewModel = new TransmittalViewModel(transmittal, GetProjectStatus());
                return View("CreateTransmital", viewModel);
            }
            return new HttpNotFoundResult();

        }


        public ActionResult CreateTransmital(long projectid, long folderId)
        {
            SetTopNaArea();
            EnsureProjectInfo();
            EnsureFolderInfo(folderId);
            if (this.DocllerContext.Security.CanCreateTransmittal)
            {

                return View(new TransmittalViewModel(GetProjectStatus()));
            }
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
        }

        [HttpPost]
        public ActionResult CreateTransmital(long projectid, string updatedFiles)
        {
            SetTopNaArea();
           
            string[] internalNames = updatedFiles.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<Guid> guids = new List<Guid>();
            foreach (string internalName in internalNames)
            {
                Guid guidName;
                if (Guid.TryParse(internalName, out guidName))
                {
                    guids.Add(guidName);
                }
            }

            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            IEnumerable<File> files = storageService.GetFiles(guids);
            TransmittalViewModel viewModel = new TransmittalViewModel(files, GetProjectStatus());
            return View(viewModel);
        }

        [HttpPost]
        [MultiButtonActionAttribute(Button = TransmittalButtons.Transmit)]
        public ActionResult Transmit(TransmittalViewModel viewModel)
        {
            SetTopNaArea();
            if (ModelState.IsValid)
            {
                viewModel.StatusList = GetProjectStatus();
                Transmittal transmittal = viewModel.Convert(this.DocllerContext);
                transmittal.IsDraft = false;
                
                TransmittalServiceStatus status = CreateNewTransmittal(transmittal, viewModel.To, viewModel.Cc);
                if (status == TransmittalServiceStatus.Success)
                {
                    return RedirectToAction("TransmittalCreated",
                                            new RouteValueDictionary()
                                                {
                                                    {"projectId", this.DocllerContext.ProjectId},
                                                    {"transmittalId", transmittal.TransmittalId}
                                                });
                }
            }
            return View("CreateTransmital", viewModel);
        }

        [HttpPost]
        [MultiButtonActionAttribute(Button = TransmittalButtons.SaveTransmittal)]
        public ActionResult SaveTransmittal(TransmittalViewModel viewModel)
        {
            SetTopNaArea();
            if (ModelState.IsValid)
            {
                viewModel.StatusList = GetProjectStatus();
                Transmittal transmittal = viewModel.Convert(this.DocllerContext);
                transmittal.IsDraft = true;
                TransmittalServiceStatus status = CreateNewTransmittal(transmittal, viewModel.To,viewModel.Cc);
                if (status == TransmittalServiceStatus.Success)
                {
                    ViewBagWrapper.TransmittalSaved = true;
                    ICustomerSubscriptionService customerSubscription =
                        ServiceFactory.GetCustomerSubscriptionService(this.DocllerContext.CustomerId);
                    //Need to Populate Subscritem's text
                    viewModel.To = customerSubscription.GetSubscribers(this.DocllerContext.ProjectId,
                                                                       viewModel.To.Select(x => x.Id).ToArray());
                    viewModel.Cc = customerSubscription.GetSubscribers(this.DocllerContext.ProjectId,
                                                                       viewModel.Cc.Select(x => x.Id).ToArray());
                    viewModel.TransmittalId = transmittal.TransmittalId;
                }
            }
            return View("CreateTransmital", viewModel);
        }

        public ActionResult TransmittalCreated(long projectId, long transmittalId)
        {
            SetTopNaArea();
            ITransmittalService transmittalService = ServiceFactory.GetTransmittalService(this.DocllerContext.CustomerId);
            Transmittal transmittal = transmittalService.GetTransmittal(projectId, transmittalId);

            return View(transmittal);
        }

        public ActionResult FileHistory(long projectId, long folderId, long fileId)
        {
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            FileHistory history= storageService.GetFileHistory(fileId);
            
            return PartialView("_FileHistory", history);
        }

        public ActionResult IssueSheet(long transmittalId)
        {
            ITransmittalService transmittalService = ServiceFactory.GetTransmittalService(this.DocllerContext.CustomerId);
            Stream stream = transmittalService.GetIssueSheet(transmittalId,false);

            return new FileStreamResult(stream, MIMETypes.Current[".pdf"]) {FileDownloadName = "IssueSheet"};
            
        }

        public ActionResult ManageAttachments(long projectId, long folderId, long fileId)
        {
            JsonNetResult result = GetAttachments(fileId);
            return PartialView("ManageAttachments", result.GetRawJson());
        }

        [HttpPost]
        public JsonNetResult DeleteAttachment(long projectId, long fileId, int? revisionNumber)
        {
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            JsonNetResult jsonNet = new JsonNetResult() { Data = new FileAttachmentViewModel() };
            if (revisionNumber == null ||  revisionNumber.Value == 0)
            {
                storageService.DeleteAttachment(new FileAttachment() {FileId = fileId});
            }
            else
            {
                FileAttachment attachment = storageService.DeleteAttachment(new FileAttachmentVersion()
                    {
                        FileId = fileId,
                        RevisionNumber = revisionNumber.Value
                    });
                jsonNet.Data = new FileAttachmentViewModel(attachment);
            }
            return jsonNet;
        }

        [HttpPost]
        public ActionResult DeleteFileVersion(long projectId, long fileId, int revisionNumber)
        {
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            FileHistory history= storageService.DeleteFileVersion(fileId, revisionNumber);
            return PartialView("_FileHistory", history);
        }

        [HttpPost]
        public JsonNetResult DeleteFiles(long projectId, long[] fileIds)
        {
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            bool allDeleted = storageService.DeleteFiles(fileIds, projectId);
            return new JsonNetResult() { Data = new { AllDeleted = allDeleted } };

        }

      
        public ActionResult ManageProjectPermissions(long projectId)
        {
            EnsureProjectInfo();
            ISecurityService securityService = ServiceFactory.GetSecurityService(this.DocllerContext.CustomerId);
            IEnumerable<PermissionInfo> permissionInfos = securityService.GetProjectPermissions(projectId);
            //JsonNetResult jsonResult = new JsonNetResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = permissionInfos};
            //string json = jsonResult.GetRawJson();
            return PartialView("_ManageProjectPermissions", permissionInfos);
        }

        [HttpPost]
        public ActionResult SaveProjectPermissions(long projectId, IEnumerable<PermissionInfo> changedPermissions)
        {
            EnsureProjectInfo();
            ISecurityService securityService = ServiceFactory.GetSecurityService(this.DocllerContext.CustomerId);
            securityService.UpdateProjectPermissions(projectId, changedPermissions);
            JsonNetResult jsonNetResult = new JsonNetResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { Success = false }
            };
            return jsonNetResult;
        } 

        public ActionResult ManageFolderPermissions(long projectId, long folderId)
        {
            EnsureProjectInfo();
            ISecurityService securityService = ServiceFactory.GetSecurityService(this.DocllerContext.CustomerId);
            IEnumerable<PermissionInfo> permissionInfos = securityService.GetFolderPermissions(projectId, folderId);
            //JsonNetResult jsonResult = new JsonNetResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = permissionInfos};
            //string json = jsonResult.GetRawJson();
            return PartialView("_ManageFolderPermissions", permissionInfos);
        }

        [HttpPost]
        public ActionResult SaveFolderPermissions(long projectId, long folderId, IEnumerable<PermissionInfo> changedPermissions)
        {
            EnsureProjectInfo();
            ISecurityService securityService = ServiceFactory.GetSecurityService(this.DocllerContext.CustomerId);
            securityService.UpdateFolderPermissions(projectId, folderId, changedPermissions);
            JsonNetResult jsonNetResult = new JsonNetResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                Data = new { Success = false }
            };
            return jsonNetResult;
        }

        public ActionResult Transmittals(long projectId, MyTransmittalSearch? searchOptions, int? pageNum)
        {
            EnsureProjectInfo();
            ITransmittalService transmittalService = ServiceFactory.GetTransmittalService(this.DocllerContext.CustomerId);
            if (searchOptions == null) searchOptions = MyTransmittalSearch.MyDrafts;
            PageableData<Transmittal> transmittals = null;
            int pNum = pageNum != null ? pageNum.Value : 0;
            switch (searchOptions)
            {
                case MyTransmittalSearch.MyTransmittals:
                    transmittals = transmittalService.GetMyTransmittals(projectId, false, pNum, Const.DefaultPageSize);
                    break;
                case MyTransmittalSearch.SendToMe:
                    transmittals = transmittalService.GetTransmittalsForMe(projectId, pNum, Const.DefaultPageSize);
                    break;
                case MyTransmittalSearch.SendToMyCompany:
                    transmittals = transmittalService.GetTransmittalsForMyCompany(projectId, pNum, Const.DefaultPageSize);
                    break;
                default:
                    transmittals = transmittalService.GetMyTransmittals(projectId, true, pNum, Const.DefaultPageSize);
                    break;
            }
            ViewBagWrapper.TransmittalSearchOption = searchOptions;
            return View(transmittals);
        }

        public ActionResult UpdateProject(long projectId)
        {
            EnsureProjectInfo();
            Project p = Utils.GetCurrentProject(this.DocllerContext);
            return PartialView("_UpdateProject", p);
        }
        [HttpPost]
        public ActionResult UpdateProject(Project project, HttpPostedFileBase logo)
        {
            EnsureProjectInfo();
            IProjectService projectService = ServiceFactory.GetProjectService(this.DocllerContext.CustomerId);
            projectService.UpdateProject(project);
            this.DocllerContext.Cache.Remove(Utils.GetKeyForProject(SessionAndCahceKeys.ProjectNameFormat,
                this.DocllerContext.CustomerId, project.ProjectId));
            return RedirectToAction("FileRegister", new {ProjectId = project.ProjectId});
        }

        [NonAction]
        private void InvalidatedAllFoldersCache()
        {
            string key = Utils.GetKeyForProject(SessionAndCahceKeys.AllFolders, this.DocllerContext.CustomerId, this.DocllerContext.ProjectId);
            SessionHelper.Invalidate(key);
        }

        [NonAction]
        private JsonNetResult GetUserInvitationsErrors(IEnumerable<UserInvitationError> errors)
        {
            IEnumerable<string> messages =
                        errors.Select(
                            userInvitationError =>
                            string.Format("{0} belongs to {1}, while you are attempting to add user to {2}",
                                          userInvitationError.UserName, userInvitationError.ExistingCompany,
                                          userInvitationError.CompanyName));
            JsonNetResult jsonNetResult = new JsonNetResult() { JsonRequestBehavior = JsonRequestBehavior.DenyGet };
            var data = new { Success = false, Errors = messages };
            jsonNetResult.Data = data;
            return jsonNetResult;
        }

        [NonAction]
        private JsonNetResult GetModelErrorsJson()
        {
            JsonNetResult jsonNetResult = new JsonNetResult() { JsonRequestBehavior = JsonRequestBehavior.DenyGet };
            var data = new { Success = false, Errors = new List<string>() };

            foreach (ModelState modelState in this.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        data.Errors.Add(error.ErrorMessage);
                    }
                }
            }
            jsonNetResult.Data = data;

            return jsonNetResult;
        }
        
        
        [NonAction]
        private void ClearSubscribersCache(long projectId)
        {
            this.DocllerContext.Cache.Remove(Utils.GetKeyForProject(SessionAndCahceKeys.AllSubscribers,
                                                                        this.DocllerContext.CustomerId, projectId));
        }
        

        [NonAction]
        private TransmittalServiceStatus CreateNewTransmittal(Transmittal transmittal, IEnumerable<SubscriberItem> to, IEnumerable<SubscriberItem> cc)
        {
            ITransmittalService transmittalService = ServiceFactory.GetTransmittalService(this.DocllerContext.CustomerId);
            TransmittalCreationInfo info = transmittalService.CreateTransmittal(transmittal, to, cc);
            if (info.Status == TransmittalServiceStatus.Success && info.Transmittal != null && !info.Transmittal.IsDraft)
            {

                info.Transmittal.ProjectName = Utils.GetCurrentProjectName(this.DocllerContext);
                info.Transmittal.ProjectId = this.DocllerContext.ProjectId;
                info.Transmittal.CustomerId = this.DocllerContext.CustomerId;
                info.Transmittal.CustomerName = CustomerCache.Get(this.DocllerContext.CustomerId).CustomerName;
                if (!string.IsNullOrEmpty(CustomerCache.Get(this.DocllerContext.CustomerId).ImageUrl))
                {
                    ISubscriptionService subscriptionService = ServiceFactory.GetSubscriptionService();
                    info.IssueSheetData.CustomerLogo = subscriptionService.LogoFile(this.DocllerContext.CustomerId);
                }
                TransmittalCreationActions actions =
                    new TransmittalCreationActions(Factory.GetInstance<IIssueSheetProvider>());
                actions.Start(this.HttpContext, info);
            }
            return info.Status;
        }
        
        [NonAction]
        private IEnumerable<SelectListItem> GetProjectStatus()
        {
            return
                CacheHelper.GetOrSet(
                    Utils.GetKeyForProject(SessionAndCahceKeys.ProjectStatusesFormat, this.DocllerContext.CustomerId, this.DocllerContext.ProjectId),
                    CacheDurationHours.Default,
                    delegate()
                        {
                            IProjectService projectService =
                                ServiceFactory.GetProjectService(this.DocllerContext.CustomerId);
                            List<Status> statues =
                                new List<Status>(projectService.GetProjectStatuses(this.DocllerContext.ProjectId));
                            statues.Insert(0, new Status() { StatusId = 0, StatusLongText = string.Empty, StatusText = string.Empty });
                            IEnumerable<SelectListItem> listItems = statues.Select(status => new SelectListItem()
                                {
                                    Text = status.StatusText,
                                    Value = status.StatusId.ToString(CultureInfo.InvariantCulture)
                                });

                            return listItems;
                        });
        }
        
        [NonAction]
        private JsonNetResult GetAttachments(long fileId)
        {
            IStorageService storageService = ServiceFactory.GetStorageService(this.DocllerContext.CustomerId);
            FileAttachment fileAttachment = storageService.GetFileAttachment(fileId);
            FileAttachmentViewModel view;
            if (fileAttachment != null)
            {
                view = new FileAttachmentViewModel(fileAttachment);
            }
            else
            {
                view = new FileAttachmentViewModel(fileId);
            }
            JsonNetResult json = new JsonNetResult()
            {
                Data = view
            };
           return json;
         
        }
        [NonAction]
        private string GetSubscribersJson()
        {
            ICustomerSubscriptionService customerSubscription =
               ServiceFactory.GetCustomerSubscriptionService(this.DocllerContext.CustomerId);
            IEnumerable<Company> companies = customerSubscription.GetSubscribedCompanies();

            JsonNetResult jsonResult = new JsonNetResult()
            {
                Data = companies,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return jsonResult.GetRawJson();
        }
        
    }
}

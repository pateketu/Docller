using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Security;
using System.Text.RegularExpressions;
using Docller.Core.Common;
using Docller.Core.Exceptions;
using Docller.Core.Images;
using Docller.Core.Infrastructure;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Storage;
using File = Docller.Core.Models.File;

namespace Docller.Core.Services
{
    public class StorageService : ServiceBase<IStorageRepository>, IStorageService
    {
        private readonly IBlobStorageProvider _blobStorageProvider;
        private ISecurityService _securityService;
        public StorageService(IStorageRepository repository, IBlobStorageProvider blobStorageProvider) : base(repository)
        {
            _blobStorageProvider = blobStorageProvider;
        }

        public FolderCreationStatus CreateFolders(string userName, long projectId, List<Folder> folders)
        {
            return CreateFolders(userName, projectId, 0, folders);
        }

        public FolderCreationStatus CreateFolders(string userName, long projectId, long parentFolder, List<Folder> folders)
        {
            ContextInfo context = new ContextInfo()
            {
                CurrentUserName = userName,
                CurrentProjectId = projectId,
                CurrentCustomerId = Context.CustomerId
            };

            if (!this.GetSecurityService(Context.CustomerId).CanCreateFolder(context,
                                                                     parentFolder))
            {
                throw new SecurityAccessDeniedException();
            }
            EnsureFolderIds(folders);
            FolderCreationStatus status = new FolderCreationStatus();
            List<Folder> duplicateFolders;
            int returnVal = this.Repository.CreateFolders(userName, Context.CustomerId, projectId, parentFolder, PermissionFlag.Admin,
                                          this._blobStorageProvider.GetPathSeparator(), folders, out duplicateFolders);
            status.Status = (StorageServiceStatus)returnVal;
            status.DuplicateFolders = duplicateFolders;
            return status;
        }

        public Folders GetFolders(string userName, long projectId)
        {
            return this.Repository.GetFolders(userName, projectId, 0, 0);
        }

        public StorageServiceStatus RenameFolder(Folder folder)
        {
            return (StorageServiceStatus)this.Repository.RenameFolder(folder);
        }

        public virtual IEnumerable<File> GetPreUploadInfo(long projectId, long folderId, string[] fileNames, bool attachCADFilesToPdfs, bool patternMatchForVersions)
        {
            if(projectId <= 0)
                throw new ArgumentException("projectId");

            if(folderId <= 0)
                throw new ArgumentException("folderId");

            string regEx = string.Empty;
            if(patternMatchForVersions)
            {
                Company company = this.GetFilePref();
                regEx = company.RevisionRegEx;
            }

            List<File> files = GetFilesWithAttachments(fileNames, attachCADFilesToPdfs, regEx);
            return GetFileVersionInfo(projectId, folderId, files, regEx);
        }

        public StorageServiceStatus AddFile(File file, bool addAsNewVersion, string versionPath)
        {
            if (file.CustomerId == 0) file.CustomerId = this.Context.CustomerId;
            if(file.FileId <= 0) file.FileId = IdentityGenerator.Create(IdentityScope.File);
            EnsureFileInfo(file);
            
            RepositoryStatus status = this.Repository.AddFile(file, addAsNewVersion, versionPath,this.Context.UserName);
            return (StorageServiceStatus) status;
        }

        public StorageServiceStatus AddAttachment(FileAttachment attachment, bool addAsNewVersion, string versionPath)
        {
            if (attachment.CustomerId == 0) attachment.CustomerId = this.Context.CustomerId;
            if (attachment.FileId <= 0 && attachment.ParentFile.Equals(Guid.Empty)) throw new ArgumentException("FileId or ParentFile is required");
            EnsureFileInfo(attachment);
            RepositoryStatus status = this.Repository.AddAttachment(attachment, addAsNewVersion, versionPath, Context.UserName);
            return (StorageServiceStatus)status;
        }

        public StorageServiceStatus AddAttachment2(FileAttachment2 attachment, bool addAsNewVersion, string versionPath)
        {
            if (attachment.CustomerId == 0) attachment.CustomerId = this.Context.CustomerId;
            if (attachment.FileId <= 0 ) throw new ArgumentException("FileId is required");
            attachment.FileAttachmentId = IdentityGenerator.Create(IdentityScope.FileAttachments);
            EnsureFileInfo(attachment);
            RepositoryStatus status = this.Repository.AddAttachment2(attachment, addAsNewVersion, versionPath, Context.UserName);
            return (StorageServiceStatus)status;
        }

        
        public StorageServiceStatus Upload<T>(T file,Stream data, int chunk, int totalChunks, bool addAsNewVersion) where T : BlobBase, new()
        {
            string version;
            StorageServiceStatus storageServiceStatus = StorageServiceStatus.Unknown;
            EnsureFileInfo(file);
            if(file is File)
            {
                storageServiceStatus = UploadFile(file as File, addAsNewVersion, data, chunk, totalChunks,out version);
                if(storageServiceStatus == StorageServiceStatus.Success)
                {
                    this.AddFile(file as File, true, version);
                }
            }else if (file is FileAttachment)
            {
                storageServiceStatus = UploadFile(file as FileAttachment, addAsNewVersion, data, chunk, totalChunks, out version);
                if (storageServiceStatus == StorageServiceStatus.Success)
                {
                    this.AddAttachment(file as FileAttachment, true, version);
                }
            }else if (file is FileAttachment2)
            {
                storageServiceStatus = UploadFile(file, addAsNewVersion, data, chunk, totalChunks, out version);
                if (storageServiceStatus == StorageServiceStatus.Success)
                {
                    var f = file as FileAttachment2;
                    this.AddAttachment2(f, f.IsExistingFile, version);
                }
            }

            //Post processing
            if (storageServiceStatus == StorageServiceStatus.Success)
            {
                IFileProcessor processor = Factory.GetInstance<IFileProcessor>();
                processor.ProcessAsync(file.CustomerId,file);
            }

            return storageServiceStatus;
        }

        public IEnumerable<File> GetFilesForEdit(List<File> filesToEdit)
        {
            return this.Repository.GetFilesForEdit(filesToEdit);
        }

        public IEnumerable<File> GetFiles(List<Guid> fileInternNames)
        {
            //for now it is just calling GetFilesForEdit
            IEnumerable<File> files =
                fileInternNames.Select(fileInternalName => new File() {FileInternalName = fileInternalName});
            return GetFilesForEdit(files.ToList());
        }

        public StorageServiceStatus TryUpdateFiles(List<File> filesToUpdate, out IEnumerable<File> filesNotUpdated)
        {
            Company company = this.GetFilePref();
            foreach (File file in filesToUpdate)
            {
                FileInfo info = new FileInfo(file.FileName);
                //Fix up file extensions
                if (!info.Extension.Equals(file.FileExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    file.FileName =
                        string.Concat(
                            string.IsNullOrEmpty(info.Extension)
                                ? info.Name
                                : info.Name.Replace(info.Extension, string.Empty),
                            file.FileExtension);
                }
                file.BaseFileName = GetBaseFileName(file.FileName, company.RevisionRegEx);
                EnsureFileDocNumber(file,true);
            }

            FilesInfo filesInfo = this.Repository.UpdateFiles(filesToUpdate);
            filesNotUpdated = filesInfo.DuplicateFiles;
            return (StorageServiceStatus)filesInfo.Status;
        }

        public Files GetFiles(long projectId, long folderId, string userName, FileSortBy sortBy,
                                       SortDirection sortDirection, int pageNumber, int pageSize)
        {
            Files files = this.Repository.GetFiles(projectId, folderId, userName, !this.Context.Security.CanViewAllFiles, sortBy, sortDirection,
                                            pageNumber, pageSize);

            return files;
        }

        public StorageServiceStatus DownloadToStream(Stream target, BlobBase file, IClientConnection clientConnection)
        {
            //this._blobStorageProvider.SessionProvider = this.Context.Session;
            int chunkSize = Config.GetValue<int>(ConfigKeys.DownloadChunkSize);
            return (StorageServiceStatus) _blobStorageProvider.DownloadToStream(file, target, chunkSize, clientConnection);
        }

        public FileHistory GetFileHistory(long fileId)
        {
            return this.Repository.GetFileHistory(fileId);
        }

        public FileAttachment GetFileAttachment(long fileId)
        {
            return this.Repository.GetFileAttachment(this.Context.UserName, fileId);
        }

        public void DeleteAttachment(FileAttachment fileAttachment)
        {
            DeleteAttachment(fileAttachment.FileId, 0);
            
        }

        public FileAttachment DeleteAttachment(FileAttachmentVersion fileAttachmentVersion)
        {
           return DeleteAttachment(fileAttachmentVersion.FileId, fileAttachmentVersion.RevisionNumber);
        }

        public bool DeleteFiles(long[] fileIds, long projectId)
        {
            ISecurityService securityService = ServiceFactory.GetSecurityService(this.Context.CustomerId);
            IEnumerable<File> filesInfo = securityService.TryGetFileInfo(this.Context.UserName, fileIds,  PermissionFlag.ReadWrite);
            bool allDeleted = true;
            foreach (File file in filesInfo)
            {
                File f = file;
                f.Project.ProjectId = projectId;
                if (!this.Repository.DeleteFile(this.Context.UserName, file.FileId, 0, delegate()
                    {
                        this._blobStorageProvider.Delete(f);
                        return true;
                    }))
                {
                    allDeleted = false;
                }
                IPreviewImageProvider previewImageProvider = Factory.GetInstance<IPreviewImageProvider>();
                previewImageProvider.DeletePreviews(this.Context.CustomerId, f);
            }
            return allDeleted;
        }

        public FileHistory DeleteFileVersion(long fileId, int revisionNumber)
        {
            ISecurityService securityService = ServiceFactory.GetSecurityService(this.Context.CustomerId);
            FileVersion fileVersion = securityService.TryGetFileVersionInfo(this.Context.UserName, fileId, revisionNumber,
                                                  PermissionFlag.ReadWrite);
            FileHistory fileHistory = this.Repository.DeleteFileVersion(this.Context.UserName, fileId, revisionNumber,
                                                                        delegate()
                                                                            {
                                                                                this._blobStorageProvider.Delete(fileVersion);
                                                                                return true;
                                                                            });
            return fileHistory;

        }

        public StorageServiceStatus UploadAttachment(FileAttachment fileAttachment, Stream data, int chunk, int totalChunks)
        {
            ISecurityService securityService = ServiceFactory.GetSecurityService(this.Context.CustomerId);
            IEnumerable<File> files = securityService.TryGetFileInfo(this.Context.UserName, new[] {fileAttachment.FileId}, PermissionFlag.ReadWrite);
            File f = files.FirstOrDefault();
            if(f != null)
            {
                fileAttachment.Project.BlobContainer = f.Project.BlobContainer;
                fileAttachment.Folder.FullPath = f.Folder.FullPath;
                fileAttachment.ParentFile = f.FileInternalName;
                fileAttachment.BaseFileName = GetBaseFileName(fileAttachment.FileName, null);

                return this.Upload(fileAttachment, data, chunk, totalChunks, true);
            }
            return StorageServiceStatus.Unknown;
        }

        public StorageServiceStatus UploadVersion(long fileId, long folderId, long projectId, string fileName, decimal fileSize,
                                                  Stream data, int chunk, int totalChunks)
        {
            ISecurityService securityService = ServiceFactory.GetSecurityService(this.Context.CustomerId);
            IEnumerable<File> files = securityService.TryGetFileInfo(this.Context.UserName, new[] { fileId }, PermissionFlag.ReadWrite);
            File f = files.FirstOrDefault();
            if (f != null)
            {
                f.FileName = fileName;
                f.IsExistingFile = true;
                f.FileId = fileId;
                f.FileSize = fileSize;
                f.Project.ProjectId = projectId;
                f.Folder.FolderId = folderId;
                f.Project.BlobContainer = f.Project.BlobContainer;
                f.Folder.FullPath = f.Folder.FullPath;
                f.BaseFileName = GetBaseFileName(fileName, null);

                return this.Upload(f, data, chunk, totalChunks, true);
            }
            return StorageServiceStatus.Unknown;
        }

        public StorageServiceStatus UploadComment(long projectId, long folderId, long fileId, string fileName, decimal fileSize, Stream data, int chunk, int totalChunks, string comments, bool isFromApp, string appDetails)
        {
            MarkUpFile markUpFile = GetMarkUpMetadataInfo(fileName, fileId,folderId,projectId);
            bool byAnohterUser = markUpFile.IsExistingFile && !markUpFile.CreatedByCurrentUser;
            if (!byAnohterUser)
            {
                MarkUpFile fileToUpload = new MarkUpFile()
                    {
                        FileId=fileId,
                        FileRevisionNumber = markUpFile.FileRevisionNumber,
                        ParentFile = markUpFile.ParentFile,
                        FileName = fileName,
                        Comments = comments,
                        FileSize = fileSize,
                        IsFromApp = isFromApp,
                        IsExistingFile = markUpFile.IsExistingFile,
                        ParentFileAttachmentId = markUpFile.IsExistingFile ? markUpFile.FileAttachmentId : 0,
                        AppDetails = appDetails,
                        Project = new Project(){ProjectId = projectId,BlobContainer = markUpFile.Project.BlobContainer},
                        Folder = new Folder(){FolderId = folderId,FullPath = markUpFile.Folder.FullPath}

                    };
               return this.Upload(fileToUpload, data, chunk, totalChunks, markUpFile.IsExistingFile);
            }
           
            return StorageServiceStatus.ExistingFile;
        }

        public MarkUpFile GetMarkUpMetadataInfo(string markupFileName, long fileId, long folderId,
                                                long projectId)
        {

            MarkUpFile markUpFile = this.Repository.GetMarkUpMetadataInfo(Context.UserName, markupFileName, fileId, folderId,
                                                         projectId);
            return markUpFile;
            
        }

        

        private FileAttachment DeleteAttachment(long fileId, int revisionNumber)
        {
            ISecurityService securityService = ServiceFactory.GetSecurityService(this.Context.CustomerId);
            FileAttachment attachmentToRemove = securityService.TryGetFileAttachmentInfo(this.Context.UserName, fileId, revisionNumber, PermissionFlag.ReadWrite);
            
           
            FileAttachment attachment = this.Repository.DeleteAttachment(this.Context.UserName, fileId, revisionNumber,
                                                                         delegate()
                                                                             {
                                                                                 this._blobStorageProvider.Delete(
                                                                                     attachmentToRemove);
                                                                                 return true;
                                                                             });
            return attachment;
        }

        protected static List<File> GetFilesWithAttachments(string[] fileNames, bool attachCADFilesToPdfs, string revisionReplaceRegEx)
        {
            List<string> supportedCadFileTypes = attachCADFilesToPdfs ? GetSupportedCADFileTypes() : null;
            Dictionary<string, File> files = new Dictionary<string, File>(fileNames.Length, StringComparer.InvariantCultureIgnoreCase);

            Dictionary<string, FileInfo> cadFiles = new Dictionary<string, FileInfo>(StringComparer.InvariantCultureIgnoreCase);
            foreach (string fileName in fileNames)
            {
                FileInfo fileInfo = new FileInfo(fileName);
                //just make sure we have not already added the file
                if (!files.ContainsKey(fileName))
                {
                    //Add the file with a New Guid
                    files.Add(fileName, new File() {FileName = fileName, FileInternalName = Guid.NewGuid()});

                    //Keept track of the base filename just in case we need to attach the CAD File to PDF
                    if (supportedCadFileTypes != null && supportedCadFileTypes.Contains(fileInfo.Extension))
                    {
                        cadFiles.Add(fileName, fileInfo);
                    }
                }
            }
            if (attachCADFilesToPdfs)
            {
                AttachCADFilesToPdfs(files,cadFiles);   
            }

            return files.Values.ToList();

        }

        protected IEnumerable<File> GetFileVersionInfo(long project, long folderId, List<File> filesWithAttachments, string revisionReplaceRegEx)
        {
            foreach (File filesWithAttachment in filesWithAttachments)
            {
                filesWithAttachment.BaseFileName = GetBaseFileName(filesWithAttachment.FileName,revisionReplaceRegEx);
                if (filesWithAttachment.Attachments != null)
                {
                    foreach (FileAttachment fileAttachment in filesWithAttachment.Attachments)
                    {
                        fileAttachment.BaseFileName = GetBaseFileName(fileAttachment.FileName, revisionReplaceRegEx);
                    }
                }
            }
            return this.Repository.GetUploadVersionInfo(project,folderId, filesWithAttachments);
        }
        
        protected static string ExtractBaseFileName(string fileName, string revisionReplaceRegEx)
        {
            Regex regEx = new Regex(revisionReplaceRegEx);
            bool isMatch = regEx.IsMatch(fileName);
            if (isMatch)
            {
                return regEx.Replace(fileName, string.Empty);
            }
            return fileName;

            #region Sample RegEx Code

            //string fileName = "jjjjjjjjj-REV-AA2";



            //Regex regEx = new Regex(@"\s*-\s*REV\s*-\s*(?<revision>[a-zA-Z0-9]*)$");

            //bool isMatch = regEx.IsMatch(fileName);

            //if (isMatch)
            //{

            //    Match match = regEx.Match(fileName);

            //    string baseFile = regEx.Replace(fileName, string.Empty);

            //}
            #endregion

        }

        private StorageServiceStatus UploadFile(BlobBase file, bool addAsNewVersion,Stream data, int chunk, int totalChunks, out string version)
        {
            ValidatedBlob(file);
            this._blobStorageProvider.SessionProvider = this.Context.Session;
            
            BlobStorageProviderStatus blobStorageProviderStatus = this._blobStorageProvider.UploadFile(file,
                                                                                                       addAsNewVersion,
                                                                                                       data, chunk,
                                                                                                       totalChunks,
                                                                                                       out version);
            if (blobStorageProviderStatus == BlobStorageProviderStatus.UploadCompleted)
            {
                return StorageServiceStatus.Success;
            }else if(blobStorageProviderStatus == BlobStorageProviderStatus.UploadInPorgress)
            {
                return StorageServiceStatus.UploadInProgress;    
            }
            return StorageServiceStatus.Unknown;
        }

        private static void ValidatedBlob(BlobBase blob)
        {
            if (blob.Folder == null)
            {
                throw new InvalidBlobException(blob, "Folder is not set");
            }

            if (blob.Project == null)
            {
                throw new InvalidBlobException(blob, "Project not set");
            }
            if(string.IsNullOrEmpty(blob.Project.BlobContainer))
            {
                throw new InvalidBlobException(blob, "BlobContainer is  not set");
            }

            if (string.IsNullOrEmpty(blob.Folder.FullPath))
            {
                throw new InvalidBlobException(blob,"Folder Path is not set");
            }

            if (blob is File)
            {
                if (blob.FileInternalName == Guid.Empty)
                {
                    throw new InvalidBlobException(blob, "File Id not set");
                }
            }else if (blob is FileAttachment)
            {
                if (((FileAttachment)blob).ParentFile == Guid.Empty)
                {
                    throw new InvalidBlobException(blob, "Parent File Id not set");
                }
            }
            else if (blob is FileAttachment2)
            {
                if (((FileAttachment2)blob).ParentFile == Guid.Empty)
                {
                    throw new InvalidBlobException(blob, "Parent File Id not set");
                }
            }
        }

        private Company GetFilePref()
        {
            return this.Repository.GetFilePreferences(this.Context.UserName);
        }

        private static string GetBaseFileName(string fileName, string revisionReplaceRegEx)
        {
            return GetBaseFileName(new FileInfo(fileName), revisionReplaceRegEx);
        }

        private static string GetBaseFileName(FileInfo fileInfo, string revisionReplaceRegEx)
        {
            string extension = fileInfo.Extension;
            string baseFileName = fileInfo.Name.Replace(extension, string.Empty);
            return string.IsNullOrEmpty(revisionReplaceRegEx)
                       ? string.Concat(baseFileName, extension)
                       : string.Concat(ExtractBaseFileName(baseFileName, revisionReplaceRegEx), extension);

        }

        private static void AttachCADFilesToPdfs(Dictionary<string, File> files, Dictionary<string, FileInfo> cadFiles)
        {
            foreach (KeyValuePair<string, FileInfo> cadFile in cadFiles)
            {
                string pdfFileName = cadFile.Value.Name.Replace(cadFile.Value.Extension, Constants.PdfExtension);
                //See if we got a pdf file with the same name
                if(files.ContainsKey(string.Concat(pdfFileName)))
                {
                       File file = files[pdfFileName];
                       if(file.Attachments == null)
                       {
                           file.Attachments = new List<FileAttachment>();
                       }
                       file.Attachments.Add(new FileAttachment() {FileName = cadFile.Key, ParentFile = file.FileInternalName});
                       files.Remove(cadFile.Key);
                }
            }
        }

        private static List<string > GetSupportedCADFileTypes()
        {
            string cadExt = Config.GetValue<string>(ConfigKeys.SupportedCADFileTypes);
            return !string.IsNullOrEmpty(cadExt)
                       ? cadExt.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList()
                       : new List<string>();
        }

        private static void EnsureFolderIds(IEnumerable<Folder> folders)
        {
            foreach (Folder folder in folders)
            {
                if (folder.FolderId <= 0)
                {
                    folder.FolderId = IdentityGenerator.Create(IdentityScope.Folder);
                }
            }
        }

        private static void EnsureFileInfo(BlobBase blobBase)
        {
            FileInfo fileInfo = new FileInfo(blobBase.FileName);
            if (string.IsNullOrEmpty(blobBase.ContentType))
            {
                blobBase.ContentType = MIMETypes.Current[fileInfo.Extension];
            }
            if(string.IsNullOrEmpty(blobBase.FileExtension))
            {
                blobBase.FileExtension = fileInfo.Extension;
            }
            EnsureFileDocNumber(blobBase as File);
        }

        private static void EnsureFileDocNumber(File file, bool forceUpdate=false)
        {
            if (file != null && (forceUpdate || string.IsNullOrEmpty(file.DocNumber)))
            {
                file.DocNumber = file.BaseFileName.Replace(file.FileExtension, string.Empty);
            }
        }

        private ISecurityService GetSecurityService(long customerId)
        {
            return this._securityService ?? (this._securityService = ServiceFactory.GetSecurityService(customerId));
        }
    }
}
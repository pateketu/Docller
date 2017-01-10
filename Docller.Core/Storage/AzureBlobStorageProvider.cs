using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using BlobContainerPermissions = Microsoft.WindowsAzure.Storage.Blob.BlobContainerPermissions;
using BlobContainerPublicAccessType = Microsoft.WindowsAzure.Storage.Blob.BlobContainerPublicAccessType;
using CloudBlobClient = Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient;
using CloudBlobContainer = Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer;
using CloudBlockBlob = Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob;
using File = Docller.Core.Models.File;

namespace Docller.Core.Storage
{
    public class AzureBlobStorageProvider : IBlobStorageProvider
    {
        public void CreateContainer(Project project)
        {
            //string blobContainer = string.Format(CultureInfo.InvariantCulture, StorageHelper.BlobContainerFormat,
              //                                   project.CustomerId, project.ProjectId);
            CreateContainer(project.BlobContainer);
        }

        public void CreateContainer(string containerName)
        {
            
            CloudBlobClient client = GetBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);
            container.CreateIfNotExists();
            BlobContainerPermissions permissions = container.GetPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Off;
            container.SetPermissions(permissions);
        }

        public string GetPathSeparator()
        {
            return Config.GetValue<string>(ConfigKeys.AzureFolderPathSeperator);
        }

        public IDocllerSession SessionProvider { get; set; }

        public BlobStorageProviderStatus UploadFile(BlobBase file,bool uploadAsNewversion,Stream data, int chunk, int totalChunks, out string version)
        {
            string filePath = GetFilePath(file);
            bool isExistingFile = false;
            version = string.Empty;
            if(file is File)
            {
                isExistingFile = ((File) file).IsExistingFile;
            }
            else if(file is FileAttachment)
            {
                isExistingFile = ((FileAttachment)file).IsExistingFile;
            }
            else if (file is FileAttachment2)
            {
                isExistingFile = ((FileAttachment2)file).IsExistingFile;
            }

            if (totalChunks > 0)
            {
                return UploadChunk(filePath, file, uploadAsNewversion,isExistingFile, data, chunk, totalChunks, out version);
            }
            return Upload(filePath, file.ContentType, uploadAsNewversion, isExistingFile, file, data, out version);
        }

        public BlobStorageProviderStatus DownloadToStream(BlobBase file, Stream target, int chunkSize, IClientConnection clientConnection)
        {
            string filePath = GetFilePath(file);

            DateTime? snapShot = null;
            if (file is FileVersion || file is FileAttachmentVersion)
            {
                FileVersion version = file as FileVersion;
                FileAttachmentVersion attachmentVersion = null;
                if (version == null)
                {
                   attachmentVersion = file as FileAttachmentVersion;
                }
                snapShot = version != null
                               ? new DateTime(long.Parse(version.VersionPath))
                               : new DateTime(long.Parse(attachmentVersion.VersionPath));
            }else if (file is TransmittedFileVersion)
            {
                TransmittedFileVersion transmittedFileVersion = (TransmittedFileVersion) file;
                snapShot = new DateTime(long.Parse(transmittedFileVersion.VersionPath));

            }
            ICloudBlob blob = GetCloudBlob(filePath, file.Project.BlobContainer, snapShot); 
            /*if (chunkSize == 0)
            {
                chunkSize = 1048576*2;
            }*/
            if (chunkSize == 0 || file.FileSize <= chunkSize)
            {
                blob.DownloadToStream(target);
                
                return BlobStorageProviderStatus.DownloadComplete;
            }
            long offset = 0;
            while ((offset < file.FileSize) && clientConnection.IsClientConnected)
            {
                long length = GetLength(file.FileSize, chunkSize,offset);
                blob.DownloadRangeToStream(target,offset,length);
                offset += chunkSize;
            }
            return clientConnection.IsClientConnected
                       ? BlobStorageProviderStatus.DownloadCanceled
                       : BlobStorageProviderStatus.DownloadComplete;
        }

        public void Delete(BlobBase blobBase)
        {
            if (blobBase is FileAttachmentVersion)
            {
                DeleteAttachmentVersion((FileAttachmentVersion) blobBase);
            }else if (blobBase is FileAttachment)
            {
                DeleteAttachment((FileAttachment)blobBase);    
            }else if (blobBase is File)
            {
                DeleteFile((File) blobBase);
            }else if (blobBase is FileVersion)
            {
                DeleteFile((FileVersion)blobBase);
            }
        }

        public void DeleteDirectory(string directoryPath, string blobContainer)
        {
            CloudBlobClient blobClient = GetBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);
            CloudBlobDirectory directory = container.GetDirectoryReference(directoryPath);
            var blobs = directory.ListBlobs(true);
            foreach (IListBlobItem listBlobItem in blobs)
            {
                ICloudBlob blob = container.GetBlockBlobReference(listBlobItem.Uri.ToString());
                blob.Delete();
            }

        } 

        public BlobStorageProviderStatus UploadFile(string container, string folder, string fileName, Stream data, string contentType)
        {
            string filePath = string.Format("{0}{1}{2}", folder, this.GetPathSeparator(), fileName);
            //ICloudBlob blob = this.GetBlockBlob()
            CloudBlobClient blobClient = GetBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
            ICloudBlob cloudBlob =
                blobContainer.GetBlockBlobReference(filePath);
            cloudBlob.Properties.ContentType = contentType;
            cloudBlob.UploadFromStream(data);
            return BlobStorageProviderStatus.DownloadComplete;
           
        }

        public void DownloadToStream(string container, string filePath, Stream target)
        {
            ICloudBlob blob = GetCloudBlob(filePath, container, null);
            blob.DownloadToStream(target);
        }

        #region Private Methods
       
        private void DeleteFile(FileVersion fileVersion)
        {
            if (fileVersion.Attachments != null && fileVersion.Attachments.Any())
            {
                DeleteAttachmentVersion(fileVersion.Attachments.First());
            }

            string filePath = GetFilePath(fileVersion);
            DateTime? snapShot = new DateTime(long.Parse(fileVersion.VersionPath));
            ICloudBlob blob = GetCloudBlob(filePath, fileVersion.Project.BlobContainer, snapShot);
            blob.Delete();
        }

        private void DeleteFile(File file)
        {
            if (file.Attachments != null && file.Attachments.Any())
            {
                DeleteAttachment(file.Attachments.First());
            }

            string filePath = GetFilePath(file);
            ICloudBlob blob = GetCloudBlob(filePath, file.Project.BlobContainer, null);
            blob.Delete(DeleteSnapshotsOption.IncludeSnapshots);
        }

        private void DeleteAttachmentVersion(FileAttachmentVersion attachmentVersion)
        {
            DateTime? snapShot = new DateTime(long.Parse(attachmentVersion.VersionPath));
            string filePath = GetFilePath(attachmentVersion);
            ICloudBlob blob = GetCloudBlob(filePath, attachmentVersion.Project.BlobContainer, snapShot);
            blob.Delete();
        }
        private void DeleteAttachment(FileAttachment fileAttachment)
        {
            string filePath = GetFilePath(fileAttachment);
            ICloudBlob blob = GetCloudBlob(filePath, fileAttachment.Project.BlobContainer,null);
            blob.Delete(DeleteSnapshotsOption.IncludeSnapshots);

        }

        private static long GetLength(decimal fileSize, int chunkSize, long offset)
        {
            long length;
            if ((fileSize - offset) < chunkSize)
            {
                length = (long) (fileSize - offset);
            }
            else
            {
                length = chunkSize;
            }
            return length;
        }
        private static BlobStorageProviderStatus Upload(string filePath, string contentType, bool uploadAsNewVersion, bool isExistingFile, BlobBase file, Stream data, out string versionInfo)
        {
            versionInfo = null;
            CloudBlobClient blobClient = GetBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(file.Project.BlobContainer);
            ICloudBlob cloudBlob =
                container.GetBlockBlobReference(filePath);

            cloudBlob.Properties.ContentType = contentType;
            if (uploadAsNewVersion && isExistingFile)
            {
                versionInfo = CreateSnapshot(cloudBlob, contentType);
            }
            
            cloudBlob.UploadFromStream(data);
            return BlobStorageProviderStatus.UploadCompleted;
        }

        private BlobStorageProviderStatus UploadChunk(string filePath, BlobBase file, bool uploadAsNewVersion, bool isExistingFile, Stream data, int chunk, int totalChunks, out string versionInfo)
        {
            versionInfo = null;
            CloudBlockBlob blob = GetBlockBlob(filePath, file.ContentType, file.Project.BlobContainer);
            //If its the first chunk and if we need to create a snapshot
            if (isExistingFile && uploadAsNewVersion && chunk == 0)
            {
                versionInfo = CreateSnapshot(blob, file.ContentType);
                ((BlobSessionInfo) this.SessionProvider[filePath]).SnapshotVersion = versionInfo;
            }

            string blockId =
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0:D4}", chunk)));

            blob.PutBlock(blockId, data, null);

            if (chunk == totalChunks - 1)
            {
                this.CompleteUpload(blob, file.FileInternalName.ToString(), totalChunks);
                versionInfo = ((BlobSessionInfo) this.SessionProvider[filePath]).SnapshotVersion;
                return BlobStorageProviderStatus.UploadCompleted;
            }
            return BlobStorageProviderStatus.UploadInPorgress;

        }

        private static string CreateSnapshot(ICloudBlob cloudBlob, string contentType)
        {
            string versionInfo = null;
            ICloudBlob snapShot = cloudBlob.CreateSnapshot();
            snapShot.Properties.ContentType = contentType;
            if (snapShot.SnapshotTime != null) versionInfo = snapShot.SnapshotTime.Value.Ticks.ToString();
            return versionInfo;
        }
        private string GetFilePath(BlobBase file)
        {
            FileInfo fileInfo = new FileInfo(file.FileName);
            if(file is FileAttachment)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{1}{3}{1}{4}", file.Folder.FullPath,
                                     GetPathSeparator(),
                                     ((FileAttachment)file).ParentFile, StorageHelper.CadAttachmentsPath, file.FileName);
            }else if (file is MarkUpFile)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{1}{3}{1}{4}{1}{5}", file.Folder.FullPath,
                                    GetPathSeparator(),
                                    ((MarkUpFile)file).ParentFile,((MarkUpFile)file).FileRevisionNumber, StorageHelper.MarkUpPath, file.FileName);
            }
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", file.Folder.FullPath, GetPathSeparator(),
                                  file.FileInternalName,fileInfo.Extension);
        }
        private void CompleteUpload(CloudBlockBlob blockBlob,string fileName, int totalChunks)
        {
            List<string> blockList = new List<string>(totalChunks);

            for (int i = 0; i < totalChunks; i++)
            {
                blockList.Add(Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0:D4}", i))));
            }
            /*var blockList =
                    Enumerable.Range(0, totalChunks-1).ToList().ConvertAll(
                        rangeElement =>
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0:D4}", rangeElement))));*/

            blockBlob.PutBlockList(blockList);
            // blockBlob.SetProperties();
            this.SessionProvider.Remove(fileName); 
        }

        private ICloudBlob GetCloudBlob(string file, string blobContainer, DateTime? snapshotTime)
        {
            CloudBlockBlob cloudBlob = null;
            if (this.SessionProvider != null && this.SessionProvider[file] != null)
            {
                BlobSessionInfo blobSessionInfo = this.SessionProvider[file] as BlobSessionInfo;
                if (blobSessionInfo != null)
                {
                    cloudBlob = blobSessionInfo.BlockBlob;
                }
            }

            if (cloudBlob == null)
            {
                CloudBlobClient blobClient = GetBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);
                DateTimeOffset? offset = null;
                if (snapshotTime != null)
                {
                    offset = new DateTimeOffset(snapshotTime.Value);
                }
                cloudBlob =
                    container.GetBlockBlobReference(file, offset);

                if(this.SessionProvider != null)
                    this.SessionProvider[file] = new BlobSessionInfo() { BlockBlob = cloudBlob };
            }
            return cloudBlob;
        }
        private CloudBlockBlob GetBlockBlob(string file, string contentType, string blobContainer)
        {
            CloudBlockBlob cloudBlob = null;
            if (this.SessionProvider !=null && this.SessionProvider[file] != null)
            {
                BlobSessionInfo blobSessionInfo = this.SessionProvider[file] as BlobSessionInfo;
                if (blobSessionInfo != null)
                {
                    cloudBlob = blobSessionInfo.BlockBlob;
                }
            }

            if(cloudBlob == null)
            {
                CloudBlobClient blobClient = GetBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);
                cloudBlob =
                    container.GetBlockBlobReference(file);

                cloudBlob.Properties.ContentType = contentType;

                if (this.SessionProvider != null)
                    this.SessionProvider[file] = new BlobSessionInfo() {BlockBlob = cloudBlob};
            }
            return cloudBlob;
        } 

        private static CloudBlobClient GetBlobClient()
        {
            CloudStorageAccount cloudStorageAccount = StorageHelper.StorageAccount;
            return cloudStorageAccount.CreateCloudBlobClient();
        }
#endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docller.Core.Common;
using Docller.Core.Storage;
using File = Docller.Core.Models.File;
using Docller.Core.Models;

namespace Docller.Core.Services
{
    public class DownloadService : IDownloadService
    {
        private  class ObjectState
        {
            public string UserName;
            public long CustomerId;
            public long ProjectId;
            public long[] FileIds;
            public int RevisionNumber;
            public long TransmittalId;
            public string DownloadedBy;
            public IClientConnection ClientConnection;

        }

        public Task<DownloadState> DownloadAsync(string userName, long customerId, IClientConnection clientConnection, long fileId, int version)
        {
            ObjectState objectState = new ObjectState()
            {
                UserName = userName,
                CustomerId = customerId,
                FileIds = new[] {fileId},
                RevisionNumber = version,
                ClientConnection = clientConnection
            };
            TaskFactory<DownloadState> taskFactory = new TaskFactory<DownloadState>();
            Task<DownloadState> task = taskFactory.StartNew(DownloadVersion, objectState, clientConnection.ClientCancellationToken,
                                 TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return task;
        }

        public Task<DownloadState> DownloadTransmittalAsync(string userName, long customerId, IClientConnection clientConnection, long projectId, long transmittalId)
        {
            ObjectState objectState = new ObjectState()
            {
                UserName = userName,
                CustomerId = customerId,
                TransmittalId = transmittalId,
                ProjectId = projectId,
                ClientConnection = clientConnection
            };
            TaskFactory<DownloadState> taskFactory = new TaskFactory<DownloadState>();
            Task<DownloadState> task = taskFactory.StartNew(DownloadTransmittal, objectState, clientConnection.ClientCancellationToken,
                                 TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return task;
        }

        public Task<DownloadState> DownloadSharedFilesAsync(long customerId, IClientConnection clientConnection, long projectId,
            long transmittalId, string downloadedBy)
        {
            ObjectState objectState = new ObjectState()
            {
                CustomerId = customerId,
                TransmittalId = transmittalId,
                ProjectId = projectId,
                ClientConnection = clientConnection,
                DownloadedBy = downloadedBy
            };

            TaskFactory<DownloadState> taskFactory = new TaskFactory<DownloadState>();
            Task<DownloadState> task = taskFactory.StartNew(DownloadShared, objectState, clientConnection.ClientCancellationToken,
                                 TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return task;

        }

        public Task<DownloadState> DownloadAsync(string userName, long customerId, IClientConnection clientConnection, params long[] fileIds)
        {
            ObjectState objectState = new ObjectState()
                {
                    UserName = userName,
                    CustomerId = customerId,
                    FileIds = fileIds,
                    ClientConnection = clientConnection
                };
            TaskFactory<DownloadState> taskFactory = new TaskFactory<DownloadState>();
            Task<DownloadState> task = taskFactory.StartNew(Download, objectState, clientConnection.ClientCancellationToken,
                                 TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return task;
            //AsyncOperation asyncOperation = AsyncOperationManager.CreateOperation(null);
            //new Thread(() => Download(userName,customerId,clientConnection,fileIds, asyncOperation)).Start();

        }

        private DownloadState Download(object state)
        {
            ObjectState objectState = (ObjectState) state;
            IDownloadProvider downloadProvider = null;
            Exception exception = null;
            try
            {
                //Get the files from security service this ensures that user requesting the files have access to the files
                ISecurityService securityService = ServiceFactory.GetSecurityService(objectState.CustomerId);
                IEnumerable<File> files = securityService.TryGetFileInfo(objectState.UserName, objectState.FileIds, PermissionFlag.Read);
                if (files != null)
                {
                    List<File> f = files.ToList();

                    if (f.Count == 1 &&
                        (f.First().Attachments == null || f.First().Attachments.Count == 0))
                    {
                        downloadProvider = new SingleFileDownloadProvider(f.First(), objectState.CustomerId);
                    }
                    else
                    {
                        string fileName = GetCompressedFileName(f);
                        downloadProvider = new MultipleFileDownloadProvider(f, fileName, objectState.CustomerId);
                    }

                    downloadProvider.PrepareDownload(objectState.ClientConnection);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                if (downloadProvider != null)
                {
                    downloadProvider.CleanUp();
                }
            }
            return new DownloadState()
                {
                    DownloadProvider = downloadProvider, Exception = exception
                };

        }

        private DownloadState DownloadVersion(object state)
        {
            ObjectState objectState = (ObjectState)state;
            IDownloadProvider downloadProvider = null;
            Exception exception = null;
            try
            {
                //Get the files from security service this ensures that user requesting the files have access to the files
                ISecurityService securityService = ServiceFactory.GetSecurityService(objectState.CustomerId);
                FileVersion fileVersion = securityService.TryGetFileVersionInfo(objectState.UserName, objectState.FileIds.First(), objectState.RevisionNumber, PermissionFlag.Read);
                if (fileVersion != null)
                {
                    if (fileVersion.Attachments == null || fileVersion.Attachments.Count == 0)
                    {
                        downloadProvider = new SingleFileDownloadProvider(fileVersion, objectState.CustomerId);
                    }
                    else
                    {
                        string fileName = GetCompressedFileName(fileVersion);
                        downloadProvider = new FileVersionDownloader(fileVersion, fileName, objectState.CustomerId);
                    }
                    downloadProvider.PrepareDownload(objectState.ClientConnection);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            return new DownloadState()
            {
                DownloadProvider = downloadProvider,
                Exception = exception
            };

        }

        private DownloadState DownloadTransmittal(object state)
        {
            ObjectState objectState = (ObjectState)state;
            IDownloadProvider downloadProvider = null;
            Exception exception = null;
            try
            {
                ISecurityService securityService = ServiceFactory.GetSecurityService(objectState.CustomerId);
                Transmittal transmittal = securityService.TryGetTransmittalInfo(objectState.UserName, objectState.CustomerId, objectState.ProjectId, objectState.TransmittalId);
                if (transmittal != null)
                {
                    List<TransmittedFile> f = transmittal.Files;

                    if(f.Count > 0)
                    {
                        string fileName = GetCompressedFileName("Issued",transmittal);
                        downloadProvider = new TransmittalDownloadProvider(f,fileName,objectState.CustomerId);
                        downloadProvider.PrepareDownload(objectState.ClientConnection);
                    }

                    
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                if (downloadProvider != null)
                {
                    downloadProvider.CleanUp();
                }
            }

            return new DownloadState()
            {
                DownloadProvider = downloadProvider,
                Exception = exception
            };
        }

        private DownloadState DownloadShared(object state)
        {
            ObjectState objectState = (ObjectState)state;
            IDownloadProvider downloadProvider = null;
            Exception exception = null;
            try
            {
                ITransmittalService transmittalService = ServiceFactory.GetTransmittalService(objectState.CustomerId);
                Transmittal transmittal = transmittalService.GetSharedFiles(objectState.CustomerId,
                    objectState.ProjectId, objectState.TransmittalId, objectState.DownloadedBy);
                if (transmittal != null)
                {
                    List<TransmittedFile> f = transmittal.Files;

                    if (f.Count > 0)
                    {
                        string fileName = GetCompressedFileName("Shared", transmittal);
                        downloadProvider = new TransmittalDownloadProvider(f, fileName, objectState.CustomerId);
                        downloadProvider.PrepareDownload(objectState.ClientConnection);
                    }


                }
            }
            catch (Exception ex)
            {
                exception = ex;
                if (downloadProvider != null)
                {
                    downloadProvider.CleanUp();
                }
            }
            return new DownloadState()
            {
                DownloadProvider = downloadProvider,
                Exception = exception
            };
        }
        
        protected virtual string GetCompressedFileName(IEnumerable<File> files)
        {
            return files.Count() == 1
                       ? GetCompressedFileName(files.First())
                       : files.First().Folder.FolderName;
        }

        protected virtual string GetCompressedFileName(BlobBase blob)
        {
            return blob.FileName.Remove(blob.FileName.IndexOf("."));
        }

        protected virtual string GetCompressedFileName(string prefix, Transmittal transmittal)
        {
            //return transmittal.t
            return string.Format("{0}_{1}", prefix, transmittal.TransmittalId);
        }
        
       
    }
}

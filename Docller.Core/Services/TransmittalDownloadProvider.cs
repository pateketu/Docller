using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Models;
using File = Docller.Core.Models.File;

namespace Docller.Core.Services
{
    public class TransmittalDownloadProvider : MultipleFileDownloadProvider
    {
        private readonly IEnumerable<TransmittedFile> _transmittedFiles;
        private string _cacheFolder;
        public TransmittalDownloadProvider(IEnumerable<TransmittedFile> transmittedFiles, string fileName, long customerId) : base(fileName, customerId)
        {
            _transmittedFiles = transmittedFiles;
        }

        protected override string LocalFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_cacheFolder))
                {
                    ILocalStorage localStorage = Factory.GetLocalStorageProvider();
                    _cacheFolder = localStorage.EnsureCacheFolder(Constants.TransmittalFilesCacheFolder);
                }
                return _cacheFolder;
            }
        }

        protected override void DownloadFiles(ZipArchive archive, IClientConnection clientConnection)
        {
            foreach (TransmittedFile transmittedFile in _transmittedFiles)
            {

                ZipArchiveEntry entry = archive.CreateEntry(transmittedFile.FileName, CompressionLevel.Optimal);
                using (Stream stream = entry.Open())
                {
                    try
                    {
                        StorageService.DownloadToStream(stream, transmittedFile, clientConnection);
                    }
                    catch (Exception exception)
                    {
                        Logger.Warn(
                            "Eror {0} in attempting to Download file {1}, File Internal Name {2}, Folder: {3}, Blob Container: {4}",
                            exception.Message, transmittedFile.FileName, transmittedFile.FileInternalName,
                            transmittedFile.Folder != null ? transmittedFile.Folder.FullPath : "No Folder found",
                            transmittedFile.Project != null ? transmittedFile.Project.BlobContainer : "No Contrainer");
                        throw;
                    }
                }
                if (transmittedFile.Attachments != null && transmittedFile.Attachments.Count == 1)
                {
                    try
                    {
                        DownloadAttachment(transmittedFile.Attachments.First(), archive, clientConnection);
                    }
                    catch (Exception exception)
                    {
                        Logger.Warn(
                            "Eror {0} in attempting to Download Attachment {1}, Folder Path {2}, BlobContainer {3}",
                            exception.Message, transmittedFile.Attachments.First().FileName,
                            transmittedFile.Attachments.First().Folder != null
                                ? transmittedFile.Attachments.First().Folder.FullPath
                                : "No Folder found",
                            transmittedFile.Attachments.First().Project != null
                                ? transmittedFile.Attachments.First().Project.BlobContainer
                                : "No Contrainer");
                        throw;
                    }
                }
            }
        }
        public override void PrepareDownload(IClientConnection clientConnection)
        {
            if (!System.IO.File.Exists(string.Format("{0}\\{1}", LocalFolder, FileName)))
            {
                base.PrepareDownload(clientConnection);
            }

        }
    }
}
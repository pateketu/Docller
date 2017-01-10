using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Models;
using StructureMap;
using File = Docller.Core.Models.File;

namespace Docller.Core.Services
{

    public class MultipleFileDownloadProvider : IDownloadProvider
    {
        private readonly IEnumerable<File> _files;
        private readonly long _customerId;
        
        private string _tempFolder;

        protected MultipleFileDownloadProvider(string fileName, long customerId)
        {
            _customerId = customerId;
            this.FileName = string.Format("{0}.zip", fileName);
            this.ContentType = MIMETypes.Current[".zip"];
            StorageService = ServiceFactory.GetStorageService(_customerId);
        }
        public MultipleFileDownloadProvider(IEnumerable<File> files, string fileName, long customerId):this(fileName,customerId)
        {
            _files = files;
           
        }
        protected IStorageService StorageService { get; set; }
        protected virtual string LocalFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_tempFolder))
                {
                    ILocalStorage localStorage = Factory.GetLocalStorageProvider();
                    _tempFolder = localStorage.CreateTempFolder();
                }
                return _tempFolder;    
            }
            
        }
        public string ContentType { get; private set; }
        public string FileName { get; private set; }
       
        public DownloadStatus DownloadToStream(Stream target, IClientConnection clientConnection)
        {

            clientConnection.TransmitFile(string.Format("{0}\\{1}", LocalFolder, this.FileName));
            return DownloadStatus.DownloadCanceled;
        }
       
        public virtual void PrepareDownload(IClientConnection clientConnection)
        {
            if (clientConnection.IsClientConnected)
            {
                using (FileStream zipToOpen = new FileStream(GetZipFileName(), FileMode.Create))
                {
                    using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                    {
                        DownloadFiles(archive, clientConnection);
                    }
                }
            }
        }

        public virtual void CleanUp()
        {
            string zipFile = GetZipFileName();
            if (System.IO.File.Exists(zipFile))
            {
                System.IO.File.Delete(zipFile);
            }
        }


        protected virtual void DownloadFiles(ZipArchive archive, IClientConnection clientConnection)
        {
            foreach (File file in _files)
            {
                ZipArchiveEntry entry = archive.CreateEntry(file.FileName, CompressionLevel.Optimal);
                using (Stream stream = entry.Open())
                {
                    try
                    {
                        StorageService.DownloadToStream(stream, file, clientConnection);
                    }
                    catch (Exception exception)
                    {
                        Logger.Warn(
                            "Eror {0} in attempting to Download file {1}, File Internal Name {2}, Folder: {3}, Blob Container: {4}",
                            exception.Message, file.FileName, file.FileInternalName,
                            file.Folder != null ? file.Folder.FullPath : "No Folder found",
                            file.Project != null ? file.Project.BlobContainer : "No Contrainer");
                        throw;
                    }

                    if (file.Attachments != null && file.Attachments.Count == 1)
                    {
                        try
                        {
                            DownloadAttachment(file.Attachments.First(), archive, clientConnection);
                        }
                        catch (Exception exception)
                        {
                            Logger.Warn(
                                "Eror {0} in attempting to Download Attachment {1}, Folder Path {2}, BlobContainer {3}",
                                exception.Message, file.Attachments.First().FileName,
                                file.Attachments.First().Folder != null
                                    ? file.Attachments.First().Folder.FullPath
                                    : "No Folder found",
                                file.Attachments.First().Project != null
                                    ? file.Attachments.First().Project.BlobContainer
                                    : "No Contrainer");
                            throw;
                        }
                    }
                }
            }
        }

        protected virtual void  DownloadAttachment(FileAttachment fileAttachment, ZipArchive zipArchive, IClientConnection clientConnection)
        {
            string attachment = string.Format("Attachments\\{0}", fileAttachment.FileName);
            ZipArchiveEntry entry = zipArchive.CreateEntry(attachment, CompressionLevel.Optimal);
            using (Stream stream = entry.Open())
            {
               // fileAttachment.
                StorageService.DownloadToStream(stream, fileAttachment, clientConnection);
            }
        }

        protected virtual string GetZipFileName()
        {
            return string.Format("{0}\\{1}", LocalFolder, this.FileName);
        }

    }
}
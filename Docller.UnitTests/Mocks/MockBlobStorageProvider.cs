using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Storage;

namespace Docller.Tests.Mocks
{
    public class MockBlobStorageProvider : IBlobStorageProvider
    {
        #region Implementation of IBlobStorageProvider

        public void CreateContainer(Project project)
        {
            
        }

        public void CreateContainer(string containerName)
        {
            throw new NotImplementedException();
        }

        public string GetPathSeparator()
        {
            return "/";
        }

        public IDocllerSession SessionProvider { get; set; }
        public BlobStorageProviderStatus UploadFile(BlobBase file, bool uploadAsNewversion, byte[] data, int chunk, int totalChunks, out string version)
        {
            throw new NotImplementedException();
        }

        public BlobStorageProviderStatus UploadFile(BlobBase file, bool uploadAsNewversion, Stream data, int chunk, int totalChunks,
                                                    out string version)
        {
            throw new NotImplementedException();
        }

        public BlobStorageProviderStatus DownloadToStream(BlobBase file, Stream target, int chunkSize,
                                                          IClientConnection clientConnection)
        {
            throw new NotImplementedException();
        }

        public void Delete(BlobBase blobBase)
        {
            throw new NotImplementedException();
        }

        public void DeleteDirectory(string directoryPath, string blobContainer)
        {
            throw new NotImplementedException();
        }

        public BlobStorageProviderStatus UploadFile(string container, string folder, string fileName, Stream data, string contentType)
        {
            return BlobStorageProviderStatus.UploadCompleted;
        }

        public void DownloadToStream(string container, string filePath, Stream target)
        {
            throw new NotImplementedException();
        }

        public BlobStorageProviderStatus UploadFile(BlobBase file, bool uploadAsNewversion, byte[] data, int chunk, int totalChunks)
        {
            return BlobStorageProviderStatus.UploadCompleted;
        }

        #endregion
    }
}

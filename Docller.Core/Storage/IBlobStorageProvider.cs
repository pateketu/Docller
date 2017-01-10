using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Common.DataStructures;
using Docller.Core.Models;
using Docller.Core.Services;

namespace Docller.Core.Storage
{
    public enum BlobStorageProviderStatus
    {
        Unknown=0,
        UploadInPorgress=1,
        UploadCompleted=2,
        DownloadComplete=4,
        DownloadCanceled= 5

    }


    public interface IBlobStorageProvider
    {
        void CreateContainer(Project project);
        void CreateContainer(string containerName);
        string GetPathSeparator();
        IDocllerSession SessionProvider { get; set; }
        BlobStorageProviderStatus UploadFile(BlobBase file, bool uploadAsNewversion, Stream data, int chunk, int totalChunks, out string version);
        BlobStorageProviderStatus DownloadToStream(BlobBase file, Stream target, int chunkSize, IClientConnection clientConnection);
        void Delete(BlobBase blobBase);
        void DeleteDirectory(string directoryPath, string blobContainer);
        BlobStorageProviderStatus UploadFile(string container, string folder, string fileName, Stream data, string contentType);
        void DownloadToStream(string container, string filePath, Stream target);
    }
}

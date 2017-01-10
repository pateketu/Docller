using System.IO;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Services;

namespace Docller.Core.Images
{
    public interface IDirectDownloadProvider
    {
        string GetFileFromStorage(long customerId, BlobBase blobBase);
    }

    public class DirectDownloadProvider : IDirectDownloadProvider
    {
        public string GetFileFromStorage(long customerId, BlobBase blobBase)
        {
            string tempFile = GetTempFileName(blobBase.FileName);
            using (FileStream stream = new FileStream(tempFile, FileMode.Create, FileAccess.ReadWrite))
            {
                IStorageService storageService = ServiceFactory.GetStorageService(customerId);
                storageService.DownloadToStream(stream, blobBase, new NullClientConnection());
                stream.Flush(true);
            }
            return tempFile;
        }

        private string GetTempFileName(string fileName)
        {
            string tempFolder = Factory.GetInstance<ILocalStorage>().CreateTempFolder();
            return string.Format("{0}\\{1}", tempFolder, fileName);
        }
    }
}
using System.IO;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.Core.Services
{
    public class SingleFileDownloadProvider : IDownloadProvider
    {
        private readonly BlobBase _blob;
        private readonly long _customerId;

        public SingleFileDownloadProvider(BlobBase blob, long customerId)
        {
            _blob = blob;
            _customerId = customerId;
            this.FileName = blob.FileName;
            this.ContentType = blob.ContentType;
        }
       
        public string ContentType { get; private set; }
        public string FileName { get; private set; }
        public void PrepareDownload(IClientConnection clientConnection)
        {
            //nothing to do
        }

        public void CleanUp()
        {
            //nothing to clean up
        }

        public DownloadStatus DownloadToStream(Stream target, IClientConnection clientConnection)
        {
            IStorageService storageService = ServiceFactory.GetStorageService(_customerId);
            return (DownloadStatus)storageService.DownloadToStream(target, _blob, clientConnection);
        }

    }
}
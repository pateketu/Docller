using System.IO;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Storage;
using Docller.UI.PdfViews;

namespace Docller.Core.Services
{
    public class IssueSheetProvider : IIssueSheetProvider
    {
        public virtual string Create(IssueSheet data)
        {
            string folder = GetLocalCacheFolder();
            string fileName = Utils.GetIssueSheetFileName(data.TransmittalId);
            string fullFileName = string.Format("{0}\\{1}", folder, fileName);
            IssueSheetView pdfView = new IssueSheetView(data);
            pdfView.Create(fullFileName);
            //cache the file
            CacheIssueSheet(fullFileName,data);
            return fullFileName;
        }

        public bool TryGetFromCache(long transmittalId, out string cachedFile)
        {
            string folder = GetLocalCacheFolder();
            string fileName = Utils.GetIssueSheetFileName(transmittalId);
            string fullFileName = string.Format("{0}\\{1}", folder, fileName);
            FileInfo info = new FileInfo(fullFileName);
            if (info.Exists)
            {
                cachedFile = info.FullName;
                return true;
            }
            cachedFile = null;
            return false;
        }


        protected virtual void CacheIssueSheet(string fullFileName, IssueSheet data)
        {
            FileInfo info = new FileInfo(fullFileName);
            using (FileStream stream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read))
            {
                IBlobStorageProvider storageProvider = Factory.GetInstance<IBlobStorageProvider>();
                storageProvider.UploadFile(data.BlobContainer, Constants.IssueSheetCacheFolder, info.Name, stream,
                                           MIMETypes.Current[".pdf"]);
            }
        }

        private string GetLocalCacheFolder()
        {
            ILocalStorage localStorage = Factory.GetLocalStorageProvider();
            return localStorage.EnsureCacheFolder(Constants.IssueSheetCacheFolder);
            
        }


    }
}
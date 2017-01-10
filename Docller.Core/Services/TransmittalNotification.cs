using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Docller.Core.Common;
using Docller.Core.Infrastructure;
using Docller.Core.Models;
using Docller.Core.Storage;
using Docller.UI.PdfViews;

namespace Docller.Core.Services
{

    public interface ITransmittalNotification
    {
        void Notify(Transmittal transmittal, string generatedIssueSheetFile);
    }

    public class TransmittalNotification:ITransmittalNotification
    {
       
       

       /* private void AsyncStart(object state)
        {
            CreateTransmittalInfo createTransmittalInfo = (CreateTransmittalInfo) state;
            string tempFolder = this._localStorage.CreateTempFolder();
            string fileName = string.Format("IssueSheet_{0}.pdf", createTransmittalInfo.IssueSheetData.TransmittalId);

            string fullFileName = string.Format("{0}\\{1}", tempFolder,fileName);
            CreateIssueSheet(fullFileName);
            using (FileStream stream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read))
            {
                Cache(fileName, stream);
            }
            
        }

        private void CreateIssueSheet(string fileName)
        {
            IssueSheetView issueSheetView = new IssueSheetView(this._createTransmittalInfo.IssueSheetData);
            issueSheetView.Create(fileName);
            
        }

        private void Cache(string fileName, Stream fileData)
        {
            _blobStorageProvider.UploadFile(_createTransmittalInfo.IssueSheetData.BlobContainer,
                                            TransmittalService.IssueSheetFolder, fileName, fileData,
                                            MIMETypes.Current[".pdf"]);
        }

        private */
        public virtual void Notify(Transmittal transmittal, string issueSheetFile)
        {
            
        }
    }
}
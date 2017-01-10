using System.Collections.Generic;
using System.IO;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.Infrastructure;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.UI.PdfViews;
using StructureMap;
using File = Docller.Core.Models.File;

namespace Docller.Core.Services
{
    public class TransmittalService : ServiceBase<ITransmittalRepository>, ITransmittalService
    {
        internal const string IssueSheetFolder = "IssueSheets";
        #region Implementation of ITransmittalService

        public TransmittalService(ITransmittalRepository repository) : base(repository)
        {
        }

        public virtual TransmittalCreationInfo CreateTransmittal(Transmittal transmittal, IEnumerable<SubscriberItem> to, IEnumerable<SubscriberItem> cc)
        {
            if (transmittal.TransmittalId == 0)
            {
                transmittal.TransmittalId = IdentityGenerator.Create(IdentityScope.Transmittal, this.Context.CustomerId);
            }
            transmittal.CustomerId = this.Context.CustomerId;
            IEnumerable<SubscriberItem> subscriberItems = to as IList<SubscriberItem> ?? to.ToList();
            IEnumerable<SubscriberItem> enumerable = cc != null ? cc as IList<SubscriberItem> ?? cc.ToList() : null;
            DistributionExtractor distribution = new DistributionExtractor(this.Context.CustomerId,transmittal.ProjectId,subscriberItems,enumerable);
            transmittal.Distribution = distribution.Extract(); 
            CreateTransmittalInfo info  = this.Repository.CreateTransmittal(transmittal);

            TransmittalCreationInfo transmittalCreationInfo = new TransmittalCreationInfo()
            {
                IssueSheetData = info.IssueSheetData,
                Status = (TransmittalServiceStatus)info.Status,
                Transmittal = info.Transmittal

            };
          
            return transmittalCreationInfo;
        }

        public Transmittal GetTransmittal(long projectId, long transmittalId)
        {
            return this.Repository.GetTransmittal(projectId,transmittalId);
        }

        public Stream GetIssueSheet(long transmittalId, bool onlyFromCache)
        {
            IIssueSheetProvider issueSheetProvider = Factory.GetInstance<IIssueSheetProvider>();
            string fileName;
            if (!issueSheetProvider.TryGetFromCache(transmittalId, out fileName) && !onlyFromCache)
            {
                IssueSheet issueSheet = this.Repository.GetIssueSheet(this.Context.UserName, transmittalId);
                fileName = issueSheetProvider.Create(issueSheet);
            }

            return string.IsNullOrEmpty(fileName) ? null : new FileStream(fileName, FileMode.Open, FileAccess.Read);

        }

        public SharedFilesInfo ShareFiles(long projectId, long folderId, long[] fileIds, string[] emails, string message)
        {
            long transmittalId = IdentityGenerator.Create(IdentityScope.Transmittal, this.Context.CustomerId);
            SharedFilesInfo info = new SharedFilesInfo() {TransmittalId = transmittalId};
            info.Files = this.Repository.ShareFiles(this.Context.CustomerId, transmittalId, projectId, message,
                this.Context.UserName,
                fileIds, emails);
            return info;
        }

        public Transmittal GetSharedFiles(long customerId, long projectId, long transmittalId, string downloadedBy)
        {
            ITransmittalRepository transmittalRepository = Factory.GetRepository<ITransmittalRepository>(customerId);
            return transmittalRepository.GetSharedFiles(projectId, transmittalId, downloadedBy);
        }

        public PageableData<Transmittal> GetMyTransmittals(long projectId, bool showDraft, int pageNumber, int pageSize)
        {
            return this.Repository.GetMyTransmittals(this.Context.UserName, projectId, showDraft, pageNumber, pageSize);
        }

        public PageableData<Transmittal> GetTransmittalsForMe(long projectId, int pageNumber, int pageSize)
        {
            return this.Repository.GetTransmittalsForMe(this.Context.UserName, projectId, pageNumber, pageSize);
        }

        public PageableData<Transmittal> GetTransmittalsForMyCompany(long projectId, int pageNumber, int pageSize)
        {
            return this.Repository.GetTransmittalsForMyCompany(this.Context.UserName, projectId, pageNumber, pageSize);
        }

        #endregion


        
       
    }
}
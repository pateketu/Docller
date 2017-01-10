using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Docller.Core.Models;
using Docller.Core.Services;
using File = Docller.Core.Models.File;

namespace Docller.UnitTests.Mocks
{
    public class MockTransmittalService:ITransmittalService 
    {
        public TransmittalCreationInfo CreateTransmittal(Transmittal transmittal, IEnumerable<SubscriberItem> to, IEnumerable<SubscriberItem> cc)
        {
            return new TransmittalCreationInfo {Status = TransmittalServiceStatus.Success};

        }

        public Transmittal GetTransmittal(long projectId, long transmittalId)
        {
            return new Transmittal();
        }

        public Stream GetIssueSheet(long transmittalId, bool onlyFromCache)
        {
            throw new NotImplementedException();
        }

        public SharedFilesInfo ShareFiles(long projectId, long folderId, long[] fileIds, string[] emails, string message)
        {
            throw new NotImplementedException();
        }

        public Transmittal GetSharedFiles(long customerId, long projectId, long transmittalId, string downloadedBy)
        {
            throw new NotImplementedException();
        }

        public PageableData<Transmittal> GetMyTransmittals(long projectId, bool showDraft, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public PageableData<Transmittal> GetTransmittalsForMe(long projectId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public PageableData<Transmittal> GetTransmittalsForMyCompany(long projectId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transmittal> GetMyTransmittals(string userName, long projectId, bool showDraft, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transmittal> GetTransmittalsForMe(string userName, long projectId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transmittal> GetTransmittalsForMyCompany(string userName, long projectId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }


        public string CreateIssueSheet(IssueSheet issueSheetData)
        {
            throw new NotImplementedException();
        }
    }
}

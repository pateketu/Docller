using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Infrastructure;
using Docller.Core.Models;
using Docller.Core.Repository;

namespace Docller.UnitTests.Mocks
{
    public class MockTransmittalRepository: ITransmittalRepository
    {
        public CreateTransmittalInfo CreateTransmittal(Transmittal transmittal)
        {
            return new CreateTransmittalInfo();
        }

        public Transmittal GetTransmittal(long projectId, long transmittalId)
        {
            throw new NotImplementedException();
        }

        public IssueSheet GetIssueSheet(string usernName, long transmittalId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<File> ShareFiles(long customerId, long transmittalId, long projectId, string message, string userName,
            long[] fileIds, string[] emails)
        {
            throw new NotImplementedException();
        }

        public Transmittal GetSharedFiles(long projectId, long transmittalId, string downloadedBy)
        {
            throw new NotImplementedException();
        }

        public PageableData<Transmittal> GetMyTransmittals(string userName, long projectId, bool showDraft, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public PageableData<Transmittal> GetTransmittalsForMe(string userName, long projectId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public PageableData<Transmittal> GetTransmittalsForMyCompany(string userName, long projectId, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}

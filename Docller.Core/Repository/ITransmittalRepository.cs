using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Infrastructure;
using Docller.Core.Models;

namespace Docller.Core.Repository
{
    public interface ITransmittalRepository:IRepository
    {
        CreateTransmittalInfo CreateTransmittal(Transmittal transmittal);
        Transmittal GetTransmittal(long projectId, long transmittalId);
        IssueSheet GetIssueSheet(string usernName, long transmittalId);
        IEnumerable<File> ShareFiles(long customerId, long transmittalId, long projectId, string message, string userName, long[] fileIds, string[] emails);
        Transmittal GetSharedFiles(long projectId, long transmittalId, string downloadedBy);
        PageableData<Transmittal> GetMyTransmittals(string userName, long projectId, bool showDraft, int pageNumber, int pageSize);
        PageableData<Transmittal> GetTransmittalsForMe(string userName, long projectId, int pageNumber, int pageSize);
        PageableData<Transmittal> GetTransmittalsForMyCompany(string userName, long projectId, int pageNumber, int pageSize);
    }
}

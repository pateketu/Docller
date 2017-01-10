using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Docller.Core.Models;
using File = Docller.Core.Models.File;

namespace Docller.Core.Services
{
    public interface ITransmittalService
    {
        TransmittalCreationInfo CreateTransmittal(Transmittal transmittal, IEnumerable<SubscriberItem> to, IEnumerable<SubscriberItem> cc);
        Transmittal GetTransmittal(long projectId, long transmittalId);
        Stream GetIssueSheet(long transmittalId, bool onlyFromCache);
        SharedFilesInfo ShareFiles(long projectId, long folderId, long[] fileIds, string[] emails, string message);
        Transmittal GetSharedFiles(long customerId, long projectId, long transmittalId, string downloadedBy);
        PageableData<Transmittal> GetMyTransmittals(long projectId, bool showDraft, int pageNumber, int pageSize);
        PageableData<Transmittal> GetTransmittalsForMe(long projectId, int pageNumber, int pageSize);
        PageableData<Transmittal> GetTransmittalsForMyCompany(long projectId, int pageNumber, int pageSize);
    }
}

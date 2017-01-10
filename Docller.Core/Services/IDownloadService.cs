using System.Threading.Tasks;
using Docller.Core.Common;

namespace Docller.Core.Services
{
    public interface IDownloadService
    {
        Task<DownloadState> DownloadAsync(string userName, long customerId, IClientConnection clientConnection, params long[] fileIds);
        Task<DownloadState> DownloadAsync(string userName, long customerId, IClientConnection clientConnection, long fileId, int version);

        Task<DownloadState> DownloadTransmittalAsync(string userName, long customerId, IClientConnection clientConnection,
                                           long projectId, long transmittalId);

        Task<DownloadState> DownloadSharedFilesAsync(long customerId, IClientConnection clientConnection,
                                           long projectId, long transmittalId, string downloadedBy);

    }
}
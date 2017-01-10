using Docller.Core.Models;

namespace Docller.Core.Services
{
    public interface IIssueSheetProvider
    {
        string Create(IssueSheet data);
        bool TryGetFromCache(long transmittalId, out string cachedFile);
    }
}
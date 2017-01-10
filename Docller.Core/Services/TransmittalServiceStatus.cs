using Docller.Core.Repository;

namespace Docller.Core.Services
{
    public enum TransmittalServiceStatus
    {
        Unknown = RepositoryStatus.Unknown,
        Success = RepositoryStatus.Success,
        RequiredFieldsMissing = RepositoryStatus.RequiredFieldsMissing
    }
}
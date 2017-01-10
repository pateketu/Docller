using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Repository;
using Docller.Core.Storage;

namespace Docller.Core.Services
{
    public enum StorageServiceStatus
    {
        Unknown = RepositoryStatus.Unknown,
        Success = RepositoryStatus.Success,
        UploadInProgress = 1,
        ExistingFile = RepositoryStatus.ExistingFile,
        VersionPathNull = RepositoryStatus.VersionPathNull,
        ExistingFolder = RepositoryStatus.ExistingFolder,
        DownloadComplete = BlobStorageProviderStatus.DownloadComplete,
        DownloadCanceled = BlobStorageProviderStatus.DownloadCanceled
    }
}

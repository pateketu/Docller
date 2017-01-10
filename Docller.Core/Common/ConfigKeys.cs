using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Common
{
    public class ConfigKeys
    {
        public const string AzureBlobStorage = "AzureBlobStorage";
        public const string LocalStoragePath = "LocalStoragePath";
        public const string SupportedCADFileTypes = "SupportedCADFileTypes";
        public const string DistributionName = "DistributionName";
        public const string FederatioName = "FederatioName";
        public const string FederationEnabled = "FederationEnabled";
        public const string NoReplyEmailAddress = "NoReplyEmailAddress";
        public const string AzureStorageConnectionString = "AzureStorageConnectionString";
        public const string ConnectionStringName = "DocllerDbConnection";
        public const string DefaultFolders = "DefaultFolders";
        public const string AzureFolderPathSeperator = "AzureFolderPathSeperator";
        public const string DomainFormat = "DomainFormat";
        public const string DefaultStatus = "DefaultStatus";
        public const string DownloadChunkSize = "DownloadChunkSize";
        public const string UseHttpCache = "UseHttpCache";
        public const string DiagnosticsConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
    }
}

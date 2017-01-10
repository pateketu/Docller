using System;
using System.IO;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Docller.Core.Common
{
    public class AzureLocalStorage : LocalStorage
    {
        private readonly LocalResource _localResource;
        private readonly string _path;
        private const string LocalStorage = "BlobStorage";
        public AzureLocalStorage()
        {
            _localResource = RoleEnvironment.GetLocalResource(LocalStorage);
            _path = _localResource.RootPath;

        }

        protected override string LocalStorageFolderPath
        {
            get { return _path; }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Storage;
using SnowMaker;

namespace Docller.Core.Common
{
    public class DocllerUniqueIdGenerator:UniqueIdGenerator
    {
        public DocllerUniqueIdGenerator():base(GetDataStore())
        {
            
        }

        private static IOptimisticDataStore GetDataStore()
        {
            return DocllerEnvironment.UseAzureBlobStorage
                       ? (IOptimisticDataStore)
                             new BlobOptimisticDataStore(StorageHelper.StorageAccount, Constants.SystemContainer)
                             : new FileOptimisticDataStore(Config.GetValue<string>(ConfigKeys.LocalStoragePath));
        }
    }
}

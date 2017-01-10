using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Docller.Core.Common;
using Docller.Core.Storage;

namespace Docller
{
    public class CloudDeployment
    {
        public static void Initialize()
        {
            
            if (DocllerEnvironment.UseAzureBlobStorage)
            {
                CreateContainers();
            }
        }

        private static void CreateContainers()
        {
            IBlobStorageProvider blobStorage = Factory.GetInstance<IBlobStorageProvider>();
            blobStorage.CreateContainer(Constants.SystemContainer);
            blobStorage.CreateContainer(Constants.PreviewImagesContainer);
            blobStorage.CreateContainer(Constants.CustomerContainer);
        }
    }
}
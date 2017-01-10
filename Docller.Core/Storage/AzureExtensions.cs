using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.StorageClient;
using BlobRequestOptions = Microsoft.WindowsAzure.Storage.Blob.BlobRequestOptions;
using CloudBlobContainer = Microsoft.WindowsAzure.StorageClient.CloudBlobContainer;
using CloudBlockBlob = Microsoft.WindowsAzure.Storage.Blob.CloudBlockBlob;
using CloudPageBlob = Microsoft.WindowsAzure.Storage.Blob.CloudPageBlob;

namespace Docller.Core.Storage
{
    public static class AzureExtensions
    {
        /// <summary>
        /// Existses the specified container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        public static bool Exists(this CloudBlobContainer container)
        {
            try
            {
                container.FetchAttributes();
                return true;
            }
            catch (StorageClientException e)
            {
                if (e.ErrorCode == StorageErrorCode.ResourceNotFound)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }


       

        public static ICloudBlob CreateSnapshot(this ICloudBlob blob)
        {
            ICloudBlob snapShot;
            if (blob is CloudBlockBlob)
            {
                snapShot = ((CloudBlockBlob) blob).CreateSnapshot();
            }
            else
            {
                snapShot = ((CloudPageBlob) blob).CreateSnapshot();
            }

            return snapShot;
        }
    }
}

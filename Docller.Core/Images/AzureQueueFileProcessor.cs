using System;
using Docller.Core.Models;

namespace Docller.Core.Images
{
    public class AzureQueueFileProcessor:IFileProcessor
    {
        public void ProcessAsync(long customerId, BlobBase blobBase)
        {
            throw new NotImplementedException();
        }
    }
}
using Docller.Core.Models;

namespace Docller.Core.Images
{
    public interface IFileProcessor
    {
        void ProcessAsync(long customerId, BlobBase blobBase);
    }
}
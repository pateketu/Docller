using System.Threading.Tasks;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.Core.Images
{
    public class LocalFileProcessor : IFileProcessor
    {
        private class ThreadState
        {
            public long CustomerId;
            public BlobBase File;
        }
        public void ProcessAsync(long customerId, BlobBase blobBase)
        {
            ThreadState state = new ThreadState() { CustomerId = customerId, File = blobBase };
            Task.Factory.StartNew(AsyncProcess, state, TaskCreationOptions.DenyChildAttach);
        }

        private void AsyncProcess(object state)
        {
            ThreadState threadState = (ThreadState) state;
            ProcessFile(threadState.CustomerId, threadState.File);
        }

        protected virtual void ProcessFile(long customerId, BlobBase blobBase)
        {
            if (blobBase is File)
            {
                IPreviewImageProvider previewImage = Factory.GetInstance<IPreviewImageProvider>();
                previewImage.GeneratePreviews(customerId, blobBase);
            }
        }
    }
}
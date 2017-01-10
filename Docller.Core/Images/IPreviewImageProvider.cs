using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Docller.Core.Models;
using Docller.Core.Services;

namespace Docller.Core.Images
{
    public interface IPreviewImageProvider
    {
        void GeneratePreviews(long customerId, BlobBase blobBase);
        void DeletePreviews(long customerId, BlobBase blobBase);
        string GetPreview(long customerId, BlobBase blobBase, PreviewType previewType);
        string GetZoomablePreview(long customerId, BlobBase blobBase);
        string GetTile(long customerId, BlobBase blobBase, string tile);
    }
}

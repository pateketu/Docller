namespace Docller.Core.Images
{
    public interface IZoomableImageProvider
    {
        string GenerateZoomableImage(string inputImage, string destFolder);
    }
}
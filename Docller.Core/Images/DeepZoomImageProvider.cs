using System.IO;
using Microsoft.DeepZoomTools;

namespace Docller.Core.Images
{
    public class DeepZoomImageProvider : IZoomableImageProvider
    {
        public string GenerateZoomableImage(string inputImage, string destFolder)
        {
            ImageCreator ic = new ImageCreator
                {
                    TileSize = 256,
                    TileFormat = ImageFormat.Png,
                    ImageQuality = 0.95,
                    TileOverlap = 0
                };

            string target = destFolder + "\\zoomed";
            string dziFile = string.Format("{0}\\zoomed.xml", destFolder);
            if (!File.Exists(dziFile))
            {
                ic.Create(inputImage, target);
            }

            return dziFile;

        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Images
{
    public static class ImageResizer
    {
        public static void ResizeImage(string inputFile, string outputfile, double height, double width)
        {
            using (Image photo = new Bitmap(inputFile))
            {
               ResizeImage(photo,outputfile,height, width);
            }
        }

        public static void ResizeImage(Image photo, string outputfile, double height, double width)
        {
            double aspectRatio = (double)photo.Width / photo.Height;
            double boxRatio = width / height;
            double scaleFactor = 0;

            if (photo.Width < width && photo.Height < height)
            {
                // keep the image the same size since it is already smaller than our max width/height
                scaleFactor = 1.0;
            }
            else
            {
                if (boxRatio > aspectRatio)
                    scaleFactor = height / photo.Height;
                else
                    scaleFactor = height / photo.Width;
            }

            int newWidth = (int)(photo.Width * scaleFactor);
            int newHeight = (int)(photo.Height * scaleFactor);

            using (Bitmap bmp = new Bitmap(newWidth, newHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    g.DrawImage(photo, 0, 0, newWidth, newHeight);
                    if (File.Exists(outputfile))
                    {
                        File.Delete(outputfile);
                    }
                    bmp.Save(outputfile, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }
    }
}

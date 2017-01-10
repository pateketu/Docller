using System;
using System.Collections.Generic;

namespace Docller.Core.Images
{
    public static class ImageConverters
    {
        private static readonly Dictionary<string, IImageConverter> Converters;
        static ImageConverters()
        {
            Converters = new Dictionary<string, IImageConverter>(StringComparer.InvariantCultureIgnoreCase);
            PdfToImageConverter pdfToImageConverter = new PdfToImageConverter();
            JpegImageConverter jpeg = new JpegImageConverter();
            JpgImageConverter jpg = new JpgImageConverter();
            PngImageConverter png = new PngImageConverter();
            BmpImageConverter bmp = new BmpImageConverter();
            GifImageConverter gif = new GifImageConverter();

            Converters.Add(pdfToImageConverter.Extension, pdfToImageConverter);
            Converters.Add(jpeg.Extension, jpeg);
            Converters.Add(jpg.Extension, jpg);
            Converters.Add(png.Extension, png);
            Converters.Add(bmp.Extension, bmp);
            Converters.Add(gif.Extension, gif);
        }

        public static IImageConverter Get(string extension)
        {
            if (Converters.ContainsKey(extension))
            {
                return Converters[extension];
            }
            return new NullImageConverter();
        }


    }
}
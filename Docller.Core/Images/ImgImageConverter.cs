using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Images
{
    public abstract  class ImgImageConverter : IImageConverter
    {
        public virtual void Convert(string inputFile, string outputPngfile)
        {
            File.Copy(inputFile,outputPngfile,true);
        }

        public abstract string Extension { get;}
    }

    public class JpegImageConverter : ImgImageConverter
    {
        public override string Extension
        {
            get { return ".jpeg"; }
        }
    }

    public class BmpImageConverter : ImgImageConverter
    {
        public override string Extension
        {
            get { return ".bmp"; }
        }
    }

    public class GifImageConverter : ImgImageConverter
    {
        public override string Extension
        {
            get { return ".gif"; }
        }
    }

    public class PngImageConverter : ImgImageConverter
    {
        public override string Extension
        {
            get { return ".png"; }
        }
    }

    public class JpgImageConverter : ImgImageConverter
    {
        public override string Extension
        {
            get { return ".jpg"; }
        }
    }
}

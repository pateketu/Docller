namespace Docller.Core.Images
{
    public class NullImageConverter : IImageConverter
    {
        public void Convert(string inputFile, string outputPngfile)
        {
            //do nothing
        }

        public string Extension { get { return string.Empty; } }
    }
}
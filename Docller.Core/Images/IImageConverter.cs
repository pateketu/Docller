namespace Docller.Core.Images
{
    public interface IImageConverter
    {
        void Convert(string inputFile, string outputPngfile);
        string Extension { get; }
    }
}
using System.Drawing;
using Docller.Core.Common;
using GhostscriptSharp;
using GhostscriptSharp.Settings;

namespace Docller.Core.Images
{
    public class PdfToImageConverter : IImageConverter
    {
        public PdfToImageConverter()
        {
            IGhostscriptLibraryLoader gsLoader = Factory.GetInstance<IGhostscriptLibraryLoader>();
            gsLoader.LoadLibrary();
        }
        public void Convert(string inputFile, string outputPngfile)
        {
            GhostscriptWrapper.GenerateOutput(inputFile, outputPngfile,
                                              new GhostscriptSettings
                                                  {
                                                      Device = GhostscriptDevices.pngalpha,
                                                      Page = new GhostscriptPages
                                                          {
                                                              Start = 1,
                                                              End = 1,
                                                              AllPages = false
                                                          },
                                                      Resolution = new Size
                                                          {
                                                              Height = 72,
                                                              Width = 72
                                                          },
                                                      Size = new GhostscriptPageSize
                                                          {
                                                              Native = GhostscriptPageSizes.a4
                                                          }
                                                  }
                );
        }

        
        public string Extension
        {
            get
            {
                return ".pdf";
            }
        }
    }
}
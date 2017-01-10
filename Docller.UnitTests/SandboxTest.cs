using System;
using System.Drawing.Imaging;
using System.IO;
using Docller.Core.Images;
using Docller.Core.Models;
using Docller.Core.Storage;
using Docller.Tests;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using Telerik.JustMock;
using File = Docller.Core.Models.File;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Docller.UnitTests
{
    [TestClass]
    public class SandboxTest:FixtureBase
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            FixtureBase.RegisterMappings();
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        /// <remarks></remarks>
        [ClassCleanup]
        public static void CleanUp()
        {
            ObjectFactory.EjectAllInstancesOf<Database>();
        }

        
        public void Imagetest()
        {
            PreviewImageProvider preview = new PreviewImageProvider(new MockDirectDownloadProvider());

           
              
            preview.GeneratePreviews(0,
                                     new File()
                                         {
                                             FileName = "TBA_CTG_SI_01_PL_B50_014.pdf",
                                             FileExtension = ".pdf",
                                             FileId = 2,
                                             Folder = new Folder() {FolderId = 1},
                                             Project = new Project() {BlobContainer = "b", ProjectId = 1}
                                         });
        }

        [TestMethod]
        public void Pdf_Creator()
        {
            /*AzureBlobStorageProvider azureBlobStorageProvider = new AzureBlobStorageProvider();
            byte[] data;
            string fu;
            using (FileStream stream = new FileStream(@"C:\Temp\photo1.jpg",FileMode.Open,FileAccess.Read))
            {

                data = new byte[(int)stream.Length];
                stream.Read(data, 0, (int) stream.Length);
                
            }

            File f = new File {Project = new Project() {BlobContainer = "project-1-blobs"}};*/
            //AzureBlobStorageProvider.Upload("photo1.jpg", "image/jpeg", false, false, f, data, out fu);
            var doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream("C:\\temp\\PDFs\\" + Guid.NewGuid().ToString() + ".pdf", FileMode.Create));
            doc.Open();
            

            Image logo = iTextSharp.text.Image.GetInstance(@"C:\Docller\Source\Docller.UI\Images\SampleLogo.png");
            logo.ScaleToFit(45f,45f);
               
            PdfPTable table = new PdfPTable(13);
            
            table.WidthPercentage = 100;
            
            table.SetWidths(
                new float[] {172.5f, 172.5f, 50, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20});

            PdfPCell cell1 = new PdfPCell(logo)
            {
                Colspan = 3,
                Padding = 5,
                Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 1


            };
            table.AddCell(cell1);

            PdfPCell cell = new PdfPCell(new Phrase("1111 -- Issue Sheet"))
                {
                    Colspan = 10,
                    Border = PdfPCell.TOP_BORDER | PdfPCell.RIGHT_BORDER ,
                    BorderColor = BaseColor.BLACK,
                    BorderWidth = 1
                    
                    
                };
            table.AddCell(cell);

            Font f = FontFactory.GetFont("ARIAL", 9, BaseColor.WHITE);
            table.AddCell(new PdfPCell(new Phrase("Doc Number",f))
                {
                    BackgroundColor = BaseColor.DARK_GRAY,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER,
                    BorderColor = BaseColor.BLACK,
                    BorderWidth = 1
                    
                });
            table.AddCell(new PdfPCell(new Phrase("Title", f) )
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.TOP_BORDER | PdfPCell.RIGHT_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 1
            });
            table.AddCell(new PdfPCell(new Phrase("Latest Rev", f) )
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.TOP_BORDER| PdfPCell.RIGHT_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 1
            });
            
            table.AddCell(new PdfPCell(new Phrase("Issue Dates", f))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Colspan = 15,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.TOP_BORDER  | PdfPCell.RIGHT_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 1             
                
            });

            table.AddCell(new PdfPCell(new Phrase(" ")) );
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(GetInnerTable()) { Colspan = 10 });

            
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));

            table.AddCell("File A");
            table.AddCell("title A");
            table.AddCell(new PdfPCell(new Phrase("A")){HorizontalAlignment = Element.ALIGN_CENTER});
            
            table.AddCell(new PdfPCell(new Phrase("A")){HorizontalAlignment = Element.ALIGN_CENTER});
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            table.AddCell(new PdfPCell(new Phrase(" ")));
            
            
            doc.Add(table);
            
            doc.Close();
        }

        private static PdfPTable GetInnerTable1()
        {
            PdfPTable table = new PdfPTable(1);
            table.SetWidths(new float[] { 50 });
            Font f = FontFactory.GetFont("ARIAL", 8, BaseColor.BLACK);
            table.AddCell(new PdfPCell(new Phrase("Date", f)));
            table.AddCell(new PdfPCell(new Phrase("Month", f)));
            table.AddCell(new PdfPCell(new Phrase("Year", f)));
            return table;
        }
        private static PdfPTable GetInnerTable()
        {
            PdfPTable table = new PdfPTable(10);
            table.SetWidths(new float[] {20, 20, 20, 20, 20, 20, 20, 20, 20, 20});
            Font f = FontFactory.GetFont("ARIAL", 8, BaseColor.BLACK);
            for (int i = 0; i < 3; i++)
            {
                if (i == 1)
                {
                    table.AddCell(new PdfPCell(new Phrase("Jun", f)));
                    table.AddCell(new PdfPCell(new Phrase("Jun", f)));
                    table.AddCell(new PdfPCell(new Phrase("Jun", f)));
                    table.AddCell(new PdfPCell(new Phrase("Jun", f)));
                    table.AddCell(new PdfPCell(new Phrase("Jun", f)));
                    table.AddCell(new PdfPCell(new Phrase("Jun", f)));
                    table.AddCell(new PdfPCell(new Phrase("Jun", f)));
                    table.AddCell(new PdfPCell(new Phrase("Jun", f)));
                    table.AddCell(new PdfPCell(new Phrase("Jun", f)));
                    table.AddCell(new PdfPCell(new Phrase("Jun", f)));
                    
                }
                else{
                    table.AddCell(new PdfPCell(new Phrase("10", f)));
                    table.AddCell(new PdfPCell(new Phrase("10", f)));
                    table.AddCell(new PdfPCell(new Phrase("10", f)));
                    table.AddCell(new PdfPCell(new Phrase("10", f)));
                    table.AddCell(new PdfPCell(new Phrase("10", f)));
                    table.AddCell(new PdfPCell(new Phrase("10", f)));
                    table.AddCell(new PdfPCell(new Phrase("10", f)));
                    table.AddCell(new PdfPCell(new Phrase("10", f)));
                    table.AddCell(new PdfPCell(new Phrase("10", f)));
                    table.AddCell(new PdfPCell(new Phrase("10", f)));
                    
                }

            }


            return table;
        }

    }

    public class MockDirectDownloadProvider : IDirectDownloadProvider
    {
        public string GetFileFromStorage(long customerId, BlobBase blobBase)
        {
            return "C:\\TBA_CTG_SI_01_PL_B50_014.pdf";
        }
    }


    public class MockGhostscriptLibraryLoader:IGhostscriptLibraryLoader
    {
        public void LoadLibrary()
        {
            string gsDll = IntPtr.Size == 4 ? "gsdll32" : "gsdll64";
            string path =string.Format(@"C:\Docller\Source\Docller.UnitTests\Ghostscript\{0}.dll", gsDll);
            NativeMethods.LoadLibrary(path);
        }

        public void UnLoadLibrary()
        {
            
        }
    }
}
    
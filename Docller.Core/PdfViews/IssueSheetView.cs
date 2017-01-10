using System.Globalization;
using System.IO;
using Docller.Core.Common;
using Docller.Core.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Docller.UI.PdfViews
{
    public class IssueSheetView
    {
        private readonly IssueSheet _issueSheetModel; 

        public IssueSheetView(IssueSheet issueSheetModel)
        {
            _issueSheetModel = issueSheetModel;
        }


        public void Create(string fileName)
        {
            Document doc = new Document();
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                PdfWriter.GetInstance(doc, stream);
                doc.Open();
                PdfPTable table = GetTable();
                this.AddHeader(table);
                this.AddTableHeadings(table);
                this.AddDatePlaceHolderCells(table);
                this.AddEmptyRows(table, 0, BaseColor.BLACK, 1);
                this.AddDetails(table);
                this.AddEmptyRows(table, 1, BaseColor.BLACK, 5);
                this.AddInnerHeader(table, "Purpose of Issue");
                this.AddIssueStatusDetails(table);
                this.AddInnerHeader(table, "Distribution");
                this.AddDistribution(table);
                this.AddLastRow(table, BaseColor.BLACK);
                doc.Add(table);
                doc.Close();

            }
        }

        private PdfPTable GetTable()
        {
            PdfPTable table = new PdfPTable(13) {WidthPercentage = 100};
            table.SetWidths(
                new float[] { 172.5f, 172.5f, 50, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 });
            return table;
        }

        private void AddHeader(PdfPTable table)
        {

            PdfPCell logoCell;
            if (!string.IsNullOrEmpty(_issueSheetModel.CustomerLogo))
            {
                Image logo = Image.GetInstance(_issueSheetModel.CustomerLogo);
                logo.ScaleToFit(45f, 45f);
                logoCell = new PdfPCell(logo);
            }
            else
            {
                logoCell = new PdfPCell(new Phrase(" "));
            }
            logoCell.Colspan = 3;
            logoCell.Padding = 5;
            logoCell.Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER;
            logoCell.BorderColor = BaseColor.BLACK;
            logoCell.BorderWidth = 0.5f;

            table.AddCell(logoCell);

            PdfPCell issueSheetTitle = new PdfPCell(new Phrase(string.Format("Issue Sheet - {0}", this._issueSheetModel.TransmittalNumber)))
            {
                Colspan = 10,
                Border = PdfPCell.TOP_BORDER | PdfPCell.RIGHT_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                PaddingRight = 5


            };
            table.AddCell(issueSheetTitle);
        }

        private void AddTableHeadings(PdfPTable table)
        {
            Font f = FontFactory.GetFont("ARIAL", 9, BaseColor.WHITE);
            table.AddCell(new PdfPCell(new Phrase("Doc Number", f))
            {
                BackgroundColor = BaseColor.GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f

            });
            table.AddCell(new PdfPCell(new Phrase("Title", f))
            {
                BackgroundColor = BaseColor.GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f
            });
            table.AddCell(new PdfPCell(new Phrase("Latest Rev", f))
            {
                BackgroundColor = BaseColor.GRAY,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f
            });

            table.AddCell(new PdfPCell(new Phrase("Issue Dates", f))
            {
                BackgroundColor = BaseColor.GRAY,
                Colspan = 15,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f

            });
        }

        private void AddDatePlaceHolderCells(PdfPTable table)
        {
            table.AddCell(new PdfPCell(new Phrase(" "))
            {
                Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f
            });
            table.AddCell(new PdfPCell(new Phrase(" "))
            {
                Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f
            });
            table.AddCell(new PdfPCell(new Phrase(" "))
            {
                Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f
            });
            table.AddCell(new PdfPCell(GetDatesTable())
            {
                Colspan = 10,
                Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f
            });
        }

        private PdfPTable GetDatesTable() 
        {
            PdfPTable table = new PdfPTable(10);
            table.SetWidths(new float[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 });
            Font f = FontFactory.GetFont("ARIAL", 8, BaseColor.BLACK);

            var transmittals = this._issueSheetModel.Transmittals;
            for (int i = 0; i < 3; i++)
            {
                bool isFirstCell = true;

                for (int j = 0; j < 10;j++ )
                {
                    
                    string data = " ";
                    DateTime dateTime = DateTime.MinValue;
                    if (transmittals.Count > j)
                    {
                        dateTime = transmittals[j].CreatedDate;
                    }
                    if (dateTime > DateTime.MinValue)
                    {
                        switch (i)
                        {
                            case 0:
                                data = dateTime.Day.ToString(CultureInfo.InvariantCulture);
                                break;
                            case 1:
                                data = dateTime.ToString("MMM");
                                break;
                            default:
                                data = dateTime.ToString("yy");
                                break;
                        }
                    }

                    table.AddCell(new PdfPCell(new Phrase(data, f))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = isFirstCell ? PdfPCell.TOP_BORDER : PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER,
                        BorderColor = BaseColor.BLACK,                        
                        BorderWidth = 0.5f
                    });
                    isFirstCell = false;
                }
            }
            return table;
        }
        private void AddLastRow(PdfPTable table, BaseColor borderColor)
        {
            for (int j = 0; j < 13; j++)
            {
                table.AddCell(new PdfPCell(new Phrase(" "))
                    {
                        BorderColor = borderColor,
                        BorderWidth = 0.5f,
                        Border =
                            j < 13 - 1
                                ? PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER
                                : PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER | PdfPCell.LEFT_BORDER |
                                  PdfPCell.RIGHT_BORDER
                    });
            }
        }
        private void AddEmptyRows(PdfPTable table, int topBorder, BaseColor borderColor, int rowCount)
        {
            AddEmptyRows(table,topBorder,borderColor,rowCount,13);
        }
        private void AddEmptyRows(PdfPTable table, int topBorder, BaseColor borderColor, int rowCount, int col)
        {
            for (int i = 0; i < rowCount; i++) 
            {
                for (int j = 0; j < col; j++)
                {
                    table.AddCell(new PdfPCell(new Phrase(" "))
                    {
                        BorderColor = borderColor,
                        BorderWidth = 0.5f,
                        Border =  j < col-1 ? topBorder | PdfPCell.LEFT_BORDER : topBorder | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER
                    });
                }
            }
        }

        private void AddDetails(PdfPTable table)
        {
            Font font = FontFactory.GetFont("ARIAL", 9, BaseColor.BLACK);
            var files = _issueSheetModel.IssuedFiles;
            foreach (var f in files)
            {
                table.AddCell(GetDetailCell(f.FileName,font, false,false));
                table.AddCell(GetDetailCell(f.Title, font,false, false));
                table.AddCell(GetDetailCell(f.Revision, font,false, true));

                for (int i = 0; i < 10; i++)
                {
                    TransmittalInfo transmittalInfo = null;
                    string data = " ";
                    if(_issueSheetModel.Transmittals.TryGetItemAt(i,out transmittalInfo))
                    {
                        data = transmittalInfo.IssuedFilesInfo.ContainsKey(f.FileName)
                                   ? transmittalInfo.IssuedFilesInfo[f.FileName].Revision
                                   : " ";
                    }
                    table.AddCell(GetDetailCell(data, font, i ==9, true));
                }
            }            
        }

        private void AddInnerHeader(PdfPTable table, string data) 
        {
            Font f = FontFactory.GetFont("ARIAL", 9, BaseColor.WHITE);
            table.AddCell(new PdfPCell(new Phrase(data, f))
            {
                BackgroundColor = BaseColor.GRAY,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f,
                Colspan = 3

            });
            this.AddEmptyRows(table,PdfPCell.TOP_BORDER,BaseColor.BLACK,1,10);
        }

        private void AddIssueStatusDetails(PdfPTable table)
        {
            this.AddEmptyRows(table,PdfPCell.TOP_BORDER,BaseColor.BLACK,1);
            IEnumerable<string> uniqueStatus = (from t in _issueSheetModel.Transmittals
                                                select t.TransmittalStatus.StatusText).Distinct(StringComparer.InvariantCultureIgnoreCase);
            Font font = FontFactory.GetFont("ARIAL", 9, BaseColor.BLACK);
            foreach (string s in uniqueStatus)
            {
                table.AddCell(GetDetailCell(s, font, false, false));

                for (int i = -2; i < 10; i++)
                {
                    TransmittalInfo t = null;
                    string data = " ";
                    if (i >=0 && this._issueSheetModel.Transmittals.TryGetItemAt(i, out t))
                    {
                        data = t.TransmittalStatus.StatusText.Equals(s, StringComparison.InvariantCultureIgnoreCase)
                                   ? "X"
                                   : " ";
                    }

                    table.AddCell(GetDetailCell(data, font, i == 9, true));
                }

            }
            this.AddEmptyRows(table, PdfPCell.TOP_BORDER, BaseColor.BLACK, 1);

        }

        private void AddDistribution(PdfPTable table)
        {
            Font font = FontFactory.GetFont("ARIAL", 9, BaseColor.BLACK);
            this.AddEmptyRows(table, PdfPCell.TOP_BORDER, BaseColor.BLACK, 1);
            IEnumerable<TransmittalUser> uniqueUsers = (from u in _issueSheetModel.Distribution
                                                        select u).Distinct(new TransmittalUserComparer());

            foreach (TransmittalUser transmittalUser in uniqueUsers)
            {
                table.AddCell(
                    GetDetailCell(
                        string.Format("{0} ({1})", transmittalUser.DisplayName, transmittalUser.Company.CompanyName),
                        font, false, false));

                for (int i = -2; i < 10; i++)
                {
                    TransmittalInfo t = null;
                    string data = " ";
                    if (i >= 0 && this._issueSheetModel.Transmittals.TryGetItemAt(i, out t))
                    {
                        data = t.DistributionInfo.ContainsKey(transmittalUser.UserId)
                                   ? "X"
                                   : " ";
                    }

                    table.AddCell(GetDetailCell(data, font, i == 9, true));
                }
            }
            this.AddEmptyRows(table, PdfPCell.TOP_BORDER, BaseColor.BLACK, 1);
        }

        private PdfPCell GetDetailCell(string data, Font font, bool addRightBorder, bool alignCenter)
        {
            return new PdfPCell(new Phrase(data, font))
                {
                    Border =
                        addRightBorder
                            ? PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER
                            : PdfPCell.TOP_BORDER | PdfPCell.LEFT_BORDER,
                    BorderColor = BaseColor.BLACK,
                    BorderWidth = 0.5f,
                    HorizontalAlignment = alignCenter ? Element.ALIGN_CENTER : Element.ALIGN_LEFT

                };
        }



    }
}
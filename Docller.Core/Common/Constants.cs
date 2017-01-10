using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Common
{
    public class Constants
    {
        public static int ExistingFile =1;
        public const string PdfExtension =".pdf";
        public const string SystemContainer = "docller-system";
        public const string CustomerContainer = "docller-customers";
        public const string DefaultContentType = "application/octet-stream";
        public const string DefaultFileTypeIcon = "_blank.png";
        public const string MIMETypesTable = "/MIMETypes.txt";
        public const string SmallFileTypeIconsFolder = "/Images/filetype-icons/16X16";
        public const string MeidumFileTypeIconsFolder = "/Images/filetype-icons/32X32";
        public const string FileDownloadCookie = "fileDownload";

        public const string IssueSheetCacheFolder = "IssueSheets";
        public const string TransmittalFilesCacheFolder = "IssuedFiles";
        public const string PreviewImagesContainer = "img-previews";
        public const string CustomerLogoFolder = "Logos";
        public const string AnonymouseCookieName = "Docller_Anon";
    }
}

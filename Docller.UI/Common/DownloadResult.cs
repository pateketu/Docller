using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Docller.Core.Common;
using Docller.Core.Services;

namespace Docller.Common
{
    public class DownloadResult : FileResult
    {
        private readonly IDownloadProvider _downloadProvider;

        public DownloadResult(IDownloadProvider downloadProvider) : base(downloadProvider.ContentType)
        {
            _downloadProvider = downloadProvider;
            this.FileDownloadName = downloadProvider.FileName;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            response.SetCookie(new HttpCookie(Constants.FileDownloadCookie, "true") { Path = "/" });
            response.Buffer = false;
             _downloadProvider.DownloadToStream(response.OutputStream, new ClientConnection(response));
        }
    }
}
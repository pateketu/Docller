using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Docller.Core.Common;

namespace Docller.Common
{
    public class FailedDownloadResult : HttpStatusCodeResult
    {
        public FailedDownloadResult(int statusCode) : base(statusCode)
        {
        }

        public FailedDownloadResult(HttpStatusCode statusCode, string statusDescription) : base(statusCode, statusDescription)
        {
        }

        public FailedDownloadResult(int statusCode, string statusDescription) : base(statusCode, statusDescription)
        {
        }

        public FailedDownloadResult(HttpStatusCode statusCode) : base(statusCode)
        {
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.SetCookie(new HttpCookie(Constants.FileDownloadCookie, "false") { Path = "/" });
            base.ExecuteResult(context);
        }
        
    }
}
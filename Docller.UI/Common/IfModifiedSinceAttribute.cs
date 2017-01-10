using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Docller.Core.Common;

namespace Docller.UI.Common
{
    public class IfPreviewModifiedSinceAttribute : ActionFilterAttribute
    {
       public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Headers["If-Modified-Since"] != null)
            {
                DateTime modifiedSince =
                    DateTime.Parse(filterContext.HttpContext.Request.Headers["If-Modified-Since"]);
                long ticks;
                if (long.TryParse(filterContext.HttpContext.Request["PTag"], out ticks))
                {
                    DateTime imageModified = new DateTime(ticks);

                    if (imageModified.TrimMilliseconds() <= modifiedSince.TrimMilliseconds())
                    {
                        filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.NotModified);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }

    }
}
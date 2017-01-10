using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;
using Docller.Core.Common;


namespace Docller.UI.Common
{
    public class ExceptionHandlingAttribute : HandleErrorAttribute
    {
        
        public override void OnException(ExceptionContext filterContext)
        {
            //Log it
            Logger.Log(HttpContext.Current != null ? new HttpContextWrapper(HttpContext.Current) : null,
                filterContext.Exception);

            string user = HttpContext.Current != null ? HttpContext.Current.User.Identity.Name : "No HttpContext";
            Task.Run(() =>
            {
                SmtpClient client = new SmtpClient();
                client.Send("Error@Docller.com", "Info@Docller.com", "Errors found",
                    string.Format("User: {0}\n Exception: {1}", user, filterContext.Exception.Message));
            });
            
            base.OnException(filterContext);
        }

      

    }
}
using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.Mvc;
using ActionMailer.Net;
using ActionMailer.Net.Mvc;
using Docller.Core.Common;
using RazorEngine;
using StructureMap;
using Encoding = System.Text.Encoding;

namespace Docller.Common
{
    public class AsyncEmailResult:EmailResult
    {
        private readonly object _model;

        public AsyncEmailResult(object model, IMailInterceptor interceptor, IMailSender sender, MailMessage mail, string viewName, string masterName, Encoding messageEncoding, bool trimBody) : base(interceptor, sender, mail, viewName, masterName, messageEncoding, trimBody)
        {
            _model = model;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            string htmlViewName = String.Format("{0}.html", ViewName);
            string template = GetTemplateView(htmlViewName);
            string html = RenderView(template);
            var altView = AlternateView.CreateAlternateViewFromString(html, MessageEncoding ?? Encoding.Default, MediaTypeNames.Text.Html);
            Mail.AlternateViews.Add(altView);
        }

        private string RenderView(string template)
        {
            string result = Razor.Parse(template, _model);
            return result;
        }


        private string GetTemplateView(string htmlViewName)
        {
            IPathMapper pathMapper = ObjectFactory.GetInstance<IPathMapper>();
            string fullPath = pathMapper.MapPath(string.Format("\\Views\\Mail\\{0}.cshtml", htmlViewName));
            string data = null;
            using (StreamReader stream = new StreamReader(fullPath))
            {
                data = stream.ReadToEnd();
            }
            return data;
        }

    }
}
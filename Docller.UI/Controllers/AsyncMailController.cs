using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ActionMailer.Net;
using ActionMailer.Net.Mvc;
using Docller.Common;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.Controllers
{
    public class AsyncMailController:MailerBase
    {
        public AsyncEmailResult TransmittalEmail(TransmittalEmailView transmittal, string issueSheetFile)
        {
            IEnumerable<string> to = transmittal.Transmittal.Distribution .Where(x => !x.IsCced).Select(x => x.Email);
            List<string> cc = transmittal.Transmittal.Distribution.Where(x => x.IsCced).Select(x => x.Email).ToList();
            this.From = Config.GetValue<string>(ConfigKeys.NoReplyEmailAddress);
            foreach (string s in to)
            {
                this.To.Add(s);
            }

            if (cc.Any())
            {
                foreach (string s in cc)
                {
                    this.CC.Add(s);
                }
            }
            this.Subject = string.Format("New Transmittal: {0}", transmittal.Transmittal.Subject);
            FileInfo info = new FileInfo(issueSheetFile);

            this.Attachments.Add(info.Name, System.IO.File.ReadAllBytes(issueSheetFile));
            return Email("TransmittalEmail", transmittal);
        }

        public virtual AsyncEmailResult Email(string viewName, object model = null)
        {
            if (viewName == null)
                throw new ArgumentNullException("viewName");

            var mail = this.GenerateMail();
            AsyncEmailResult result = new AsyncEmailResult(model, this, MailSender, mail, viewName, null, MessageEncoding, true);
            
            result.ExecuteResult(ControllerContext);
            return result;
        }

        public override EmailResult Email(string viewName, object model = null, string masterName = null, bool trimBody = true)
        {
            return Email(viewName, model);
        }
    }
}
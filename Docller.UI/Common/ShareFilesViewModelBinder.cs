using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAnnotationsExtensions;

namespace Docller.UI.Common
{
    public class ShareFilesViewModelBinder : DefaultModelBinder
    {
     
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Name.Equals("Emails"))
            {
                if (!string.IsNullOrEmpty(controllerContext.HttpContext.Request.Form[propertyDescriptor.Name]))
                {
                    string[] emails;
                    TryGetEmails(controllerContext, bindingContext, out emails);
                    propertyDescriptor.SetValue(bindingContext.Model,emails);
                }

            }
            else if (propertyDescriptor.Name.Equals("FileIds"))
            {
                propertyDescriptor.SetValue(bindingContext.Model,
                    GetFileIds(controllerContext.HttpContext.Request.Form[propertyDescriptor.Name]));
            }
            else
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }


        private IEnumerable<string> GetEmails(string postData)
        {
            return postData.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //return addresses.Select(address => new User() {Email = address}).ToList();
        }

        private long[] GetFileIds(string postData)
        {
            List<long> ids = new List<long>();
            if (postData != null)
            {
                string[] s = postData.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string s1 in s)
                {
                    long id;
                    if (long.TryParse(s1, out id))
                    {
                        ids.Add(id);
                    }
                }
                return ids.ToArray();
            }

            return null;
        }

        private bool TryGetEmails(ControllerContext controllerContext, ModelBindingContext bindingContext, out string[] emails)
        {
            emails = null;
            Dictionary<string, string> dupEmails = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            IEnumerable<string> allEmails =
                        GetEmails(controllerContext.HttpContext.Request.Form["Emails"]);

            EmailAttribute emailAttribute = new EmailAttribute();
            bool hasErrros = false;
            foreach (string email in allEmails)
            {
                if (emailAttribute.IsValid(email) && !dupEmails.ContainsKey(email))
                {
                    dupEmails.Add(email, email); 
                }
                else
                {
                    bindingContext.ModelState.AddModelError(Guid.NewGuid().ToString(), string.Format("{0} is not a valid email address.", email));
                    hasErrros = true;
                }
            }
            emails = dupEmails.Values.ToArray();
            return hasErrros;
        }
    }
}
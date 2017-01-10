using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAnnotationsExtensions;
using Docller.Core.Models;
using Docller.UI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Docller.UI.Common
{
    public class CompaniesModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string jsonString = controllerContext.HttpContext.Request["data"];
            IEnumerable<InviteUsersViewModel> users = JsonConvert.DeserializeObject<IEnumerable<InviteUsersViewModel>>(jsonString);
            IEnumerable<Company> companies;
            TryBuildValidateModal(users, bindingContext.ModelState, out companies);
            return companies;
        }

        private IEnumerable<string> GetEmails(string users)
        {
            return users.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            //return addresses.Select(address => new User() {Email = address}).ToList();
        }

        public bool TryBuildValidateModal(IEnumerable<InviteUsersViewModel> users, ModelStateDictionary modalState,  out IEnumerable<Company> companies)
        {
            Dictionary<string, List<User>> dictionary = new Dictionary<string, List<User>>(StringComparer.InvariantCultureIgnoreCase);
            Dictionary<string, string> dupEmails = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            EmailAttribute emailAttribute = new EmailAttribute();
            bool hasErrors = false;
            foreach (InviteUsersViewModel userViewModel in users)
            {
                if (string.IsNullOrEmpty(userViewModel.CompanyName) || string.IsNullOrEmpty(userViewModel.Users))
                    continue;
                
                IEnumerable<string> emails = GetEmails(userViewModel.Users);
                if (!dictionary.ContainsKey(userViewModel.CompanyName))
                {
                    dictionary.Add(userViewModel.CompanyName,new List<User>());
                    foreach (string email in emails)
                    {
                        if (emailAttribute.IsValid(email))
                        {
                            if (!dupEmails.ContainsKey(email))
                            {
                                dupEmails.Add(email, email);
                                dictionary[userViewModel.CompanyName].Add(new User() {Email = email});
                            }
                            else
                            {
                                hasErrors = true;
                                modalState.AddModelError(Guid.NewGuid().ToString(), string.Format("{0} appears in multiple companies.", email));
                            }
                        }
                        else
                        {
                            hasErrors = true;
                            modalState.AddModelError(Guid.NewGuid().ToString(),string.Format("{0} is not a valid email address.",email));
                        }
                    }
                }
                else
                {
                    modalState.AddModelError(Guid.NewGuid().ToString(), string.Format("Attempting to invite {0} more than once.", userViewModel.CompanyName));
                }
            }
            
            companies = !hasErrors ? ConvertToModal(dictionary) : null;
            return hasErrors;
        }


        private IEnumerable<Company> ConvertToModal(Dictionary<string, List<User>> dictonary)
        {
            return dictonary.Select(keyValuePair => new Company() {CompanyName = keyValuePair.Key, Users = keyValuePair.Value});
        }
       
    }
}
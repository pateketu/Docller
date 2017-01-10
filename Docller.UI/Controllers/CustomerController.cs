using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Services;
using Docller.Models;
using Docller.UI.Common;
using StructureMap;

namespace Docller.Controllers
{
    public class CustomerController : DocllerControllerBase
    {
        //
        // GET: /Customer/
        [AllowAnonymous]
        public ActionResult Subscribe()
        {
            ViewBag.FormAction = ControllerContext.RouteData.GetRequiredString("action");
            return View();
        }

        [AllowAnonymous]
        public JsonResult IsDomainUrlInUse(string domainUrl)
        {
            string fullUrl = GetFullDomainUrl(domainUrl);
            ISubscriptionService subscriptionService = ServiceFactory.GetSubscriptionService();
            return !subscriptionService.IsDomainUrlExists(fullUrl)
                       ? Json(true, JsonRequestBehavior.AllowGet)
                       : Json(string.Format("{0} is not available.", domainUrl),
                              JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateCustomer()
        {
            return PartialView("_CustomerDetails",CustomerCache.Get(this.DocllerContext.CustomerId));
        }

        [HttpPost]
        public ActionResult UpdateCustomer(HttpPostedFileBase logo)
        {
            ISubscriptionService subscriptionService = ServiceFactory.GetSubscriptionService();
            subscriptionService.SaveLogo(this.DocllerContext.CustomerId,logo.InputStream,logo.FileName);
            CustomerCache.Invalidate(this.DocllerContext.CustomerId);
            return Redirect("/");
        }

        public ActionResult Logo()
        {
            ISubscriptionService subscriptionService = ServiceFactory.GetSubscriptionService();
            string logoFile = subscriptionService.LogoFile(this.DocllerContext.CustomerId);

            return new CachedFileResult(logoFile, "image/png");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Subscribe(Customer customer)
        {
            if (ModelState.IsValid)
            {
                string password =  Security.GeneratePassword();
                string subDomain = customer.DomainUrl.Trim();
                customer.AdminUser.Password = password;
                customer.DomainUrl = GetFullDomainUrl(subDomain);
                ISubscriptionService subscriptionService = ServiceFactory.GetSubscriptionService();
                SubscriptionServiceStatus status = subscriptionService.Subscribe(customer);
                if (status == SubscriptionServiceStatus.Success
                    || status == SubscriptionServiceStatus.ExistingUserNewCustomer)
                {
                    try
                    {
                        CustomerAdminUserViewModel viewModel = new CustomerAdminUserViewModel()
                                                                   {
                                                                       CustomerName = customer.CustomerName,
                                                                       Password = password,
                                                                       Email = customer.AdminUser.Email,
                                                                       DomainUrl = customer.DomainUrl
                                                                   };
                        if (status == SubscriptionServiceStatus.Success)
                        {
                            new MailController().WelcomeEmail(viewModel).Deliver();
                        }
                        else
                        {
                            new MailController().NewCustomerEmail(viewModel).Deliver();
                        }
                        return RedirectToAction("SubscriptionComplete", "Customer");
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(this.HttpContext, exception);
                        ModelState.AddModelError("", "Account was Created but problems occured while E-mailing your login details");
                    }

                    
                }else if (status == SubscriptionServiceStatus.ExistingCustomer)
                {
                    ModelState.AddModelError("",
                                             string.Format(CultureInfo.InvariantCulture,
                                                           "{0} is an existing Customer, Please contact us if the account needs to be re-activated",
                                                           customer.CustomerName));
                }else if(status == SubscriptionServiceStatus.DomainUrlInUse)
                {
                    ModelState.AddModelError("",
                                             string.Format(CultureInfo.InvariantCulture,
                                                           "Sudomain {0} is in use, Please use another one",
                                                           subDomain));
                }
                else
                {
                    ModelState.AddModelError("", "Errors occured, Please use the contact us link above to report the problem");
                }

            }
            return View(customer);
        }

        [AllowAnonymous]
        public ActionResult SubscriptionComplete()
        {
            return View();
        }

        /// <summary>
        /// Gets the full domain URL.
        /// </summary>
        /// <param name="subdomain">The subdomain.</param>
        /// <returns></returns>
        private static string GetFullDomainUrl(string subdomain)
        {
            return string.Format(CultureInfo.InvariantCulture, Config.GetValue<string>(ConfigKeys.DomainFormat),
                                 subdomain);
        }
    }
}

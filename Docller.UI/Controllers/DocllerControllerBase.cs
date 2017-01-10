using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Docller.Common;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Services;
using StructureMap;

namespace Docller.Controllers
{
    public abstract class DocllerControllerBase : Controller
    {
        private ViewBagWrapper _viewBagWrapper;
        public ViewBagWrapper ViewBagWrapper
        {
            get { return _viewBagWrapper ?? (_viewBagWrapper = new ViewBagWrapper(this)); }
        }

        public CookieData CurrentCookieData { get; set; }
        protected string CurrentDomain 
        {
            get { return Utils.GetDomain(Request.Url ?? System.Web.HttpContext.Current.Request.Url); }
        }

        protected void SaveCurrentCookieData()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName] ??
                                    Response.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                FormsAuthenticationTicket newAuthTicket = new FormsAuthenticationTicket(authTicket.Version, 
                                                                                        authTicket.Name,
                                                                                        authTicket.IssueDate,
                                                                                        authTicket.Expiration,
                                                                                        authTicket.IsPersistent,
                                                                                        this.CurrentCookieData.ToString());
                string encryptedTicket = FormsAuthentication.Encrypt(newAuthTicket);
                authCookie.Value = encryptedTicket;
                Response.Cookies.Set(authCookie);
            }
            else
            {
                throw new NullReferenceException("Auth Cookie is null");
            }
         
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (this.CurrentCookieData != null && this.CurrentCookieData.IsForceAccountUpdate)
            {
                if (!(filterContext.Controller is AccountController &&
                        filterContext.ActionDescriptor.ActionName.Equals("UpdateAccount")))
                {
                    filterContext.Result = this.RedirectToAction("UpdateAccount", "Account", routeValues: new { returnUrl = this.Request.Url.PathAndQuery});
                }

            }
            EnsureUserCanAccessCurrentProjectOrFolder(filterContext);
            base.OnAuthorization(filterContext);


        }

        private void EnsureUserCanAccessCurrentProjectOrFolder(AuthorizationContext filterContext)
        {
            if (base.Request.IsAuthenticated && !filterContext.ActionDescriptor.ActionName.Equals("Error"))
            {
                if ((this.DocllerContext.ProjectId > 0 && !this.DocllerContext.Security.CanAccessCurrentProject)
                    || (this.DocllerContext.FolderContext != null && this.DocllerContext.FolderContext.CurrentFolderId > 0 &&
                    !this.DocllerContext.Security.CanAccessCurrentFolder))
                {
                    filterContext.Result = this.RedirectToAction("Error", new { message = "Your not authorized" });
                    
                }
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (base.Request.IsAuthenticated)
            {
                ViewBagWrapper.HeaderBrand = "Docller";
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    this.CurrentCookieData = new CookieData(authTicket.UserData);
                    InjectContext();
                }
                else
                {
                    throw new NullReferenceException("Auth Cookie is null");
                }
            }
        }

        protected ActionResult ContextDependentView(string masterLayout)
        {
            return ContextDependentView<object>(masterLayout, null);
        }

        protected ActionResult ContextDependentView()
        {
            return ContextDependentView<object>(null, null);
        }

        protected ActionResult ContextDependentView<T>(string masterLayout, T model) where T : class
        {
            string actionName = ControllerContext.RouteData.GetRequiredString("action");
            if (Request.QueryString["content"] != null)
            {
                ViewBagWrapper.FormAction = "Json" + actionName;
                ViewBagWrapper.IsDlg = true;
                return PartialView();
            }
            else
            {
                ViewResult view = model != null ? View(model) : View();
                if(!string.IsNullOrEmpty(masterLayout))
                {
                    view.MasterName = masterLayout;
                }
                ViewBagWrapper.FormAction = actionName;
                return view;
            }
        }

        public IDocllerContext DocllerContext
        {
            get { return Core.Common.DocllerContext.Current; }
        }

        protected void EnsureAnonymousContext()
        {
            if (!this.Request.IsAuthenticated)
            {
                //See if we have Anonymous cookie 
                //MachineKey.Protect(System.Text.ASCIIEncoding.ASCII.GetBytes(1.ToString()), "Anon CustomerId");
                HttpCookie anonCookie = Request.Cookies[Constants.AnonymouseCookieName];

                if (anonCookie == null)
                {
                    anonCookie = new HttpCookie(Constants.AnonymouseCookieName);
                    ISubscriptionService subscriptionService = ServiceFactory.GetSubscriptionService();
                    Customer customer = subscriptionService.GetCustomer(this.CurrentDomain);
                    this.CurrentCookieData = new CookieData() {CustomerId = customer.CustomerId};
                    anonCookie.Value = Security.Encrypt(this.CurrentCookieData.ToString());
                    Response.Cookies.Add(anonCookie);
                }
                else
                {
                    this.CurrentCookieData = new CookieData(Security.Decrypt<string>(anonCookie.Value));
                }
                InjectContext();
            }

        }

        protected void InjectContext()
        {
            Factory.GetInstance<IDocllerContext>().Inject(this);
        }

        protected void InjectContext(User user)
        {
            Factory.GetInstance<IDocllerContext>().Inject(this,user);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Error(string message)
        {
            ViewBagWrapper.ErrorMessage = string.IsNullOrEmpty(message)
                ? "An Error has occured"
                : HttpUtility.HtmlEncode(message);
            return View();
        }
    }
}

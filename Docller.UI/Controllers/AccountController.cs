using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Docller.Common;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Models;
using Docller.Core.Services;

namespace Docller.Controllers
{

    [Authorize]
    public class AccountController : DocllerControllerBase
    {

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            DocllerContext.Session.End();

            return RedirectToAction("LogOn");
        }

        //
        // GET: /Account/LogOn

        [AllowAnonymous]
        public ActionResult LogOn()
        {
            if (this.User.Identity.IsAuthenticated)
            {
               User user;
               return RedirectAuthUser(this.User.Identity.Name, this.CurrentCookieData.CustomerId, out user);
            }
            this.ViewBagWrapper.ReturnUrl = GetReturnUrl();
            return View();
        }

        //
        // POST: /Account/LogOn

        [AllowAnonymous]
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ISubscriptionService subscriptionService = ServiceFactory.GetSubscriptionService();
                Customer customer = subscriptionService.GetCustomer(this.CurrentDomain);

                IUserService userService = ServiceFactory.GetUserService();
                
                UserServiceStatus userServiceStatus = userService.LogOn(model.UserName, model.Password);

                if(userServiceStatus == UserServiceStatus.LoginSuccessAndForcePasswordChange)
                {
                    SetCookie(model, customer.CustomerId, true);
                    InjectContext();
                    CustomerCache.Set(customer);
                    return RedirectToAction("UpdateAccount", "Account", new {ReturnUrl = returnUrl});
                }
                else if (userServiceStatus == UserServiceStatus.LoginSuccess)
                {
                    User user = userService.GetUserInfo(model.UserName, customer.CustomerId);
                    if (user.FailedLogInAttempt > 0)
                    {
                        user.UserName = model.UserName;
                        user.FailedLogInAttempt = 0;
                        userService.UpdateUserFailedLoginAttempt(user);
                    }

                    SetCookie(model, customer.CustomerId, false);
                    User userInfo;
                    ActionResult results = RedirectAuthUser(model.UserName, customer.CustomerId, returnUrl, out userInfo);
                    InjectContext(userInfo);
                    CustomerCache.Set(customer);
                    return results;
                }
                if (userServiceStatus == UserServiceStatus.InvalidUserNameOrPassword)
                {
                    // Logic for Account Locking Functionality
                    User user = userService.GetUserInfo(model.UserName, customer.CustomerId);
                    if (user != null)
                    {
                        user.UserName = model.UserName;
                        user.FailedLogInAttempt = user.FailedLogInAttempt + 1;
                        if (user.FailedLogInAttempt == 5)
                        {
                            user.IsLocked = true;
                            userService.UpdateUserFailedLoginAttempt(user);
                            ModelState.AddModelError("", "Your account have locked. please reset your password using forgot your password link and after reset password please login again.");
                        }
                        else
                        {
                            userService.UpdateUserFailedLoginAttempt(user);
                            ModelState.AddModelError("", "The user name or password provided is incorrect.");
                        }
                    }
                    else { ModelState.AddModelError("", "The user name or password provided is incorrect."); }
                }

                if (userServiceStatus == UserServiceStatus.UserAccountLocked)
                {
                    ModelState.AddModelError("", "Your account have locked. please reset your password using forgot your password link and after reset password please login again.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

         [AllowAnonymous]
        public ActionResult Fubar(string key)
        {
            Response.Write(Config.GetValue<string>(key));
            
            Response.End();
            return null;
        }
        //

        //
        // GET: /Account/ChangePassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.Message = "";
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPassword(UserDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                IUserService userService = ServiceFactory.GetUserService();
                if (userService.IsUserExists(model.UserName))
                {
                    // do stuff here
                    string password = userService.ResetPassword(model.UserName);
                    if (password != String.Empty)
                    {
                        UserDetailsViewModel userdetails = new UserDetailsViewModel
                        {
                            Password = password,
                            UserName = model.UserName
                        };
                        // sent an email to user
                        new MailController().ForgotPasswordEmail(userdetails).Deliver();
                        ViewBag.Message = "An email with password reset instruction has sent to you. please follow the instruction.";
                    }
                }
                else { ModelState.AddModelError("", "If a valid email address was provided, Password reset instruction has been send to it"); }
            }
            return View(model);
        }

        public ActionResult UpdateAccount()
        {
            this.ViewBagWrapper.ReturnUrl = GetReturnUrl();
            this.ViewBagWrapper.Message = "Please provide your details before using Docller!";
            return ContextDependentView(Const.SimpleLayout);
        }

        [HttpPost]
        public ActionResult JsonUpdateAccount(UserViewModel userViewModel, string returnUrl)
        {
            return this.UpdateAccount(userViewModel)
                       ? Json(new { success = true })
                       : Json(new { errors = GetErrorsFromModelState() });
        }

        [HttpPost]
        public ActionResult UpdateAccount(UserViewModel userViewModel, string returnUrl)
        {
            User user;
            return this.UpdateAccount(userViewModel)
                       ? RedirectAuthUser(this.User.Identity.Name, this.CurrentCookieData.CustomerId, returnUrl, out user)
                       : ContextDependentView(Const.SimpleLayout, userViewModel);
        }

        private string GetReturnUrl()
        {
            return !string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]) &&
                                    !Request.QueryString["ReturnUrl"].Contains("logoff")
                                        ? Request.QueryString["ReturnUrl"]
                                        : "/";
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors
                .Select(error => error.ErrorMessage));
        }

        private void SetCookie(LogOnModel model, long customerId, bool forceUserUpdate)
        {
            this.CurrentCookieData = new CookieData()
                                  {
                                      CustomerId = customerId,
                                      IsForceAccountUpdate = forceUserUpdate
                                      
                                  };
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                1,
                model.UserName,
                DateTime.Now,
                DateTime.Now.AddDays(30),
                model.RememberMe,
                this.CurrentCookieData.ToString()
                );

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            if (authTicket.IsPersistent)
            {
                authCookie.Expires = authTicket.Expiration;
            }
            this.Response.Cookies.Add(authCookie);

        }


        private ActionResult RedirectAuthUser(string userName, long customerId, out  User user)
        {
            return RedirectAuthUser(userName, customerId, null, out user);
        }
        private ActionResult RedirectAuthUser(string userName, long customerId, string returnUrl, out User user)
        {
            IUserService userService = ServiceFactory.GetUserService();

            user = userService.GetUserInfo(userName, customerId);
            this.CurrentCookieData.CompanyName = string.IsNullOrEmpty(user.Company.CompanyAlias)
                                                     ? user.Company.CompanyName
                                                     : user.Company.CompanyAlias;
            this.CurrentCookieData.DisplayName = user.DisplayName;
            this.SaveCurrentCookieData();
            if (!string.IsNullOrEmpty(returnUrl) && !returnUrl.Equals("/"))
            {
                return RedirectTo(returnUrl);
            }
            if (user.Projects.Count == 1)
            {
                //redirect to Project homepage
                return RedirectToAction("FileRegister", "Project", new { user.Projects[0].ProjectId });

            }
            else if (user.Projects.Count > 1)
            {
                return RedirectToAction("Lists", "Project");
            }
            else if (user.IsCustomerAdmin)
            {
                return RedirectToAction("Create", "Project");
            }

            return null;
        }


        private ActionResult RedirectTo(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return null;
            }
        }

        private bool UpdateAccount(UserViewModel userViewModel)
        {
            User user = new User
            {
                UserName = this.User.Identity.Name,
                FirstName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                Password = userViewModel.Password
            };

            IUserService userService = ServiceFactory.GetUserService();
            UserServiceStatus status = userService.Update(user);
            if (status == UserServiceStatus.Success)
            {
                this.CurrentCookieData.DisplayName = user.DisplayName;
                this.CurrentCookieData.IsForceAccountUpdate = false;
                this.SaveCurrentCookieData();
                return true;
            }
            else
            {
                return false;
            }
        }
    }


}

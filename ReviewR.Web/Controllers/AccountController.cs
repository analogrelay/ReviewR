using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ReviewR.Web.Models;
using ReviewR.Web.Services;
using ReviewR.Web.ViewModels;

namespace ReviewR.Web.Controllers
{
    public class AccountController : Controller
    {
        private UrlService _defaultUrlService;
        private UrlService _urlService;

        // Lazy DI because I don't want to figure out how to get a per-request UrlHelper in to the container :)
        public UrlService UrlService
        {
            get
            {
                if (_urlService == null)
                {
                    if (_defaultUrlService == null)
                    {
                        _defaultUrlService = new UrlService(Url);
                    }
                    return _defaultUrlService;
                }
                return _urlService;
            }
            set { _urlService = value; }
        }

        public AuthenticationService AuthService { get; set; }
        public AuthTokenService TokenService { get; set; }

        public AccountController(AuthenticationService authService, AuthTokenService tokenService)
        {
            AuthService = authService;
            TokenService = tokenService;
        }

        public AccountController(AuthenticationService authService, AuthTokenService tokenService, UrlService urlService) : this(authService, tokenService)
        {
            UrlService = urlService;
        }

        [HttpGet]
        public ActionResult Login(string content)
        {
            var model = new LoginViewModel();
            if (!String.IsNullOrEmpty(content))
            {
                return PartialView(model);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl, bool isAjaxRequest)
        {
            if (ModelState.IsValid)
            {
                User user = AuthService.LogIn(model.Email, model.Password);
                if (user != null)
                {
                    TokenService.SetAuthCookie(model.Email, createPersistentCookie: model.RememberMe);
                    return CreateSuccessResult(returnUrl, isAjaxRequest);
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Either there is no user with that email address, or you entered the wrong password.");
                }
            }
            return CreateErrorResult(model, isAjaxRequest);
        }

        [HttpGet]
        public ActionResult Register(string content)
        {
            var model = new RegisterViewModel();
            if (!String.IsNullOrEmpty(content))
            {
                return PartialView(model);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model, string returnUrl, bool isAjaxRequest)
        {
            if (ModelState.IsValid)
            {
                CreateUserResult status = AuthService.CreateUser(model.Email, model.DisplayName, model.Password);
                if (status == CreateUserResult.Success)
                {
                    TokenService.SetAuthCookie(model.Email, createPersistentCookie: false);
                    return CreateSuccessResult(returnUrl, isAjaxRequest);
                }
                else
                {
                    ModelState.AddModelError("", "There is already a user with that email address.");
                }
            }
            return CreateErrorResult(model, isAjaxRequest);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private ActionResult CreateSuccessResult(string returnUrl, bool isAjaxRequest)
        {
            returnUrl = (!String.IsNullOrEmpty(returnUrl) && UrlService.IsLocalUrl(returnUrl)) ? returnUrl : UrlService.Action("Index", "Home");
            if (isAjaxRequest)
            {
                return Json(new { success = true, redirect = returnUrl });
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        private ActionResult CreateErrorResult(object model, bool isAjaxRequest)
        {
            if (isAjaxRequest)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(s => s.Errors.Select(e => e.ErrorMessage)).ToArray() });
            }
            else
            {
                return View(model);
            }
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}

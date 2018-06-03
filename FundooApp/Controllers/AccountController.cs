﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using FundooApp.Data.Models;
using FundooApp.Results;
using FundooApp.Service;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.Owin.Security.Cookies;
using System.Web.Security;
using FundooApp.Data.Log;
using System.IO;
using System.Net;

namespace FundooApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        WebApiController WebApiController = new WebApiController();
        public static string token = "";
        public static string CurrentUrl = "";
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // The Authorize Action is the end point which gets called when you access any
        // protected Web API. If the user is not logged in then they will be redirected to 
        // the Login page. After a successful login you can call a Web API.

        [HttpGet]
        public ActionResult Authorize()
        {
            var claims = new ClaimsPrincipal(User).Claims.ToArray();
            var identity = new ClaimsIdentity(claims, "Bearer");
            AuthenticationManager.SignIn(identity);
            var url = Request.Url.AbsoluteUri;
            return new EmptyResult();

        }



        [AllowAnonymous]
        [Route("Token")]
        public async Task<string> Token(LoginViewModel context)
        {
            try
            {

                string access_token = await WebApiController.GenerateTokenAsync(context);
                token = access_token;
                TempData["access_token"] = access_token;

                return access_token;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());
            }

            return "Error";
        }

        public async Task<int> ConsumePostApi(tblNote model, string token)
        {
            try
            {
                var client = new HttpClient();
                //var url = Request.Url;
                //var returnurl = "https://localhost:44399/api/NotesApi/AddNote";

                string returnurl = CurrentUrl + "/api/NotesApi/AddNote";

                client.BaseAddress = new Uri(returnurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var body = new List<KeyValuePair<string, string>>
                {
                        new KeyValuePair<string, string>("ID", Convert.ToString(model.ID)),
                        new KeyValuePair<string, string>("Content", model.Content),
                        new KeyValuePair<string, string>("UserID", Convert.ToString(model.UserID)),
                        new KeyValuePair<string, string>("Title", model.Title),
                        new KeyValuePair<string, string>("Mode",model.Mode),
                        new KeyValuePair<string,string>("IsPin",Convert.ToString(model.IsPin)),
                        new KeyValuePair<string,string>("ColorCode",(model.ColorCode)),
                        new KeyValuePair<string,string>("Reminder",(model.Reminder)),
                        new KeyValuePair<string,string>("IsArchive",Convert.ToString(model.IsArchive)),
                        new KeyValuePair<string,string>("IsActive",Convert.ToString(model.IsActive)),
                        new KeyValuePair<string,string>("IsDelete",Convert.ToString(model.IsDelete)),
                        new KeyValuePair<string,string>("IsTrash",Convert.ToString(model.IsTrash)),
                        new KeyValuePair<string,string>("ImageUrl",model.ImageUrl),
                        new KeyValuePair<string,string>("UserID",model.UserID)


                };
                var content = new FormUrlEncodedContent(body);
                HttpResponseMessage response = await client.PostAsync(returnurl, content);
                return 1;
            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());
                ex.ToString();
            }
            return 0;

        }

        public async Task<List<tblNote>> ConsumeApi(string returnUrl, string token, string userid)
        {
            var list = new List<tblNote>();
            try
            {
                var client = new HttpClient();
                //var url = Request.Url;
                //var returnurl = url.Scheme + "://" + url.Host + ":" + url.Port + "/api/NotesApi/GetNotes";

                string returnurl = CurrentUrl + "/api/NotesApi/GetNotes?userid=" + userid;



                client.BaseAddress = new Uri(returnurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                var response = await client.GetAsync(returnurl);
                var contents = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<tblNote>>(contents);

                foreach (tblNote item in data)
                {
                    list.Add(item);
                }

            }
            catch (Exception ex)
            {
                Logger.Write(ex.ToString());
                ex.ToString();
            }
            return list;
        }


        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            CurrentUrl = Request.Url.Scheme + "://" + Request.Url.Authority;

            return View();
        }


        // GET: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                TempData["InEmailPassword"] = "Invalid Email or Password";
                return View("Login");
            }
            else
            {
                var result = await WebApiController.Login(model, returnUrl);
                switch (result)
                {
                    case SignInStatus.Success:
                        TempData["Login"] = "Logged In Successfully";
                        TempData["access_token"] = token;
                        TempData["Email"] = model.Email;
                        AccountsRepository accountsRepository = new AccountsRepository();
                        ApplicationUser data = await accountsRepository.GetInfoAsync(model.Email);
                        TempData["UserID"] = data.Id;
                        return RedirectToAction("GetNotes", "Notes");
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        TempData["LoginFaliure"] = "Invalid Login attempt";
                        return View("Login");
                }
            }
        }

        //GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }


        // POST: /Account/Register
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                result = await WebApiController.Register(model);
                if (result.Succeeded)
                {
                    TempData["RegisterSuccess"] = "Registered Successfully";

                    return View("Login");
                }
                else
                {
                    TempData["EmailExists"] = "Email already Exists";
                    return View();
                }

            }

            TempData["Fields"] = "Enter all Fields Correctly";
            return View(model);
        }


        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            //var result = await UserManager.ConfirmEmailAsync(userId, code);
            var result = await WebApiController.ConfirmEmail(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");


        }


        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }


        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                await WebApiController.ForgotPassword(model);
                return RedirectToAction("ForgotPasswordConfirmation", "Account");

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult VerifyOTP()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await WebApiController.ResetPassword(model);
            if (result != "Error")
            {
                return View("ResetPasswordConfirmation");
            }
            return View("ResetPassword");

        }

        // GET: /Account/SendPhoneNumber
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> SendPhoneNumber(VerifyPhoneNumberBindingModel model)
        {
            if (ModelState.IsValid)
            {
                await WebApiController.SendPhoneNumber(model);

            }
            return View("Login");

        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //POST: /Account/VerifyPhoneNumber
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Error");
            }
            await WebApiController.VerifyPhoneNumber(model);
            return View("Login");

        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }
            TempData["Email"] = loginInfo.Email.ToString();

            string url = "";

            if (loginInfo.Login.LoginProvider == "Facebook")
            {
                var identifier = loginInfo.ExternalIdentity.FindFirstValue(ClaimTypes.NameIdentifier);
                var picture = $"https://graph.facebook.com/{identifier}/picture?type=large";
                string fileSaveLocation = HttpContext.Server.MapPath("~/Images");

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(picture), fileSaveLocation + "/" + loginInfo.DefaultUserName + ".jpg");
                    url = CurrentUrl + "/Images/" + loginInfo.DefaultUserName + ".jpg";

                }

            }
            
            // Sign in the user with this external login provider if the user already has a login
            var result = await WebApiController.ExternalLoginCallback(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    ViewBag.LoginProvider = loginInfo.Login.ProviderKey;

                    //var picture = $"https://graph.facebook.com/{identifier}/picture?type=large";
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email, Picture = url });
            }
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
            //return RedirectToAction("GetNotes", "Notes");

        }


        // POST:/Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Index", "Manage");
            //}
            if (ModelState.IsValid)
            {
                var result = await WebApiController.ExternalLoginConfirmation(model, returnUrl);
                if (result != null)
                {
                    return RedirectToAction("Index", "Home");
                    // return RedirectToAction("GetNotes", "Notes");

                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //POST:/Account/RegisterExternal
        [HttpPost]
        [System.Web.Http.HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [AllowAnonymous]
        public async Task<string> RegisterExternal(ExternalLoginBindingModel external)
        {
            //var info = await AuthenticationManager.GetExternalLoginInfoAsync();

            AccountsRepository accountsRepository = new AccountsRepository();
            var result = await accountsRepository.RegisterExternal(external);
            return result;
        }


        ////GET: /Account/GetInfo
        //[HttpGet]
        //[AllowAnonymous]
        //public List<tblNote> GetInfo(string UserID)
        //{
        //    var list = new List<tblNote>();

        //    AccountsRepository accountsRepository = new AccountsRepository();
        //    list = accountsRepository.GetInfo(UserID);
        //    return list;
        //}


        [AllowAnonymous]
        [HttpGet]
        public ActionResult Logout()
        {
            TempData.Remove("access_token");
            TempData.Remove("UserID");
            TempData.Remove("picture");

            FormsAuthentication.SignOut();
            Session.Abandon();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalBearer);
            AuthenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            //WebApiController.Logout();
            return View("Login");
        }


    }
}

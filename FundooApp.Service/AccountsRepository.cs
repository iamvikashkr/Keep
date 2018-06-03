using FundooApp.Data.Infrastructure;
using FundooApp.Data.Models;
using FundooApp.Service.UrlHelper1;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FundooApp.Service
{
    public class AccountsRepository
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;


        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public async Task<IdentityResult> Register(RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, Lastname = model.Lastname, Gender = model.Gender, BirthDate = model.BirthDate, Aadhar = model.Aadhar, PhoneNumber = model.PhoneNumber, Address1 = model.Address1, Address2 = model.Address2, city = model.city, District = model.District, PinCode = model.PinCode, State = model.State, Country = model.Country };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                try
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    var callbackUrl = urlHelper.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: "https");

                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    return IdentityResult.Success;
                }
                catch (System.Exception ex)
                {

                    ex.ToString();
                }
                //For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                //Send an email with this link
            }

            return IdentityResult.Failed("Email already exists");

        }


        public async Task<SignInStatus> Login(LoginViewModel model, string returnUrl)
        {

            //var user = await UserManager.FindByNameAsync(model.Email);
            //if (user != null)
            //{
            //    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
            //    {
            //        ViewBag.errorMessage = "You must have a confirmed email to log on.";
            //        return View("Error");
            //    }
            //}
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true


            SignInStatus result;
            try
            {

                result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                var data = await UserManager.FindByEmailAsync(model.Email);


                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }



        public async Task<IdentityResult> ConfirmEmail(string userId, string code)
        {
            IdentityResult result = null;
            try
            {
                result = await UserManager.ConfirmEmailAsync(userId, code);
                return result;
            }
            catch (Exception ex)
            {

                ex.ToString();
            }
            return result;
        }


        public async Task<string> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return "User does not exists";
            }
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var callbackUrl = urlHelper.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: "https");
            await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
            return "Check Your Email";
        }


        public async Task<string> SendPhoneNumber(VerifyPhoneNumberBindingModel model)
        {
            var userid = await UserManager.FindByEmailAsync(model.Email);

            if (userid == null)
            {
                return "The email is not a registered user !";
            }

            try
            {
                var phoneNumber = await UserManager.GetPhoneNumberAsync(userid.Id.ToString());

                if (phoneNumber == null)
                {
                    return "The phone number is not valid !";
                }

                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(userid.Id.ToString(), phoneNumber.ToString());

                await UserManager.SendSmsAsync(userid.Id.ToString(), "Your FundooApp mobile number verification, the current code is " + code);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return "Success";
        }

        //public void Logout()
        //{

        //    FormsAuthentication.SignOut();
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalBearer);
        //    AuthenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);
        //}

        public async Task<string> VerifyPhoneNumber(VerifyPhoneNumberBindingModel model)
        {
            var userid = await UserManager.FindByEmailAsync(model.Email);

            if (userid == null)
            {
                return "The email is not a registered user !";
            }

            var phoneNumber = await UserManager.GetPhoneNumberAsync(userid.Id.ToString());

            if (phoneNumber == null)
            {
                return "The phone number is not valid !";
            }

            var result = await UserManager.ChangePhoneNumberAsync(userid.Id.ToString(), phoneNumber, model.Code);

            if (result.Succeeded)
            {
                return "Success";
            }
            return "Error";

        }


        public async Task<string> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return "user does not exist";
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return "Password Changed";
            }
            return "Error";
        }

        public async Task<SignInStatus> ExternalLoginCallback(ExternalLoginInfo loginInfo, bool isPersistent)
        {
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return result;
            }
            var userdata = await UserManager.FindByEmailAsync(info.Email);
            if (userdata != null)
            {
                var ID = userdata.Id;



            }
            return result;

        }


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public async Task<string> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            IdentityResult result = null;
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return "Error";
            }
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email ,Picture=model.Picture};
            result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await UserManager.AddLoginAsync(user.Id, info.Login);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return model.Picture;
                }
            }
            return "Error";
        }


        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Email, ProviderKey, null, LoginProvider));
                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Email, UserName, null, LoginProvider));
                }
                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.Email);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Email)
                };
            }
        }

        public async Task<string> RegisterExternal(ExternalLoginBindingModel external)
        {
            var infos = await AuthenticationManager.GetExternalLoginInfoAsync();
            string info = Convert.ToString(AuthenticationManager.User.Identity.Name);
            if (info == null)
            {
                return "Error";
            }
            var userdata = await UserManager.FindByEmailAsync(info);

            if (userdata != null)
            {
                var ID = userdata.Id;
                return ID;
            }
            else
            {

            }
            return "Error";
        }

        public async Task<ApplicationUser> GetInfoAsync(string email)
        {

            ApplicationUser  data = await UserManager.FindByEmailAsync(email);

            return data;
        }

       


        [HttpPost]
        public async Task RegisterUpdate(string  url,string email)
        {
            ApplicationUser data = await UserManager.FindByEmailAsync(email);

            url = "https://localhost:44399/Images/"+url;
            data.Picture = url;
            var result1 = await UserManager.UpdateAsync(data);

            if (result1.Succeeded)
            {
                try
                {
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                   
                }
                catch (System.Exception ex)
                {

                    ex.ToString();
                }
                //For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                //Send an email with this link
            }


        }


    }
}

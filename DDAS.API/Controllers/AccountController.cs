using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using DDAS.Models;
using DDAS.API.Identity;
using DDAS.Models.Interfaces;
using DDAS.Models.ViewModels;
using DDAS.Models.Entities.Identity;
using Utilities.EMail;
using System.Linq;
using System.Globalization;

namespace DDAS.API.Controllers
{
    //[Authorize(Roles = "admin")]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private IUnitOfWork _UOW;
        private IUserService _userService;
        private IEMailService _EMailService;
        public AccountController(IUnitOfWork uow, IUserService userService, IEMailService email)
        {
              _UOW = uow;
            _userService = userService;
            _EMailService = email;
            
            //var cred = new EMailServerCredentialsModel();
            //cred.EMailHost = System.Configuration.ConfigurationManager.AppSettings["EMailHost"];
            //string port = System.Configuration.ConfigurationManager.AppSettings["EMailPort"];
            //cred.EMailPort = Int32.Parse(port);
            //cred.FromEMailId = System.Configuration.ConfigurationManager.AppSettings["FromEMailId"];
            //cred.FromEMailPassword = System.Configuration.ConfigurationManager.AppSettings["FromEMailPassword"];
            //_EMailService = new EMailService(cred);

        }

        #region WorkingCode

        
        //[Authorize(Roles = "admin")]
        [Route("GetUsers")]
        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            var Users = _userService.GetUsers();
            return Ok(Users);
        }

        [Route("GetAdminList")]
        [HttpGet]
        public IHttpActionResult GetAdminList()
        {
            var Users = _userService.GetUsers();

            var AdminList = Users.Where(x =>
            x.ActiveRoles.ToLower().Contains("admin")).ToList();

            return Ok(AdminList);
        }

        [Route("GetUser")]
        [HttpGet]
        public IHttpActionResult GetUser(string UserId)
        {
            if (UserId == null)
            {
                return Ok(_userService.GetNewUser());
            }
            else { 
                Guid gUserId = Guid.Parse(UserId);
                return Ok(_userService.GetUser(gUserId));
            }
        }

        [Route("SaveUser")]
        [HttpPost]
        public IHttpActionResult SaveUser(UserViewModel user)
        {
           return Ok( _userService.SaveUser(user));
        }

        [Route("DeleteUser")]
        [HttpDelete]
        public IHttpActionResult DeleteUser(Guid userId)
        {
            return Ok(_userService.DeleteUser(userId));
        }


        // POST api/Account/SetPassword
        [Route("SetPassword")]
        [HttpPost]
        public IHttpActionResult SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserStore userStore = new UserStore(_UOW);
            var userManager = new UserManager<IdentityUser, Guid>(userStore);

            String hashedNewPassword = userManager.PasswordHasher.HashPassword(model.NewPassword);
            var userId = User.Identity.GetUserId();
            var userName = User.Identity.GetUserName();
            IdentityUser user = userManager.Find(userName, model.CurrrentPassword);
            if (user == null)
            {
                return Ok("The current password is incorrect");
            }
            //var user = userManager.FindById(Guid.Parse(userId));
            userStore.SetPasswordHashAsync(user, hashedNewPassword);

            //IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
            //IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
            //if (!result.Succeeded)
            //{
            //    return GetErrorResult(result);
            //}

            return Ok("Password change successful");
        }


        [Route("ResetPassword")]
        [HttpGet]
        public IHttpActionResult ResetPassword(Guid userId)
        {
  
            UserStore userStore = new UserStore(_UOW);
            var userManager = new UserManager<IdentityUser, Guid>(userStore);

            var password = GeneratePassword();
            String hashedNewPassword = userManager.PasswordHasher.HashPassword(password);
            
            var user = userManager.FindById(userId);
            userStore.SetPasswordHashAsync(user, hashedNewPassword);

            var EMail = new EMailModel();
            EMail.To.Add(user.EmailId);
            EMail.Subject = "Password reset for Due Diligence Automation System Account";

            var htmlBody = "Dear " + user.UserName + ",<br/><br/> ";
            htmlBody += "Your password for Due Diligence Automation System was reset. <br/> ";
            htmlBody += "Your new password is : <b>" + password + "</b><br/><br/>";
            htmlBody += "Yours Sincerely,<br/>";
            htmlBody += "DDAS Team";

            EMail.Body = htmlBody;
            _EMailService.SendMail( EMail);

            return Ok(true);
        }
                
        [Route("getLogHistory")]
        [HttpGet]
        public IHttpActionResult GetAllLoginHistory(string DateFrom, string DateTo)
        {
            DateTime from;
            DateTime to;
            DateTime toPlusOne;
            //DateTime.TryParseExact(DateFrom, "yyyy-mm-dd", null,
            //    System.Globalization.DateTimeStyles.None, out from);
            DateTime.TryParse(DateFrom, out from);
            //DateTime.TryParseExact(DateTo, "yyyy-mm-dd", null,
            //  System.Globalization.DateTimeStyles.None, out to);
            DateTime.TryParse(DateTo, out to);
            toPlusOne = to.AddDays(1);
            var AllLoginHistory = _userService.GetAllLoginHistory()
                .Where(x =>  x.LoginAttemptTime >= from && x.LoginAttemptTime < toPlusOne
            )
            .OrderByDescending(d => d.LoginAttemptTime);
            
            return Ok(AllLoginHistory);

        }

        [Route("getMyLogHistory")]
        [HttpGet]
        public IHttpActionResult GetMyLoginHistory(string UserName, string DateFrom, string DateTo)
        {
            DateTime from;
            DateTime to;
            DateTime toPlusOne;
            //"M'/'d'/'yyyy"
            //DateTime.TryParseExact(DateFrom, "yyyy-mm-dd",   System.Globalization.CultureInfo.InvariantCulture,
            //    System.Globalization.DateTimeStyles.None, out from);

            DateTime.TryParse(DateFrom, out from);

            //DateTime.TryParseExact(DateTo, "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture,
            //   System.Globalization.DateTimeStyles.None, out to);

            DateTime.TryParse(DateTo, out to);

            toPlusOne = to.AddDays(1);

            var AllLoginHistory = _userService.GetAllLoginHistory();


            var MyLoginHistory = AllLoginHistory.Where(x =>
            x.UserName.ToLower() == UserName.ToLower()
            & x.LoginAttemptTime >= from && x.LoginAttemptTime < toPlusOne
            )
            .OrderByDescending(d => d.LoginAttemptTime);

            return Ok(MyLoginHistory);
        }

        //[Route("AddLoginDetails")]
        //[HttpGet]
        public IHttpActionResult AddLoginDetails(
            string UserName, bool IsLoginSuccessful)
        {
            //var LocalIPAddress =
            //    HttpContext.Current.Request.ServerVariables.Get("LOCAL_ADDR");

            //var HostIPAddress =
            //    HttpContext.Current.Request.ServerVariables.Get("REMOTE_ADDR");

            //var PortNumber =
            //    HttpContext.Current.Request.ServerVariables.Get("SERVER_PORT");

            //_userService.AddLoginDetails(
            //    UserName,
            //    LocalIPAddress,
            //    HostIPAddress,
            //    PortNumber,
            //    IsLoginSuccessful);

            return Ok();
        }
        #endregion

        #region FromAccountsController



        // POST api/Account/Logout
       
        [Route("Logout")]
        [HttpPost]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

  

        protected override void Dispose(bool disposing)
        {
            //if (disposing && _userManager != null)
            //{
            //    _userManager.Dispose();
            //    _userManager = null;
            //}

            base.Dispose(disposing);
        }
        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }


        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
        #endregion

        #region Helpers
        string GeneratePassword()
        {
            return System.Web.Security.Membership.GeneratePassword(5, 2);
        }
        #endregion

    }
}

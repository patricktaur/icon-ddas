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
using System.Collections.Generic;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Interfaces;
using DDAS.Models.ViewModels;
using DDAS.Models.Entities.Identity;
using System.Threading.Tasks;

namespace DDAS.API.Controllers
{
    //[Authorize(Roles = "admin")]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private IUnitOfWork _UOW;
        private IUserService _userService;
        public AccountController(IUnitOfWork uow, IUserService userService)
        {
              _UOW = uow;
            _userService = userService;
        }

        #region WorkingCode

        
        //[Authorize(Roles = "admin")]
        [Route("GetUsers")]
        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            return Ok(_userService.GetUsers());
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

            //Temp: until the email is ready, later only Ok must be returned.
            return Ok(password);
        }



        #endregion

        #region FromAccountsController



        // POST api/Account/Logout
        [Route("Logout")]
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

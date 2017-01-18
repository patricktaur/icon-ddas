using DDAS.API.Identity;
using DDAS.API.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using DDAS.Data.Mongo;
using DDAS.Services.UserService;

namespace DDAS.API
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }
        public static Func<UserManager<IdentityUser, Guid>> UserManagerFactory { get; set; }


        static Startup()
        {
            PublicClientId = "self";

            //???
            //ninject.mvc3 or ninject.mvc5 has to be included in the project, otherwise uow is assigned to null 
            //var uow = DependencyResolver.Current.GetService<IUnitOfWork>();
            var uow = new UnitOfWork("DefaultConnection");
            var userService = new UserService(uow);

            UserManagerFactory = () => new UserManager<IdentityUser, Guid>(new UserStore(uow));

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(
                    PublicClientId, UserManagerFactory, userService),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };
        }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);


        }
    }
}
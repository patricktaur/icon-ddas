using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using DDAS.API.Identity;
using DDAS.Models.Entities.Domain;
using DDAS.Models;
using DDAS.Data.Mongo;
using DDAS.API.Controllers;
using DDAS.Models.Interfaces;

namespace DDAS.API.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private readonly Func<UserManager<IdentityUser, Guid>> _userManagerFactory;

        private IUserService _UserService;
        private string _ClientVer = "T1.0.35";

        //public ApplicationOAuthProvider(string publicClientId, Func<UserManager<IdentityUser, Guid>> userManagerFactory)
        //{

        public ApplicationOAuthProvider(string publicClientId, 
            Func<UserManager<IdentityUser, Guid>> userManagerFactory,
            IUserService UserService)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            if (userManagerFactory == null)
            {
                throw new ArgumentNullException("userManagerFactory");
            }

            _publicClientId = publicClientId;
            _userManagerFactory = userManagerFactory;
            //temp - until Mongo Identity is implemented:
            //_UOW = new UnitOfWork("DefaultConnection");
            _UserService = UserService;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            using (UserManager<IdentityUser, Guid> userManager = _userManagerFactory())
            {
                try
                {
                    var form = await context.Request.ReadFormAsync();
                    var verSubmitted = form["Ver"];
                    
                    if (verSubmitted.Length != _ClientVer.Length ||  verSubmitted.Substring(0, _ClientVer.Length) != _ClientVer)
                    {
                        context.SetError(
                           "invalid_grant", "Incorrect version used.  The current version is: " + _ClientVer + "  Close the web page to clear the cache and reopen.");
                        return;
                    }

                    IdentityUser user = 
                        await userManager.FindAsync(context.UserName, context.Password);

                    var LocalIPAddress =
                        HttpContext.Current.Request.ServerVariables.Get("LOCAL_ADDR");

                    var HostIPAddress =
                        HttpContext.Current.Request.ServerVariables.Get("REMOTE_ADDR");

                    var PortNumber =
                        HttpContext.Current.Request.ServerVariables.Get("SERVER_PORT");

                    var ServerProtocol =
                        HttpContext.Current.Request.ServerVariables.Get("SERVER_PROTOCOL");

                    var ServerSoftware =
                        HttpContext.Current.Request.ServerVariables.Get("SERVER_SOFTWARE");

                    var HttpHost =
                        HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST");

                    var ServerName =
                        HttpContext.Current.Request.ServerVariables.Get("SERVER_NAME");

                    var GatewayInterface =
                        HttpContext.Current.Request.ServerVariables.Get("GATEWAY_INTERFACE");

                    var Https =
                        HttpContext.Current.Request.ServerVariables.Get("HTTPS");

                    if (user == null)
                    {
                        _UserService.AddLoginDetails(
                            context.UserName, 
                            LocalIPAddress,
                            HostIPAddress, 
                            PortNumber, 
                            false,
                            ServerProtocol,
                            ServerSoftware,
                            HttpHost,
                            ServerName,
                            GatewayInterface,
                            Https);

                        context.SetError(
                            "invalid_grant", "The user name or password is incorrect.");
                        return;
                    }

                    //if (user.Active == false)
                    //{
                    //    _UserService.AddLoginDetails(
                    //        context.UserName,
                    //        LocalIPAddress,
                    //        HostIPAddress,
                    //        PortNumber,
                    //        false,
                    //        ServerProtocol,
                    //        ServerSoftware,
                    //        HttpHost,
                    //        ServerName,
                    //        GatewayInterface,
                    //        Https);

                    //    context.SetError(
                    //        "invalid_grant", "Access denied.");
                    //    return;
                    //}

                    _UserService.AddLoginDetails(
                        context.UserName, 
                        LocalIPAddress,
                        HostIPAddress, 
                        PortNumber, 
                        true,
                        ServerProtocol,
                        ServerSoftware,
                        HttpHost,
                        ServerName,
                        GatewayInterface,
                        Https);

                    ClaimsIdentity oAuthIdentity = 
                        await userManager.CreateIdentityAsync(
                            user, context.Options.AuthenticationType);

                    var userRoles = userManager.GetRoles(user.Id);

                    //ClaimsIdentity cookiesIdentity = await userManager.CreateIdentityAsync(user,
                    //    CookieAuthenticationDefaults.AuthenticationType);

                    //Role Properties are added:
                    AuthenticationProperties properties = CreateProperties(user, userRoles);

                    //foreach (Role role in mongoUser.Roles)
                    //{
                    //    oAuthIdentity.AddClaim(new Claim("Role", role.Name));
                        
                    //}
 
                    AuthenticationTicket ticket = 
                        new AuthenticationTicket(oAuthIdentity, properties);

                    context.Validated(ticket);
                    //context.Request.Context.Authentication.SignIn(cookiesIdentity);
                }
                catch(Exception e)
                {
                    Console.Write("" + e);
                }
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

        //Added: to include user roles: temp until mongo Identity is implemented.
        public static AuthenticationProperties CreateProperties(IdentityUser user, IList<string> roles)
        {
            var userFullName = user.UserFullName != null ? user.UserFullName : "";

            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", user.UserName },
                
                { "userFullName", userFullName }
            };
            foreach (string role in roles)
            {
                data.Add(role, "Role");
            }
            return new AuthenticationProperties(data);
        }
    }
}
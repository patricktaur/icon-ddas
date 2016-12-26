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

namespace DDAS.API.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;
        private readonly Func<UserManager<IdentityUser, Guid>> _userManagerFactory;

        private IUnitOfWork _UOW;
        //public ApplicationOAuthProvider(string publicClientId, Func<UserManager<IdentityUser, Guid>> userManagerFactory)
        //{

        public ApplicationOAuthProvider(string publicClientId, Func<UserManager<IdentityUser, Guid>> userManagerFactory)
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
            _UOW = new UnitOfWork("");
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            using (UserManager<IdentityUser, Guid> userManager = _userManagerFactory())
            {

                try
                {
                    IdentityUser user = await userManager.FindAsync(context.UserName, context.Password);
                    

                    if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }

                    ClaimsIdentity oAuthIdentity = await userManager.CreateIdentityAsync(user,
                        context.Options.AuthenticationType);
                    var r = userManager.GetRoles(user.Id);



                    ClaimsIdentity cookiesIdentity = await userManager.CreateIdentityAsync(user,
                        CookieAuthenticationDefaults.AuthenticationType);

                    //AuthenticationProperties properties = CreateProperties(user.UserName);
                    //Modified for mongo roles
                    User mongoUser = _UOW.UserRepository.FindById(user.Id);
                    if (mongoUser == null)
                    {
                        throw new Exception("Unable to access user record from Mongo DB");
                    }

                    //Role Properties are added:
                    AuthenticationProperties properties = CreateProperties(mongoUser);

                    foreach (Role role in mongoUser.Roles)
                    {
                        oAuthIdentity.AddClaim(new Claim("Role", role.Name));
                        
                    }
 

                    AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                    context.Validated(ticket);
                    context.Request.Context.Authentication.SignIn(cookiesIdentity);
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
        public static AuthenticationProperties CreateProperties(User user)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", user.UserName },
                { "userFullName", user.UserFullName + "" }

            };
            foreach (Role role in user.Roles)
            {
                data.Add(role.Name, "Role");
            }
            return new AuthenticationProperties(data);
        }

    }
}
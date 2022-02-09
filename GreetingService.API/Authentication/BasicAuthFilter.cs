using GreetingService.Core;
using GreetingService.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace GreetingService.API.Authentication
{

    /// <summary>
    /// Inspiration from https://codeburst.io/adding-basic-authentication-to-an-asp-net-core-web-api-project-5439c4cf78ee
    /// 
    /// An IAuthorizationFilter is executed in the ASP.NET pipeline before the execution reaches our own code.
    /// A nice thing with ASP.NET is that it's possible to add our own custom logic to the pipeline. API authentication logic is a great example.
    /// Implementing IAuthorizationFilter requires only one method: OnAuthorization()
    /// This method is called for all incoming requests to methods with our custom attribute [BasicAuth]
    /// 
    /// </summary>

    public class BasicAuthFilter : IAuthorizationFilter
    {
        private readonly string _realm;

        public BasicAuthFilter(string realm)
        {
            _realm = realm;
            if (string.IsNullOrWhiteSpace(_realm))
            {
                throw new ArgumentNullException(nameof(realm), @"Please provide a non-empty realm value.");
            }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                string authHeader = context.HttpContext.Request.Headers["Authorization"];
                if (authHeader != null)
                {
                    var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                    if (authHeaderValue.Scheme.Equals(AuthenticationSchemes.Basic.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        var credentials = Encoding.UTF8
                                            .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                                            .Split(':', 2);
                        if (credentials.Length == 2)
                        {
                            if (IsAuthorized(context, credentials[0], credentials[1]))
                            {
                                return;
                            }
                        }
                    }
                }

                ReturnUnauthorizedResult(context);
            }
            catch (FormatException)
            {
                ReturnUnauthorizedResult(context);
            }
        }

        public bool IsAuthorized(AuthorizationFilterContext context, string username, string password)
        {
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            return userService.IsValidUser(username, password);
        }

        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            // Return 401 and a basic authentication challenge (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
            context.Result = new UnauthorizedResult();
        }
    }
}

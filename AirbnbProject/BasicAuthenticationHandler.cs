using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Text;

namespace AirbnbProject
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        // Core method that handles the authentication process
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Response.Headers.Add("WWW-Authenticate", "Basic");

            // Check if the Authorization header is missing
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header missing."));
            }

            // Extract the authorization header
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var authHeaderRegex = new Regex(@"Basic (.*)");

            // Check if the Authorization header is properly formatted
            if (!authHeaderRegex.IsMatch(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header not properly formatted."));
            }

            // Decode the base64 encoded credentials
            var authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1")));
            var authSplit = authBase64.Split(':', 2);
            var authUsername = authSplit[0];
            var authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Unable to get password");

            // Validate the credentials
            if ((authUsername == "admin" && authPassword == "admin") || (authUsername == "test" && authPassword == "test"))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, authUsername)
                };

                // Assign role based on username
                if (authUsername == "admin")
                {
                    claims.Add(new Claim(ClaimTypes.Role, "admin"));
                }
                else if (authUsername == "test")
                {
                    claims.Add(new Claim(ClaimTypes.Role, "user"));
                }

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Basic"));

                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, "Basic")));
            }

            return Task.FromResult(AuthenticateResult.Fail("The username or password is not correct."));
        }
    }

}

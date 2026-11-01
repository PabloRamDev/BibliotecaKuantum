using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace biblioteca_kuantum.Api.Auth
{
    public class ApiKeySchemeHandler : AuthenticationHandler<ApiKeySchemeOptions>
    {
        public ApiKeySchemeHandler(IOptionsMonitor<ApiKeySchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(Options.HeaderName, out var headerValue) || string.IsNullOrWhiteSpace(headerValue))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            //Constant-time comparison to avoid the key to be guessed through response timing
            var provided = Encoding.UTF8.GetBytes(headerValue.ToString());
            var expected = Encoding.UTF8.GetBytes(Options.ApiKey);

            if (!CryptographicOperations.FixedTimeEquals(provided, expected))
            {
                return Task.FromResult(AuthenticateResult.Fail("ApiKey no válida"));
            }

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "api-key-client"),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

    }
}

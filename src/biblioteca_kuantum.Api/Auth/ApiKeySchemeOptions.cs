using Microsoft.AspNetCore.Authentication;

namespace biblioteca_kuantum.Api.Auth
{
    public class ApiKeySchemeOptions : AuthenticationSchemeOptions
    {
        public const string Scheme = "ApiKeyScheme";

        //User secrets in development env var Authentication__ApiKey in Docker
        public const string ConfigurationKey = "Authentication:ApiKey";

        public string HeaderName { get; set; } = "X-Api-Key";

        public string ApiKey { get; set; } = string.Empty;
    }
}

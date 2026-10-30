using biblioteca_kuantum.Api.Auth;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace biblioteca_kuantum.Api.OpenApi
{
    //adds the api key security scheme to the OpenAPI document so Swagger UI shows an "Authorize" button. Made with Claude
    public class ApiKeySecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        private readonly IOptionsMonitor<ApiKeySchemeOptions> _options;

        public ApiKeySecuritySchemeTransformer(IOptionsMonitor<ApiKeySchemeOptions> options)
        {
            this._options = options;
        }

        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var headerName = _options.Get(ApiKeySchemeOptions.Scheme).HeaderName;

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes[ApiKeySchemeOptions.Scheme] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = headerName,
                Description = $"API key enviada en el header {headerName}"
            };


            document.Security ??= new List<OpenApiSecurityRequirement>();
            document.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(ApiKeySchemeOptions.Scheme, document)] = new List<string>()
            });

            return Task.CompletedTask;
        }
    }
}

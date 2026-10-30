using biblioteca_kuantum.DataAccessLayer;
using biblioteca_kuantum.BusinessLayer;
using biblioteca_kuantum.Api.Auth;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

//Add DataAccessLayer

builder.Services.AddDataAccessLayer(builder.Configuration);

//Add BusinessLayer

builder.Services.AddBusinessLayer();

//Add Controllers

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var apiKey = builder.Configuration[ApiKeySchemeOptions.ConfigurationKey];

if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException($"Valor de '{ApiKeySchemeOptions.ConfigurationKey}' no configurado");
}

builder.Services.AddAuthentication(ApiKeySchemeOptions.Scheme)
    .AddScheme<ApiKeySchemeOptions, ApiKeySchemeHandler>(ApiKeySchemeOptions.Scheme, options => options.ApiKey = apiKey);

//Add required authentication policy. Public routes must opt out with AllowAnonymous

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder(ApiKeySchemeOptions.Scheme)
        .RequireAuthenticatedUser()
        .Build());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "biblioteca kuantum v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();


app.Run();



using biblioteca_kuantum.Api.Auth;
using biblioteca_kuantum.Api.OpenApi;
using biblioteca_kuantum.BusinessLayer;
using biblioteca_kuantum.DataAccessLayer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

//Add DataAccessLayer

builder.Services.AddDataAccessLayer(builder.Configuration);

//Add BusinessLayer

builder.Services.AddBusinessLayer();

//Add Controllers

builder.Services.AddControllers();

builder.Services.AddProblemDetails();

//Add Healthchecks
builder.Services.AddHealthChecks();


//Display the apikey configuration on OpenApi
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<ApiKeySecuritySchemeTransformer>());

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

app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    
    app.UseDeveloperExceptionPage();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "biblioteca kuantum v1");
        options.EnablePersistAuthorization();
    });
}
//Map healthcheck endpoint
app.MapHealthChecks("/healthz").AllowAnonymous();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();


app.Run();



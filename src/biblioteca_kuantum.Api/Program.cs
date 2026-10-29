using biblioteca_kuantum.DataAccessLayer;
using biblioteca_kuantum.BusinessLayer;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "biblioteca kuantum v1");
    });
}

app.UseHttpsRedirection();


app.MapControllers();


app.Run();



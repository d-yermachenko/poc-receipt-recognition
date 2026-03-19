using System;
using Microsoft.AspNetCore.Http;
using Scalar;
using Scalar.AspNetCore;
using Microsoft.Extensions.Configuration;
using ReceiptRecognition.API.Endpoints;
using ReceiptRecognition.OllamaSharp;
using StackExchange.Redis;
using ReceiptRecognition.API.Application.Services;
using ReceiptRecognition.API.Application;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAntiforgery();
builder.Services.AddCors();

builder.AddServiceDefaults();

builder.Services.AddLogging();

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    string redisConnectionString = configuration.GetConnectionString("recognitionTasksStorage") ?? throw new InvalidOperationException("Redis connection string is not configured.");
    return ConnectionMultiplexer.Connect(redisConnectionString);
});
builder.Services.AddApplicationServices();
var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Receipt Recognition API";
    });
}

app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
    app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapRecognitionEndPoints();
app.UseAntiforgery();
app.UseExceptionHandler();



app.Run();


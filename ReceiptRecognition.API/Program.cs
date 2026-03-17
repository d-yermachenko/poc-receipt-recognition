using System;
using Microsoft.AspNetCore.Http;
using Scalar;
using Scalar.AspNetCore;
using Microsoft.Extensions.Configuration;
using ReceiptRecognition.API.Endpoints;
using ReceiptRecognition.OllamaSharp;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAntiforgery();

builder.AddServiceDefaults();

builder.Services.AddLogging();

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddOllamaSharpServices();

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

app.MapRecognitionEndPoints();
app.UseAntiforgery();
app.UseExceptionHandler();



app.Run();


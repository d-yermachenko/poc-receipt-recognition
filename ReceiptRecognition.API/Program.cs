
using Microsoft.AspNetCore.Mvc;

namespace ReceiptRecognition.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration
            .AddUserSecrets(typeof(Program).Assembly)
            .AddJsonFile("appsettings.json");
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapPost("/start-recognition", (HttpContext httpContext) =>
        {
            string securityKey = httpContext.Request.Headers["secutity-key"];
            if (securityKey != "here is basic security key, its okey for value and of this endpoint")
                return Results.Unauthorized();

            return Results.StatusCode(StatusCodes.Status501NotImplemented);
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi()
        .WithFormOptions(multipartBodyLengthLimit: 5 * 1024 * 1024);

        app.Run();
    }
}

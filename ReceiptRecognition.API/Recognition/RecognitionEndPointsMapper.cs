using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using ReceiptRecognition.API.Application.Services;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.API.Endpoints;

public static class RecognitionEndPointsMapper
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapRecognitionEndPoints()
        {
            endpoints.MapPost("/recognize",async ([FromFormAttribute] IFormFile file, [FromForm] SupportedReceiptType receiptType, [FromServices] IRecognitionDispatcher recognitionDispatcher, CancellationToken cancellationToken = default) => {
                
                RecognitionJob job = RecognitionJob.CreateFromData(file.OpenReadStream(), file.ContentType, CultureInfo.CurrentCulture, receiptType);
                await recognitionDispatcher.DispatchRecognitionJobAsync(job, cancellationToken);
                return Results.Accepted($"/recognitionStatus/{job.JobId}", new { jobId = job.JobId });

            })
                .DisableAntiforgery()
                .WithDisplayName("Recognize Receipt with Ollama")
                .WithDescription("Recognizes receipt with default ollama model")
                .WithRequestTimeout(TimeSpan.FromMinutes(10)); // Recognition can be a long process, especially for large receipts, so we set a longer timeout for this endpoint. But policy is prefered


            endpoints.MapGet("/recognitionStatus/{id:guid}", async ([FromRoute] Guid id, [FromServices] IRecognitionStatusNotificationService recognitionService) =>
            {
                var result = await recognitionService.GetStatusAsync(id);
                return Results.Ok(result);
            })
                .WithDisplayName("Get Recognition Status")
                .WithDescription("Gets the status and result of receipt recognition by id");


            return endpoints;
        }


    }
}

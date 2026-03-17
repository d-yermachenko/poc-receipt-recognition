using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.API.Endpoints;

public static class RecognitionEndPointsMapper
{
    extension(IEndpointRouteBuilder endpoints)
    {
        public IEndpointRouteBuilder MapRecognitionEndPoints()
        {
            endpoints.MapPost("/recognize",async ([FromFormAttribute] IFormFile file, [FromForm] SupportedReceiptType receiptType, [FromServices] IReceiptRecognitionService recognitionService, CancellationToken cancellationToken = default) => {
                var result = await recognitionService.GetReceiptData(file.OpenReadStream(), file.ContentType, CultureInfo.CurrentCulture, receiptType, cancellationToken);
                Results.Ok(result);
            })
                .DisableAntiforgery()
                .WithDisplayName("Recognize Receipt with Ollama")
                .WithDescription("Recognizes receipt with default ollama model")
                .WithRequestTimeout(TimeSpan.FromMinutes(10)); // Recognition can be a long process, especially for large receipts, so we set a longer timeout for this endpoint. But policy is prefered
            return endpoints;
        }
    }
}

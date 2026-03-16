using ReceiptRecognition.Core;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ReceiptRecognition.Ollama.Options;
using ReceiptRecognition.Ollama.DTOs;
using System.Globalization;
using ReceiptRecognition.Ollama.ReceiptSchemaProviders;

namespace ReceiptRecognition.Ollama.Services;



internal class OllamaImageRecognitionService(IHttpClientFactory httpClientFactory, IOptions<PureRecognitionOptions> optionsShot, IReceiptSchemaProviderFactory schemaFactory, ILogger<OllamaImageRecognitionService> logger) : IReceiptRecognitionService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public const string CompletionUrl = "/api/generate";

    public async  Task<ReceiptDto?> GetReceiptData(Stream imageStream, string imageMimeType, CultureInfo receiptCulture, SupportedReceiptType receiptType, CancellationToken cancellationToken)
    {
        using MemoryStream memoryStream = new();
        await imageStream.CopyToAsync(memoryStream, cancellationToken);
        string b64 = Convert.ToBase64String(memoryStream.ToArray());
        logger.LogInformation("Converting image to base64 string completed. Length: {Length}", b64.Length);
        var options = optionsShot.Value;
        var schemaProvider = schemaFactory.GetReceiptSchemaProvider(receiptType);
        dynamic receiptSchema = schemaProvider.ReceiptFormat;
        //string prompt = options.Prompt;
        string prompt = schemaProvider.GetDefaultPrompt(receiptCulture);
        var request = new OllamaRequest(options.Model, prompt, receiptSchema, new string[] { b64 })
        {
            Options = new
            {
                temperature = 0,
                seed = 42
            }
        };

        
        using HttpClient httpClient = httpClientFactory.CreateClient(nameof(OllamaImageRecognitionService));
        HttpResponseMessage result = await httpClient.PostAsync(CompletionUrl, request.ToHttpContent(), cancellationToken);
        string content = await result.Content.ReadAsStringAsync(cancellationToken);
        if (!result.IsSuccessStatusCode)
        {
            logger.LogError("Unsuccesfull request to image2text model. Status code: {StatusCode} \n {content}", result.StatusCode, content);
            return null;
        }
        OllamaResponse? response = JsonSerializer.Deserialize<OllamaResponse>(content, JsonOptions);
        if (string.IsNullOrWhiteSpace(response?.response))
            return null;
        ReceiptDto? receipt = schemaProvider.SerializeToReceipt(response?.response);
        return receipt;
    }
}

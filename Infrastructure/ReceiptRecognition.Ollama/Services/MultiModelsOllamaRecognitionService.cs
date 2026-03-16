using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReceiptRecognition.Core;
using ReceiptRecognition.Ollama.DTOs;
using ReceiptRecognition.Ollama.Options;
using ReceiptRecognition.Ollama.ReceiptSchemaProviders;

namespace ReceiptRecognition.Ollama.Services
{
    internal class MultiModelsOllamaRecognitionService(IHttpClientFactory httpClientFactory, IOptions<OcrInputOptions> optionsShot, IReceiptSchemaProviderFactory schemaFactory, ILogger<MultiModelsOllamaRecognitionService> logger) : IReceiptRecognitionService
    {
        private const string ReceiptTextSchema = """
        {
          "$schema": "http://json-schema.org/draft-07/schema#",
          "type": "object",
          "properties": {
            "receipt": {
              "type": "string"
            }
          },
          "required": [
            "receipt"
          ],
          "additionalProperties": false
        }
        """;

        public const string CompletionUrl = "/api/generate";

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };



        public async Task<ReceiptDto?> GetReceiptData(Stream imageStream, string imageMimeType, CultureInfo receiptCulture, SupportedReceiptType receiptType, CancellationToken cancellationToken)
        {
            string? receiptFormattedTest = await GetReceiptTextAsync(imageStream, imageMimeType, cancellationToken);
            if (string.IsNullOrEmpty(receiptFormattedTest))
                return null;
            ReceiptDto? receiptDto = await GetReceiptObjectAsync(receiptFormattedTest, receiptType, cancellationToken);
            return receiptDto;

        }

        private record ReceiptTextObject(string Receipt);

        public async Task<string?> GetReceiptTextAsync(Stream stream, string _, CancellationToken cancellationToken)
        {
            

            using MemoryStream memoryStream = new();
            await stream.CopyToAsync(memoryStream, cancellationToken);
            string b64 = Convert.ToBase64String(memoryStream.ToArray());
            logger.LogInformation("Converting image to base64 string completed. Image base64 string length: {Length}", b64.Length);
            dynamic? schema = JsonSerializer.Deserialize<dynamic>(ReceiptTextSchema);
            var options = optionsShot.Value;
            var request = new OllamaRequest(options.OCRModelName, options.OCRPromptInEnglish, schema, new string[] {b64})
            {
                Options = new
                {
                    temperature = 0.1,
                    seed = 42
                }
            };

            using HttpClient httpClient = httpClientFactory.CreateClient(nameof(OllamaImageRecognitionService));
            HttpResponseMessage result = await httpClient.PostAsync(CompletionUrl, JsonContent.Create(request), cancellationToken);
            string content = await result.Content.ReadAsStringAsync(cancellationToken);
            if (!result.IsSuccessStatusCode)
            {
                logger.LogError("Unsuccesfull request to image2text model. Status code: {StatusCode} \n {content}", result.StatusCode, content);
                return null;
            }
            OllamaResponse? ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(content, jsonOptions);
            if (ollamaResponse is null)
            {
                logger.LogError("Ai image2text response is null");
                return null;
            }
            string ollamaResponseText = JsonSerializer.Serialize(ollamaResponse with { response = "" });
            logger.LogInformation("Ollama image2text response obtained succesfully.\n {ollamaResponseText}", ollamaResponseText);

            if (string.IsNullOrEmpty(ollamaResponse.response))
            {
                logger.LogError("Ai response content is null");
                return null;
            }
            ReceiptTextObject? textObject = JsonSerializer.Deserialize<ReceiptTextObject>(ollamaResponse.response, jsonOptions);
            return textObject?.Receipt;
        }

        public async Task<ReceiptDto?> GetReceiptObjectAsync(string text, SupportedReceiptType receiptType, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var options = optionsShot.Value;
            string combinedPrompt = $"""
                {options.TextParserPromptInEnglish}

                Receipt text:
                -------
                {text}
                """;
            dynamic format = schemaFactory.GetReceiptSchemaProvider(receiptType).ReceiptFormat;
            var request = new OllamaRequest(options.TextParserModelName, combinedPrompt, format);

            using HttpClient httpClient = httpClientFactory.CreateClient(nameof(OllamaImageRecognitionService));
            HttpResponseMessage result = await httpClient.PostAsync(CompletionUrl, request.ToHttpContent(), cancellationToken);
            string content = await result.Content.ReadAsStringAsync(cancellationToken);
            if (!result.IsSuccessStatusCode)
            {
                logger.LogError("Unsuccesfull request to text2receipt model. Status code: {StatusCode} \n {content}", result.StatusCode, content);
                return null;
            }
            OllamaResponse? ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(content, jsonOptions);
            if (ollamaResponse is null)
            {
                logger.LogError("Ai response is null");
                return null;
            }
            string ollamaResponseData = JsonSerializer.Serialize(ollamaResponse with { response = "" });
            logger.LogInformation("Response for parsing text to image obtained. \n {ollamaResponseData} ", ollamaResponseData);

            if (string.IsNullOrEmpty(ollamaResponse.response))
            {
                logger.LogError("Ai response content is null");
                return null;
            }
            ReceiptDto? receiptDto = JsonSerializer.Deserialize<ReceiptDto?>(ollamaResponse.response);
            return receiptDto;

        }
    }

}

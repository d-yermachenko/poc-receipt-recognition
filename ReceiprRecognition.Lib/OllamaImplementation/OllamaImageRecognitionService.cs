using ReceiptRecognition.Lib;
using ReceiptRecognition.Core;
using Microsoft.Extensions.Options;
using System.Xml.Schema;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ReceiptRecognition.Lib.OllamaImplementation;



internal class OllamaImageRecognitionService(IHttpClientFactory httpClientFactory, IOptions<ImageInputOptions> optionsShot, ILogger<OllamaImageRecognitionService> logger) : IReceiptRecognitionService
{

    public const string CompletionUrl = "/api/generate";
    public async  Task<ReceiptDto?> GetReceiptData(Stream imageStream, string imageMimeType, CancellationToken cancellationToken)
    {
        using MemoryStream memoryStream = new();
        await imageStream.CopyToAsync(memoryStream, cancellationToken);
        string b64 = Convert.ToBase64String(memoryStream.ToArray());
        var options = optionsShot.Value;
        var request = new
        {
            model = options.Model,
            format = "json",
            stream = false,
            system = options.SystemPrompt,
            prompt = options.UserPrompt,
            options = new
            {
                temperature = 0,
            },
            images = new string[] {b64}
        };
        using HttpClient httpClient = httpClientFactory.CreateClient(nameof(OllamaImageRecognitionService));
        HttpResponseMessage result = await httpClient.PostAsync(CompletionUrl, JsonContent.Create(request), cancellationToken);
        string content = await result.EnsureSuccessStatusCode().Content.ReadAsStringAsync(cancellationToken);
        OllamaResponse? response = JsonSerializer.Deserialize<OllamaResponse>(content, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        ReceiptDto? receipt = JsonSerializer.Deserialize<ReceiptDto>(response?.response);
        return receipt;
    }
}

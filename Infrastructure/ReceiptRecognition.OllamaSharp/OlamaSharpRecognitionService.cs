using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;
using OllamaSharp.Tools;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.OllamaSharp;

public class OlamaSharpRecognitionService : IReceiptRecognitionService
{
    private readonly OllamaSharpOptions _options;
    private readonly IChatClient _client;

    public OlamaSharpRecognitionService(IChatClient chatClient, IOptions<OllamaSharpOptions> options)
    {
        _options = options.Value;
        _client = chatClient;   
    }

    public async Task<ReceiptDto?> GetReceiptData(Stream imageStream, string imageMimeType, CultureInfo receiptCulture, SupportedReceiptType receiptType = SupportedReceiptType.Unknown, CancellationToken cancellationToken = default)
    {
        var messageContent = await GetImageBytesAsync(imageStream);


       ChatMessage systemPrompt = new ChatMessage(ChatRole.System, _options.SuffixSchemaToPrompt(GetExpectedObjectJsonSchema()));
       ChatMessage userMessage = new ChatMessage(ChatRole.User, $"Recognize given receipt as {receiptType.ToString()}, for culture {receiptCulture.Name}");
       userMessage.Contents.Add(new DataContent(messageContent, imageMimeType));
        ChatOptions chatOptions = new ChatOptions()
        {
            Temperature = 0,
            Seed = 42
        };

        var response = await _client.GetResponseAsync<ReceiptDto>([ systemPrompt, userMessage ], chatOptions, true, cancellationToken);
        return response.Result;

    }

    private static async Task<byte[]> GetImageBytesAsync(Stream imageStream, CancellationToken cancellationToken= default)
    {
            using MemoryStream memoryStream = new();
            await imageStream.CopyToAsync(memoryStream, cancellationToken);
            return memoryStream.ToArray();
    }

    private string GetExpectedObjectJsonSchema()
    {
        JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerOptions.Default)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        JsonNode jsonNode = jsonSerializerOptions.GetJsonSchemaAsNode(typeof(ReceiptDto));
        return jsonNode.ToString();
    }
}

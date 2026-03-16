using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ReceiptRecognition.Ollama.DTOs;

internal class OllamaRequest(string model, string prompt, dynamic? schema = null, string[]? base64Attachments = null)
{
    /// <summary>
    /// The model name
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Model { get; init; } = model;

    /// <summary>
    /// The prompt to generate a response for
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Prompt { get; init; } = prompt;

    /// <summary>
    /// The text after the model response
    /// </summary>
    public string? Suffix { get; init; } = default;

    /// <summary>
    /// (optional) A list of base64-encoded images (for multimodal models such as llava)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyCollection<string>? Images { get; init; } = base64Attachments;

    /// <summary>
    /// Output format. Can be simple json or valid json Schema
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public dynamic Format { get; set; } = schema ?? "json";

    /// <summary>
    /// Options
    /// Additional model parameters listed in the documentation for the Modelfile such as temperature
    /// </summary>
    /// <example>
    /// "options": {
    ///    "num_keep": 5,
    ///    "seed": 42,
    ///    "num_predict": 100,
    ///    "top_k": 20,
    ///    "top_p": 0.9,
    ///    "min_p": 0.0,
    ///    "typical_p": 0.7,
    ///    "repeat_last_n": 33,
    ///    "temperature": 0.8,
    ///    "repeat_penalty": 1.2,
    ///    "presence_penalty": 1.5,
    ///    "frequency_penalty": 1.0,
    ///    "penalize_newline": true,
    ///    "stop": ["\n", "user:"],
    ///    "numa": false,
    ///    "num_ctx": 1024,
    ///    "num_batch": 2,
    ///    "num_gpu": 1,
    ///    "main_gpu": 0,
    ///    "use_mmap": true,
    ///    "num_thread": 8
    ///  }
    /// </example>
    /// <example>
    /// new { temperature = 0.1, num_ctx = 4096 }
    /// </example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public dynamic? Options { get; set; } = null;

    /// <summary>
    /// system message to (overrides what is defined in the Modelfile)
    /// </summary>
    /// <seealso cref="https://github.com/ollama/ollama/blob/main/docs/modelfile.md#valid-parameters-and-values"/>
    /// <example>
    /// </example>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public dynamic? System { get; set; } = null;

    /// <summary>
    /// The prompt template to use (overrides what is defined in the Modelfile)
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public dynamic? Template { get; set; }

    /// <summary>
    /// If false the response will be returned as a single response object, rather than a stream of objects
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Stream { get; set; } = false;

    /// <summary>
    /// If true no formatting will be applied to the prompt. You may choose to use the raw parameter if you are specifying a full templated prompt in your request to the API
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Raw { get; set; } = false;

    /// <summary>
    /// Controls how long the model will stay loaded into memory following the request (default: 5m)
    /// </summary>
    [JsonPropertyName("keep_alive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [DefaultValue("10m")]
    public string KeepAlive = "10m";


    public HttpContent ToHttpContent()
    {
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        };
        return JsonContent.Create(this, options: jsonSerializerOptions);
    }
}

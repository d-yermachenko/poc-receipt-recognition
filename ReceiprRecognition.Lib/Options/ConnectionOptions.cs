using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ReceiptRecognition.Ollama.Options;

public sealed class ConnectionOptions 
{
    public string OllamaUri { get; set; } = "";
}

public sealed class ConnectionOptionsSetup(IConfiguration configuration) : IConfigureOptions<ConnectionOptions>
{
    public void Configure(ConnectionOptions options) => configuration.GetSection(nameof(ConnectionOptions)).Bind(options);
}









using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ReceiptRecognition.AzureAI;

public class AzureOptions
{
    public string Key { get; set; } = "";

    public string Endpoint { get; set; } = "";
}

public class AzureOptionsSetup(IConfiguration configuration) : IConfigureOptions<AzureOptions>
{
    public void Configure(AzureOptions options) => configuration.GetSection(nameof(AzureOptions)).Bind(options);
 
}



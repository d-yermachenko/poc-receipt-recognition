using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReceiptRecognition.Ollama;
using ReceiptRecognition.Ollama.Options;
using ReceiptRecognition.Tests.Receipts;
using ReceiptRecognition.OllamaSharp;

namespace ReceiptRecognition.Tests.Ollama;

internal static class OllamaSharpServiceContainerProvider
{
    

    static IServiceProvider? ollamaServiceProvider = null;
    
    public static IServiceProvider GetOllamaRecognitionServices()
    {
        if (ollamaServiceProvider is not null)
            return ollamaServiceProvider;
        IEnumerable<KeyValuePair<string, string?>> configuration = new Dictionary<string, string?>()
        {
            [$"{nameof(OllamaSharpOptions)}:{nameof(OllamaSharpOptions.OllamaApiUrl)}"] = "http://192.168.0.132:11434",
            [$"{nameof(OllamaSharpOptions)}:{nameof(OllamaSharpOptions.OllamaModelName)}"] = "llama3.2-vision:latest"
        };
        IServiceCollection serviceContainer = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configuration)
            .Build();
        serviceContainer.AddScoped<ReceiptsProvider>();
        serviceContainer.AddSingleton<IConfiguration>(config);
        serviceContainer.AddLogging();
        serviceContainer.AddOllamaSharpServices();
        ollamaServiceProvider = serviceContainer.BuildServiceProvider();
        return ollamaServiceProvider;
    }
}

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

namespace ReceiptRecognition.Tests.Ollama;

internal static class ServiceContainerProvider
{
    

    static IServiceProvider? ollamaServiceProvider = null;
    
    public static IServiceProvider GetOllamaRecognitionServices()
    {
        if (ollamaServiceProvider is not null)
            return ollamaServiceProvider;
        IEnumerable<KeyValuePair<string, string?>> configuration = new Dictionary<string, string?>()
        {
            [$"{nameof(ConnectionOptions)}:{nameof(ConnectionOptions.OllamaUri)}"] = "http://192.168.0.132:11434",
            //[$"{nameof(PureRecognitionOptions)}:{nameof(PureRecognitionOptions.Model)}"] = "qwen2.5vl:7b"
        };
        IServiceCollection serviceContainer = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configuration)
            .Build();
        serviceContainer.AddScoped<ReceiptsProvider>();
        serviceContainer.AddSingleton<IConfiguration>(config);
        serviceContainer.AddLogging();
        serviceContainer.AddOllamaReceiptService();
        ollamaServiceProvider = serviceContainer.BuildServiceProvider();
        return ollamaServiceProvider;
    }


    


}

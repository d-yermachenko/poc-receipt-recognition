
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReceiptRecognition.Core;
using ReceiptRecognition.Ollama.OllamaImplementation;

namespace ReceiptRecognition.Lib.OllamaImplementation;

public static class OllamaDIExtension
{
    public const string ImageRecognitionServiceKey = "SimpleRecognition";
    public const string CombinedRecognitionServiceKey = "CombinedRecognition";

    public static IServiceCollection AddOllamaReceiptService(this IServiceCollection services)
    {
        services.ConfigureOptions<ConnectionOptionsSetup>();
        services.ConfigureOptions<ImageInputOptionsSetup>();
        services.ConfigureOptions<OcrInputOptionsSetup>();
        services.AddHttpClient(nameof(OllamaImageRecognitionService), (sp, cli) =>
        {
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
            ConnectionOptions connectionOptions = sp.GetRequiredService<IOptions<ConnectionOptions>>().Value;
            cli.BaseAddress = new Uri(connectionOptions.OllamaUri);
            cli.Timeout = TimeSpan.FromMinutes(5);
        });
        services.AddHttpClient(nameof(MultiModelsOllamaRecognitionService), (sp, cli) =>
        {
            IConfiguration configuration = sp.GetRequiredService<IConfiguration>();
            ConnectionOptions connectionOptions = sp.GetRequiredService<IOptions<ConnectionOptions>>().Value;
            cli.BaseAddress = new Uri(connectionOptions.OllamaUri);
            cli.Timeout = TimeSpan.FromMinutes(5);
        });
        services.AddKeyedTransient<IReceiptRecognitionService, OllamaImageRecognitionService>(ImageRecognitionServiceKey);
        services.AddKeyedTransient<IReceiptRecognitionService, MultiModelsOllamaRecognitionService>(CombinedRecognitionServiceKey);

        return services;
    }
}

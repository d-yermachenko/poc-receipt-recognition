using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReceiptRecognition.Tests.Receipts;
using ReceiptRecognition.AzureAI;

namespace ReceiptRecognition.Tests.Azure;

internal static class ServiceContainerProvider
{
    

    static IServiceProvider? serviceProvider = null;
    
    public static IServiceProvider GetRecognitionServices()
    {
        if (serviceProvider is not null)
            return serviceProvider;
        IEnumerable<KeyValuePair<string, string?>> configuration = new Dictionary<string, string?>()
        {
        };
        IServiceCollection serviceContainer = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddUserSecrets(typeof(ServiceContainerProvider).Assembly)
            .AddInMemoryCollection(configuration)
            .Build();
        serviceContainer.AddScoped<ReceiptsProvider>();
        serviceContainer.AddSingleton(typeof(IConfiguration), config);
        serviceContainer.AddLogging();
        serviceContainer.AddAzureRecognizer();
        serviceProvider = serviceContainer.BuildServiceProvider();
        return serviceProvider;
    }


    


}

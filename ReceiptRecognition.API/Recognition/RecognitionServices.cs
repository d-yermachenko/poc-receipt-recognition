using ReceiptRecognition.Core;
using ReceiptRecognition.OllamaSharp;



namespace ReceiptRecognition.API.Recognition;

public static class RecognitionServices
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRecognitionServices()
        {
            services.AddOllamaSharpServices();
            return services;
        }
    }
}

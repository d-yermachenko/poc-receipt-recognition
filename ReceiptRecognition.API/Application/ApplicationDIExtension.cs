using ReceiptRecognition.API.Application.Services;
using ReceiptRecognition.OllamaSharp;

namespace ReceiptRecognition.API.Application;

public static class ApplicationDIExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddOllamaSharpServices();
            services.AddScoped<IRecognitionDispatcher, RecognitionDispatcher>();
            services.AddScoped<IRecognitionStatusNotificationService, RecognitionStatusNotificationService>();
            services.AddScoped<RecognitionHandler>();
            return services;
        }
    }
}

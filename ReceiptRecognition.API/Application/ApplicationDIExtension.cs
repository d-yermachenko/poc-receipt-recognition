using ReceiptRecognition.API.Application.Abstraction;
using ReceiptRecognition.API.Application.Data;
using ReceiptRecognition.API.Application.Services;
using ReceiptRecognition.OllamaSharp;

namespace ReceiptRecognition.API.Application;

public static class ApplicationDIExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddOllamaSharpServices(ServiceLifetime.Singleton);
            services.AddScoped<IRecognitionDispatcher, RecognitionDispatcher>();
            services.AddScoped<IRecognitionStatusNotificationService, RecognitionStatusNotificationService>();
            services.AddScoped<RecognitionHandler>();
            services.AddInMemoryMessageQueue();
            services.AddHostedService<RecognitionJobProcessor>();

 
            return services;
        }

        private IServiceCollection AddInMemoryMessageQueue()
        {
            services.AddSingleton(typeof(IJobQueue<>), typeof(InMemoryJobQueue<>));
            services.AddScoped<IJobPublisher<RecognitionJob>>(sp => sp.GetRequiredService<IJobQueue<RecognitionJob>>());
            services.AddSingleton<IJobConsumer<RecognitionJob>>(sp => sp.GetRequiredService<IJobQueue<RecognitionJob>>());
            return services;
        }
    }
}

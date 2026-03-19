using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ReceiptRecognition.Core;
using OllamaSharp.Tools ;
using OllamaSharp;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ReceiptRecognition.OllamaSharp;

public static class OllamaSharpExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOllamaSharpServices(ServiceLifetime serviceLifetime)
        {
            services.ConfigureOptions<OllamaSharpOptionsSetup>();
            services.AddHttpClient<OllamaApiClient>((sp, cli) =>
            {
                OllamaSharpOptions options = sp.GetRequiredService<IOptions<OllamaSharpOptions>>().Value;
                cli.BaseAddress = new Uri(options.OllamaApiUrl);
                cli.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            });
            services.AddChatClient((sp) =>
            {
                IHttpClientFactory httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var ollamaHttpClient = httpClientFactory.CreateClient(nameof(OllamaApiClient));
                OllamaSharpOptions options = sp.GetRequiredService<IOptions<OllamaSharpOptions>>().Value;
                var client= new OllamaApiClient(ollamaHttpClient, options.OllamaModelName);
                return client;
            }, serviceLifetime);
            services.Add(new ServiceDescriptor(typeof(IReceiptRecognitionService), typeof(OlamaSharpRecognitionService), serviceLifetime));
            return services;
        }
        
    }
}

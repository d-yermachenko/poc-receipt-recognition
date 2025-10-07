using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.AzureAI
{
    public static class AzureDIExtension
    {
        public static IServiceCollection AddAzureRecognizer(this IServiceCollection services)
        {
            services.ConfigureOptions<AzureOptionsSetup>();
            services.AddScoped<IReceiptRecognitionService, AzureDocumentAiService>();
            return services;
        }
    }
}

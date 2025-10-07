using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReceiptRecognition.Core;
using ReceiptRecognition.Lib.OllamaImplementation;

namespace ReceiptRecognition.Ollama.OllamaImplementation
{
    internal class MultiModelsOllamaRecognitionService(IHttpClientFactory httpClientFactory, IOptions<ImageInputOptions> optionsShot, ILogger<OllamaImageRecognitionService> logger) : IReceiptRecognitionService
    {
        public async Task<ReceiptDto?> GetReceiptData(Stream imageStream, string imageMimeType, CancellationToken cancellationToken)
        {
            string receiptFoemattedTest = await GetReceiptTextAsync(imageStream, imageMimeType, cancellationToken);
            ReceiptDto? receiptDto = await GetReceiptObjectAsync(receiptFoemattedTest, cancellationToken);
            return receiptDto;

        }

        public async Task<string> GetReceiptTextAsync(Stream stream, string imageMimeType, CancellationToken cancellationToken)
        {
            
            cancellationToken.ThrowIfCancellationRequested();
            throw new NotImplementedException();
        }

        public async Task<ReceiptDto?> GetReceiptObjectAsync(string text, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}

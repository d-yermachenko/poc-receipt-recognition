
using Microsoft.Extensions.DependencyInjection;
using ReceiptRecognition.Core;
using ReceiptRecognition.Lib.OllamaImplementation;
using ReceiptRecognition.Tests.Receipts;

namespace ReceiptRecognition.Tests.Ollama;

public class OllamaRecognitionServiceTests
{


    [Theory]
    [InlineData(OllamaDIExtension.ImageRecognitionServiceKey)]
    public async Task RecognizeKiabiReceipt(string serviceKey)
    {
        IServiceProvider serviceContainer = ServiceContainerProvider.GetOllamaRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredKeyedService<IReceiptRecognitionService>(serviceKey);
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(ReceiptsProvider.Shops.Kiabi);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Equal("kiabi", dto.MerchantName?.ToLower());
        Assert.Contains("pantalon", dto.Items.Select(x => x.Description?.ToLower()));

    }

    [Theory]
    [InlineData(OllamaDIExtension.ImageRecognitionServiceKey)]
    public async Task RecognizeAldiReceipt(string serviceKey)
    {
        IServiceProvider serviceContainer = ServiceContainerProvider.GetOllamaRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredKeyedService<IReceiptRecognitionService>(serviceKey);
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(ReceiptsProvider.Shops.Aldi);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Equal("aldi", dto.MerchantName?.ToLower());
        Assert.Contains("pain complet", dto.Items.Select(x => x.Description?.ToLower()));

    }

    [Theory]
    [InlineData(OllamaDIExtension.ImageRecognitionServiceKey)]
    public async Task RecognizeAlpeBureauReceipt(string serviceKey)
    {
        IServiceProvider serviceContainer = ServiceContainerProvider.GetOllamaRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredKeyedService<IReceiptRecognitionService>(serviceKey);
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(ReceiptsProvider.Shops.Kiabi);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Equal("alpes bureau ville la grand", dto.MerchantName?.ToLower());
        Assert.Contains("surligneur boss lilas", dto.Items.Select(x => x.Description?.ToLower()));

    }
}

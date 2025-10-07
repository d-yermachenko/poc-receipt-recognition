
using Microsoft.Extensions.DependencyInjection;
using ReceiptRecognition.Core;
using ReceiptRecognition.Tests.Receipts;

namespace ReceiptRecognition.Tests.Azure;

public class AzureRecognitionServiceTests
{
    

    [Fact]
    public async Task RecognizeKiabiReceipt()
    {
        IServiceProvider serviceContainer = ServiceContainerProvider.GetRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredService<IReceiptRecognitionService>();
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(ReceiptsProvider.Shops.Kiabi);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Equal("kiabi", dto.MerchantName?.ToLower());
        Assert.Contains("pantalon", dto.Items.Select(x => x.Description?.ToLower()));

    }

    [Fact]
    public async Task RecognizeAldiReceipt()
    {
        IServiceProvider serviceContainer = ServiceContainerProvider.GetRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredService<IReceiptRecognitionService>();
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(ReceiptsProvider.Shops.Aldi);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Equal("aldi", dto.MerchantName?.ToLower());
        Assert.Contains("pain complet", dto.Items.Select(x => x.Description?.ToLower()));

    }

    [Fact]
    public async Task RecognizeAlpeBureauReceipt()
    {
        IServiceProvider serviceContainer = ServiceContainerProvider.GetRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredService<IReceiptRecognitionService>();
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(ReceiptsProvider.Shops.Kiabi);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Equal("alpes bureau ville la grand", dto.MerchantName?.ToLower());
        Assert.Contains("surligneur boss lilas", dto.Items.Select(x => x.Description?.ToLower()));

    }
}

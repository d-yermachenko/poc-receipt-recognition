
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using ReceiptRecognition.Core;
using ReceiptRecognition.Tests.Receipts;

namespace ReceiptRecognition.Tests.Azure;

public class AzureRecognitionServiceTests
{
    [Theory]
    [InlineData(ReceiptsProvider.Shops.Kiabi, "kiabi", "m pantalon", 30, 1, 30)]
    [InlineData(ReceiptsProvider.Shops.AlpesBureau, "Alpes bureau Ville la Grand", "surligneur boss vert", 1.25, 1, 15.39)]
    [InlineData(ReceiptsProvider.Shops.Aldi, "Aldi", "pain complet", 1.35, 1,19.07 )]
    public async Task AzureRecognition(string receipt, string merchantName, string articleName, decimal expectedPrice, decimal expectedQuantity, decimal total)
    {
        IServiceProvider serviceContainer = ServiceContainerProvider.GetRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredService<IReceiptRecognitionService>();
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(receipt);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", CultureInfo.CreateSpecificCulture("fr-FR"), cancellationToken: CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Equal(merchantName, dto.MerchantName, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
        Assert.Equal(new Currency(total, "EUR"), dto.Total);
        Assert.Contains(dto.Items,
            x =>
                x.Description?.Equals(articleName, StringComparison.InvariantCultureIgnoreCase) ?? false
                && x.Quantity == expectedQuantity
                && x.UnitPrice == expectedPrice
                );

    }
}

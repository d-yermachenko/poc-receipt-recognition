using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ReceiptRecognition.Core;
using ReceiptRecognition.Ollama;
using ReceiptRecognition.Tests.Receipts;

namespace ReceiptRecognition.Tests.Ollama;

public class OllamaRecognitionServiceTests
{


    [Theory]
    [InlineData(ReceiptsProvider.Shops.France.Kiabi, "kiabi", "m pantalon", 30, 1, 30)]
    [InlineData(ReceiptsProvider.Shops.France.AlpesBureau, "Alpes bureau Ville la Grand", "surligneur boss vert", 1.25, 1, 15.39)]
    [InlineData(ReceiptsProvider.Shops.France.Aldi, "Aldi", "pain complet", 1.35, 1, 19.07)]
    public async Task RetailReceiptFrench(string receipt, string merchantName, string articleName, decimal expectedPrice, decimal expectedQuantity, decimal total, SupportedReceiptType receiptType = SupportedReceiptType.Retail)
    {

        IServiceProvider serviceContainer = ServiceContainerProvider.GetOllamaRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredKeyedService<IReceiptRecognitionService>(OllamaDIExtension.ImageRecognitionServiceKey);
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(receipt);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", receiptCulture: new("fr-FR"), receiptType, CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Equal(merchantName, dto.MerchantName, ignoreCase: true, ignoreWhiteSpaceDifferences: true);
        Assert.Equal(total, dto.Total?.Amount);
        Assert.Contains(dto.Items,
            x =>
                x.Description?.Equals(articleName, StringComparison.InvariantCultureIgnoreCase)??false
                && x.Quantity == expectedQuantity
                && x.UnitPrice?.Amount == expectedPrice
                );
    }

    
    [Theory]
    [InlineData(ReceiptsProvider.GasStations.IntermarcheResized, "Intermarché", "E10", 1.639, 22.08, 36.19)]
    [InlineData(ReceiptsProvider.GasStations.IntermarcheRaw, "Intermarché", "SP98", 1.725, 19.54, 33.71)]
    public async Task GasStationReceiptFrench(string receipt, string merchantName, string fuelType, decimal price, decimal volume, decimal total)
    {
        IServiceProvider serviceContainer = ServiceContainerProvider.GetOllamaRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredKeyedService<IReceiptRecognitionService>(OllamaDIExtension.ImageRecognitionServiceKey);
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(receipt);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", receiptCulture: new("fr-FR"), SupportedReceiptType.Gas, CancellationToken.None);
        Assert.NotNull(dto);
        Assert.StartsWith(merchantName, dto.MerchantName, StringComparison.InvariantCultureIgnoreCase);
        Assert.Equal(total, dto.Total?.Amount);
    }

    [Theory]
    [InlineData(ReceiptsProvider.Restaurant.France.McDonalds, "McDonald's", 9.40)]
    [InlineData(ReceiptsProvider.Restaurant.France.NoNameMarchant, null, 56.30)]
    [InlineData(ReceiptsProvider.Restaurant.France.SetteHolding, "Sette Holding", 9.00)]
    public async Task RestaurantReceiptFrench(string receipt, string? merchantName, decimal total)
    {
        IServiceProvider serviceContainer = ServiceContainerProvider.GetOllamaRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredKeyedService<IReceiptRecognitionService>(OllamaDIExtension.ImageRecognitionServiceKey);
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(receipt);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", receiptCulture: new("fr-FR"), SupportedReceiptType.Restaurant, CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Contains(merchantName, dto.MerchantName, StringComparison.InvariantCultureIgnoreCase);
        Assert.Equal(total, dto.Total?.Amount);
    }

    /*[Theory]
    [InlineData(ReceiptsProvider.Shops.Kiabi, "kiabi", "m pantalon", 30, 1, 30)]
    [InlineData(ReceiptsProvider.Shops.AlpesBureau, "Alpes bureau Ville la Grand", "surligneur boss vert", 1.25, 1, 15.39)]
    [InlineData(ReceiptsProvider.Shops.Aldi, "Aldi", "pain complet", 1.35, 1, 19.07)]
    public async Task CombinedRecognition(string receipt, string merchantName, string articleName, decimal expectedPrice, decimal expectedQuantity, decimal total)
    {
        IServiceProvider serviceContainer = ServiceContainerProvider.GetOllamaRecognitionServices();
        IReceiptRecognitionService recognitionService = serviceContainer.GetRequiredKeyedService<IReceiptRecognitionService>(OllamaDIExtension.CombinedRecognitionServiceKey);
        ReceiptsProvider receiptsProvider = serviceContainer.GetRequiredService<ReceiptsProvider>();
        using Stream stream = receiptsProvider.GetReceiptByName(receipt);
        var dto = await recognitionService.GetReceiptData(stream, "image/jpeg", receiptCulture: new("fr-FR"), SupportedReceiptType.Retail, CancellationToken.None);
        Assert.NotNull(dto);
        Assert.Equal(merchantName, dto.MerchantName, ignoreCase: true, ignoreWhiteSpaceDifferences : true);
        Assert.Equal(total, dto.Total?.Amount);
        Assert.Contains(dto.Items,
            x =>
                x.Description?.Equals(articleName, StringComparison.InvariantCultureIgnoreCase) ?? false
                && x.Quantity == expectedQuantity
                && x.UnitPrice?.Amount == expectedPrice
                );

    }*/

    //[Fact]
    public void TestRetailReceiptDto()
    {
        ReceiptItemDto[] receiptItemDto = [
            new("PAIN COMPLET", 1, new(1.35M, ""), new(1.35M, "EUR")),
            new("PUR JUS DE POIRE 1L", 6, new(1.79M, "EUR"), new(10.74M, "EUR")),
            new("GYOZAS POULET LEGUMES x2", 1, new(3.49M, "EUR"), new(3.49M, "EUR"))
        ];

        ReceiptTaxDetailDto[] taxDetails = [
            new("Tva", 200, new(0.99M, "EUR"))
        ];

        ReceiptPaymentDto[] paymentDetails = [
            new("CB", new(19.07M, "EUR"))
            ];
        ReceiptDto receiptDto = new (
            MerchantName: "Aldi",
            MerchantAddress : "4 rue des Buchillons, 74100 Annemasse",
            PurchaseDate: "26/05/2025 14:44:23",
            Total: new(19.07M, "EUR"),
            Taxes: new(0.99M, "EUR"),
            Tip: Currency.Empty,
            Items: [.. receiptItemDto],
            TaxDetails: [.. taxDetails],
            Payments: [..paymentDetails]);

        string json = JsonSerializer.Serialize(receiptDto);
        Assert.NotEmpty(json);
    }

    //[Fact]
    public void TestGasStationTicket()
    {
        ReceiptRecognition.Ollama.ReceiptSchemaProviders.FuelReceiptDto gasStation = new(
            "Intermarche",
            "Intermarche Vetraz Monthoux 74100 Vetraz-Monthoux",
            "26-08-2025",
            new(33.71M, "EUR"),
            new(5.62M, "EUR"),
            new(0M, "EUR"),
            "SP98",
            19.54M,
            "L",
            new(1.725M, "EUR"),
            [new ReceiptTaxDetailDto("TVA", 20.00M, new Currency(5.62M, "EUR"))],
            [new ReceiptPaymentDto("CARTE BANCAIRE", new(33.71M, "EUR"))]
        );

        string json = JsonSerializer.Serialize(gasStation);
        Assert.NotEmpty(json);
    }



}

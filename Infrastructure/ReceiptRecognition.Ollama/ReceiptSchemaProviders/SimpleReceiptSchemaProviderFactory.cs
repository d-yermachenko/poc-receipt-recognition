using ReceiptRecognition.Core;

namespace ReceiptRecognition.Ollama.ReceiptSchemaProviders;

internal class SimpleReceiptSchemaProviderFactory : IReceiptSchemaProviderFactory
{
    public IReceiptSchemaProvider GetReceiptSchemaProvider(SupportedReceiptType receiptType)
    {
        return receiptType switch
        {
            SupportedReceiptType.Gas => new GasStationReceiptSchemaProvider(),
            SupportedReceiptType.Restaurant => new RestaurantReceiptSchemaProvider(),
            SupportedReceiptType.RetailMeal => new RetailReceiptSchemaProvider(),
            _ => new RetailReceiptSchemaProvider()
        };
    }
}

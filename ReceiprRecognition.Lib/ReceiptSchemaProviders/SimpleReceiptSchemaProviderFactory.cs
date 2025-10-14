using ReceiptRecognition.Core;

namespace ReceiptRecognition.Ollama.ReceiptSchemaProviders;

internal class SimpleReceiptSchemaProviderFactory : IReceiptSchemaProviderFactory
{
    public IReceiptSchemaProvider GetReceiptSchemaProvider(SupportedReceiptType receiptType)
    {
        return receiptType switch
        {
            _ => new RetailReceiptSchemaProvider()
        };
    }
}

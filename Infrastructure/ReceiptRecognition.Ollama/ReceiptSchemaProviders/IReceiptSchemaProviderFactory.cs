using ReceiptRecognition.Core;

namespace ReceiptRecognition.Ollama.ReceiptSchemaProviders;

public interface IReceiptSchemaProviderFactory
{
    IReceiptSchemaProvider GetReceiptSchemaProvider(SupportedReceiptType receiptType);
}

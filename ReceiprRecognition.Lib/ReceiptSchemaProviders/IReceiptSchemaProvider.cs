
using System.Globalization;

namespace ReceiptRecognition.Ollama.ReceiptSchemaProviders;

public interface IReceiptSchemaProvider
{
    dynamic ReceiptFormat { get; }

    string GetDefaultPrompt(CultureInfo receiptCulture);
}

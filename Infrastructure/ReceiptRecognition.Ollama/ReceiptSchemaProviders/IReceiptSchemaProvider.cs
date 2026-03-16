
using System.Globalization;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.Ollama.ReceiptSchemaProviders;

public interface IReceiptSchemaProvider
{
    dynamic ReceiptFormat { get; }

    string GetDefaultPrompt(CultureInfo receiptCulture);

    ReceiptDto? SerializeToReceipt(string content); 
}


using System.Globalization;

namespace ReceiptRecognition.Core;

public interface IReceiptRecognitionService
{
    Task<ReceiptDto?> GetReceiptData(Stream imageStream, string imageMimeType, CultureInfo receiptCulture, SupportedReceiptType receiptType = SupportedReceiptType.Unknown, CancellationToken cancellationToken= default);
}

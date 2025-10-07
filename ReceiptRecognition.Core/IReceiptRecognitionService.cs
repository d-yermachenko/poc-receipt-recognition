
namespace ReceiptRecognition.Core;

public interface IReceiptRecognitionService
{
    public Task<ReceiptDto?> GetReceiptData(Stream imageStream, string imageMimeType, CancellationToken cancellationToken);
}

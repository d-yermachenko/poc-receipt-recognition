using System.Globalization;
using ReceiptRecognition.API.Application.Abstraction;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.API.Application.Data;

public record RecognitionJob(Guid JobId, ReadOnlyMemory<byte> ImageStream, string ImageMimeType, CultureInfo ReceiptCulture, SupportedReceiptType ReceiptType) : AbstractJob(JobId)
{
    public RecognitionJobStatus CreateStatus()
    {
        return RecognitionJobStatus.CreatePlanned(JobId);
    }

    public static RecognitionJob CreateFromData(Stream imageStream, string imageMimeType, CultureInfo receiptCulture, SupportedReceiptType receiptType)
    {
        using MemoryStream memoryStream = new();
        imageStream.CopyTo(memoryStream);
        return new RecognitionJob(Guid.NewGuid(), memoryStream.ToArray(), imageMimeType, receiptCulture, receiptType);
    }
}

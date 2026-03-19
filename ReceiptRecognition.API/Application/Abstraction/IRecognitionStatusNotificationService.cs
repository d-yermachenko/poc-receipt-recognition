using ReceiptRecognition.API.Application.Data;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.API.Application.Abstraction;

public interface IRecognitionStatusNotificationService
{
    Task NotifyInformationAsync(Guid jobId, string message, CancellationToken cancellationToken = default);

    Task NotifySuccessAsync(Guid jobId, ReceiptDto result, CancellationToken cancellationToken = default);

    Task NotifyFailureAsync(Guid jobId, string errorMessage, CancellationToken cancellationToken = default);

    Task<RecognitionJobStatus> GetStatusAsync(Guid jobId, CancellationToken cancellationToken = default);
}

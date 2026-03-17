using ReceiptRecognition.Core;

namespace ReceiptRecognition.API.Application.Services;

public class RecognitionHandler(IReceiptRecognitionService ReceiptRecognitionService, IRecognitionStatusNotificationService NotificationService, ILogger<RecognitionHandler> logger)
{
    public async Task Handle(RecognitionJob job, CancellationToken cancellationToken = default)
    {
        try
        {
            await NotificationService.NotifyInformationAsync(job.JobId, "Recognition started.", cancellationToken);
            logger.LogInformation("Starting recognition for job {JobId}", job.JobId);
            ReceiptDto? result = await ReceiptRecognitionService.GetReceiptData(new MemoryStream(job.ImageStream.ToArray()), job.ImageMimeType, job.ReceiptCulture, job.ReceiptType, cancellationToken);
            if (result != null)
            {
                await NotificationService.NotifySuccessAsync(job.JobId, result, cancellationToken);
                logger.LogInformation("Recognition completed for job {JobId}", job.JobId);
            }
            else
            {
                await NotificationService.NotifyFailureAsync(job.JobId, "Recognition returned null result", cancellationToken);
                logger.LogWarning("Recognition returned null result for job {JobId}", job.JobId);
            }
        }
        catch (Exception ex)
        {
            await NotificationService.NotifyFailureAsync(job.JobId, ex.Message, cancellationToken);
            logger.LogError(ex, "Recognition failed for job {JobId} with error: {ErrorMessage}", job.JobId, ex.Message);
        }
    }
}

using ReceiptRecognition.API.Application.Abstraction;
using ReceiptRecognition.API.Application.Data;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.API.Application.Services;

/// <summary>
/// Handler for processing receipt recognition jobs. It uses the IReceiptRecognitionService to perform the recognition and the IRecognitionStatusNotificationService to notify about the status of the job. It also logs the process using ILogger.
/// Its a anticorruption layer between the API and the core recognition logic, allowing for separation of concerns and easier maintenance. 
/// The handler is designed to be used in a background processing context, such as a worker service or a message queue consumer, to handle recognition jobs asynchronously without blocking the API's responsiveness.
/// </summary>
/// <param name="ReceiptRecognitionService">The service responsible for performing receipt recognition.</param>
/// <param name="NotificationService">The service responsible for notifying about the status of the recognition job.</param>
/// <param name="logger">The logger used for logging information and errors.</param>
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

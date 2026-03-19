using ReceiptRecognition.API.Application.Abstraction;
using ReceiptRecognition.API.Application.Data;

namespace ReceiptRecognition.API.Application.Services;

/// <summary>
/// Puts the recognition job into a queue for processing. 
/// This allows the API to return immediately and handle the recognition asynchronously, which is important for performance and scalability, especially when dealing with potentially long-running recognition tasks.
/// </summary>
public class RecognitionDispatcher : IRecognitionDispatcher
{
    private readonly IRecognitionStatusNotificationService _recognitionNotifier;
    private readonly ILogger<RecognitionDispatcher> _logger;
    private readonly IJobPublisher<RecognitionJob> _jobPublisher;

    public RecognitionDispatcher(IRecognitionStatusNotificationService recognitionNotifier, ILogger<RecognitionDispatcher> logger, IJobQueue<RecognitionJob> jobPublisher)
    {
        _recognitionNotifier= recognitionNotifier;
        _jobPublisher = jobPublisher;
        this._logger = logger;
    }
    public async Task DispatchRecognitionJobAsync(RecognitionJob job, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching recognition job {JobId}", job.JobId);
        await _recognitionNotifier.NotifyInformationAsync(job.JobId, "Recognition job dispatched and waiting for processing.", cancellationToken);
        await _jobPublisher.DispatchJobAsync(job, cancellationToken);
        _logger.LogInformation("Recognition job {JobId} dispatched successfully", job.JobId);
        // Here you would typically enqueue the job to a message queue or task scheduler for processing by a background worker.
        // For example, you could use Azure Service Bus, RabbitMQ, or a simple in-memory queue depending on your requirements and infrastructure.
    }
}

namespace ReceiptRecognition.API.Application.Services;

public interface IRecognitionDispatcher
{
    Task DispatchRecognitionJobAsync(RecognitionJob job, CancellationToken cancellationToken = default);
}


/// <summary>
/// Puts the recognition job into a queue for processing. This allows the API to return immediately and handle the recognition asynchronously, which is important for performance and scalability, especially when dealing with potentially long-running recognition tasks.
/// </summary>
public class RecognitionDispatcher : IRecognitionDispatcher
{
    private readonly IRecognitionStatusNotificationService _recognitionNotifier;
    private readonly ILogger<RecognitionDispatcher> _logger;

    public RecognitionDispatcher(IRecognitionStatusNotificationService recognitionNotifier, ILogger<RecognitionDispatcher> logger)
    {
        _recognitionNotifier= recognitionNotifier;
        this._logger = logger;
    }
    public async Task DispatchRecognitionJobAsync(RecognitionJob job, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching recognition job {JobId}", job.JobId);
        await _recognitionNotifier.NotifyInformationAsync(job.JobId, "Recognition job dispatched and waiting for processing.", cancellationToken);
        // Here you would typically enqueue the job to a message queue or task scheduler for processing by a background worker.
        // For example, you could use Azure Service Bus, RabbitMQ, or a simple in-memory queue depending on your requirements and infrastructure.
    }
}

using ReceiptRecognition.API.Application.Abstraction;
using ReceiptRecognition.API.Application.Data;

namespace ReceiptRecognition.API.Application.Services;

public class RecognitionJobProcessor : BackgroundService
{
    private readonly ILogger<RecognitionJobProcessor> _recognitionServiceLog;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public RecognitionJobProcessor(ILogger<RecognitionJobProcessor> recognitionService, IServiceScopeFactory recognitionHandler)
    {
        _recognitionServiceLog = recognitionService;
        _serviceScopeFactory = recognitionHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _recognitionServiceLog.LogInformation("Recognition background service is starting.");
        using var scope = _serviceScopeFactory.CreateScope();
            var _jobConsumer = scope.ServiceProvider.GetRequiredService<IJobConsumer<RecognitionJob>>();
        await _jobConsumer.AddConsumer(async (job, cancellationToken) =>
        {
            try
            {
                var recognitionHandler = scope.ServiceProvider.GetRequiredService<RecognitionHandler>();
                recognitionHandler.Handle(job, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _recognitionServiceLog.LogWarning("Processing of job with ID: {JobId} was cancelled.", job.JobId);
            }
            catch (Exception ex)
            {
                _recognitionServiceLog.LogError(ex, "An error occurred while processing job with ID: {JobId}", job.JobId);
            }
        }, stoppingToken);

    }
}

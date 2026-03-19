using System.Text.Json;
using System.Text.Json.Serialization;
using ReceiptRecognition.API.Application.Abstraction;
using ReceiptRecognition.API.Application.Data;
using ReceiptRecognition.Core;
using StackExchange.Redis;

namespace ReceiptRecognition.API.Application.Services;

/// <summary>
/// In this implementation I use redis directly, but for this goals notification service can be used, which can handle concurrent updates and retrieval of recognition status and results by id.
/// </summary>
public class RecognitionStatusNotificationService : IRecognitionStatusNotificationService
{
    private readonly IDatabase _redisDb;
    private readonly ILogger<RecognitionStatusNotificationService> _logger;
    private readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerOptions.Default)
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
    };

    public RecognitionStatusNotificationService(IConnectionMultiplexer redis, ILogger<RecognitionStatusNotificationService> logger)
    {
        _redisDb = redis.GetDatabase();
        _logger = logger;
    }

    private async Task<RecognitionJobStatus> GetOrCreateJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        string? jobStatusJson = await _redisDb.StringGetAsync(jobId.ToString());
        if (jobStatusJson is null)
        {
            _logger.LogWarning("Recognition job {JobId} not found in Redis.", jobId);
            return RecognitionJobStatus.CreatePlanned(jobId);
        }
        return System.Text.Json.JsonSerializer.Deserialize<RecognitionJobStatus>(jobStatusJson, jsonSerializerOptions)!;
    }

    private async Task SaveJobStatusAsync(RecognitionJobStatus jobStatus, CancellationToken cancellationToken = default)
    {
        string jobStatusJson = System.Text.Json.JsonSerializer.Serialize(jobStatus, jsonSerializerOptions);
        await _redisDb.StringSetAsync(jobStatus.JobId.ToString(), jobStatusJson, expiry: TimeSpan.FromHours(24));
    }

    public async Task NotifyFailureAsync(Guid jobId, string errorMessage, CancellationToken cancellationToken = default)
    {
        _logger.LogError("Recognition job {JobId} failed with error: {ErrorMessage}", jobId, errorMessage);
        var jobStatus = await GetOrCreateJobStatusAsync(jobId, cancellationToken);
        jobStatus.Fail(errorMessage);
        await SaveJobStatusAsync(jobStatus, cancellationToken);
    }

    public async Task NotifyInformationAsync(Guid jobId, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Recognition job {JobId} update: {Message}", jobId, message);
        var jobStatus = await GetOrCreateJobStatusAsync(jobId, cancellationToken);
        jobStatus.UpdateStatus(message);
        await SaveJobStatusAsync(jobStatus, cancellationToken);
    }

    public async Task NotifySuccessAsync(Guid jobId, ReceiptDto result, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Recognition job {JobId} succeeded.", jobId);
        var jobStatus = await GetOrCreateJobStatusAsync(jobId, cancellationToken);
        jobStatus.Succeed(result);
        await SaveJobStatusAsync(jobStatus, cancellationToken);
    }

    public async Task<RecognitionJobStatus> GetStatusAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving status for recognition job {JobId}.", jobId);
        string? jobStatusJson = await _redisDb.StringGetAsync(jobId.ToString());
        if (jobStatusJson is null)
        {
            _logger.LogWarning("Recognition job {JobId} not found in Redis.", jobId);
            return RecognitionJobStatus.CreateNotFound(jobId);
        }
        RecognitionJobStatus recognitionJobStatus = JsonSerializer.Deserialize<RecognitionJobStatus>(jobStatusJson, jsonSerializerOptions)!;
        return recognitionJobStatus!;

    }
}
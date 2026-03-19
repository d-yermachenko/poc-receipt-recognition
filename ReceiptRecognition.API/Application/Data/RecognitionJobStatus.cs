using System.Text.Json.Serialization;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.API.Application.Data;

public record RecognitionJobStatusUpdate(string Status, DateTime Update);
public class RecognitionJobStatus
{
    [JsonConstructor]
    public RecognitionJobStatus(
    Guid jobId,
    List<RecognitionJobStatusUpdate> statusHistory,
    DateTime createdAt,
    ReceiptDto? result,
    string errorMessage)
    {
        JobId = jobId;
        StatusHistory = statusHistory;
        CreatedAt = createdAt;
        Result = result;
        ErrorMessage = errorMessage;
    }

    public List<RecognitionJobStatusUpdate> StatusHistory { get; internal set; } = new();

    public Guid JobId { get; internal set; }

    public DateTime CreatedAt { get; internal set; }

    public ReceiptDto? Result { get; internal set; }

    public string ErrorMessage { get; internal set; } = string.Empty;

    public static RecognitionJobStatus CreatePlanned(Guid jobId)
    {
        RecognitionJobStatus status = new RecognitionJobStatus(jobId);
        status.UpdateStatus("Planned. Execution can take several minutes.");
        return status;
    }

    public static RecognitionJobStatus CreateNotFound(Guid jobId)
    {
        RecognitionJobStatus status = new RecognitionJobStatus(jobId);
        status.UpdateStatus("Not Found");
        return status;
    }

    private RecognitionJobStatus(Guid jobId)
    {
        JobId = jobId;
        CreatedAt = DateTime.UtcNow;
        Result = null;
    }

    public void UpdateStatus(string status)
    {
        StatusHistory.Add(new RecognitionJobStatusUpdate(status, DateTime.UtcNow));
    }

    public void Succeed(ReceiptDto result)
    {
        Result = result;
        UpdateStatus("Completed");
    }

    public void Fail(string errorMessage)
    {
        ErrorMessage = errorMessage;
        UpdateStatus($"Failed: {errorMessage}");
    }

}

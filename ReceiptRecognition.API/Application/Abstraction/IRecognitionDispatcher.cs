using ReceiptRecognition.API.Application.Data;

namespace ReceiptRecognition.API.Application.Abstraction;

/// <summary>
/// Common dispatcher interface for handling recognition jobs. 
/// The dispatcher is responsible for receiving recognition jobs and dispatching them to the appropriate processing mechanism, such as a background worker or a message queue. 
/// This abstraction allows for flexibility in how recognition jobs are processed and can be easily extended or modified without affecting the rest of the application.
/// </summary>
public interface IRecognitionDispatcher
{
    Task DispatchRecognitionJobAsync(RecognitionJob job, CancellationToken cancellationToken = default);
}

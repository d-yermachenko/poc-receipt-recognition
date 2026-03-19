namespace ReceiptRecognition.API.Application.Abstraction;

public interface IJobPublisher<in TJob>
    where TJob :  AbstractJob
{
    ValueTask<bool> DispatchJobAsync(TJob job, CancellationToken cancellationToken = default);
}

public interface IJobConsumer<out TJob>
    where TJob : AbstractJob
{
    public Task AddConsumer(Func<TJob, CancellationToken, Task> onJobReceived, CancellationToken cancellationToken = default);
}

public interface IJobQueue<TJob> : IJobPublisher<TJob>, IJobConsumer<TJob>
    where TJob : AbstractJob
{
}

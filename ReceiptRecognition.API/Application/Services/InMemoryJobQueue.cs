using ReceiptRecognition.API.Application.Abstraction;
using System.Threading.Channels;

namespace ReceiptRecognition.API.Application.Services;

public class InMemoryJobQueue<TJob> : IJobQueue<TJob>
    where TJob : AbstractJob
{

    private readonly ILogger _logger;
    private readonly Channel<TJob> _channel;

    private ChannelWriter<TJob> _writer;
    private ChannelReader<TJob> _reader;
    public InMemoryJobQueue(ILogger<InMemoryJobQueue<TJob>> logger)
    {
        BoundedChannelOptions options = new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false,
            Capacity = 100
        };

        _channel = Channel.CreateBounded<TJob>(options);
        _writer = _channel.Writer;
        _reader = _channel.Reader;
        _logger = logger;
    }


    public async Task AddConsumer(Func<TJob, CancellationToken, Task> onJobReceived, CancellationToken cancellationToken = default)
    {
        await foreach (var job in _reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Received job {JobId} for processing", job.JobId);
                if (onJobReceived is not null)
                {
                    await onJobReceived.Invoke(job, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing job {JobId}: {ErrorMessage}", job.JobId, ex.Message);
            }
        }
    }

    public async ValueTask<bool> DispatchJobAsync(TJob job, CancellationToken cancellationToken = default)
    {
       return await _writer.WaitToWriteAsync(cancellationToken) && _writer.TryWrite(job);
    }
}

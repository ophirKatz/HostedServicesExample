using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace Api;

public interface ITestRunEnqueue
{
    ValueTask RunTestAsync(Guid testRunId);
}

public interface ITestRunDequeue
{
    bool TryGetNextTest(out Guid testRunId);
}

public interface ITestRunQueue : ITestRunEnqueue, ITestRunDequeue
{

}

public class TestRunQueue : ITestRunQueue
{
    private readonly Channel<Guid> _queue;

    public TestRunQueue(IOptions<TestRunWorkersOptions> workerOptions)
    {
        var options = new BoundedChannelOptions(workerOptions.Value.TestCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        _queue = Channel.CreateBounded<Guid>(options);
    }

    public async ValueTask RunTestAsync(Guid testRunId)
    {
        await _queue.Writer.WriteAsync(testRunId);
    }

    public async ValueTask<Guid> GetTestToRunAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }

    public bool TryGetNextTest(out Guid testRunId)
    {
        return _queue.Reader.TryRead(out testRunId);
    }
}
namespace Api;

public class TestRunner
{
    private readonly ILogger<TestRunner> _logger;

    public TestRunner(ILogger<TestRunner> logger)
    {
        _logger = logger;
    }

    public async Task RunTestAsync(Guid testRunId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Running test {testRunId}...");
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        _logger.LogInformation($"Finished running test {testRunId}.");
    }
}
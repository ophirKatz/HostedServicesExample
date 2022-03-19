using Microsoft.Extensions.Options;

namespace Api;

public class TestRunningBackgroundService : BackgroundService
{
    private readonly ITestRunDequeue _testRunQueue;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IOptions<TestRunWorkersOptions> _workerOptions;
    private readonly ILogger<TestRunningBackgroundService> _logger;

    public TestRunningBackgroundService(ITestRunDequeue testRunQueue,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<TestRunWorkersOptions> workerOptions,
        ILogger<TestRunningBackgroundService> logger)
    {
        _testRunQueue = testRunQueue;
        _serviceScopeFactory = serviceScopeFactory;
        _workerOptions = workerOptions;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service running.");

        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service is working.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunTestAsync(stoppingToken);

            await Task.Delay(_workerOptions.Value.Delay, stoppingToken);
        }
    }

    private async Task RunTestAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Trying to run a test...");
        if (!_testRunQueue.TryGetNextTest(out var testRunId))
        {
            return;
        }

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var testRunner = scope.ServiceProvider.GetRequiredService<TestRunner>();
            await testRunner.RunTestAsync(testRunId, cancellationToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Consume Scoped Service Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}
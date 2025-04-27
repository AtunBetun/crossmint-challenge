using CrossmintChallenge.Host.Services;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CrossmintChallenge.Host;

public class HostedService : IHostedService
{
    public Processor Processor { get; init; }

    public HostedService(Processor processor)
    {
        Processor = processor.NotNull();
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        // Needed for Ctrl-C to work properly
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var backgroundTask = Task.Run(() => Processor.Execute(cts.Token), cts.Token);
        return Task.CompletedTask;
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("stopping hosted service");
        return Task.CompletedTask;
    }
}

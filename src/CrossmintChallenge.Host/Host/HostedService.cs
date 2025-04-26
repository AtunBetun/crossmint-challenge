using Microsoft.Extensions.Hosting;
using CrossmintChallenge.Host.Services;

namespace CrossmintChallenge.Host;

public class HostedService : IHostedService
{
    public Processor Processor { get; init; }

    public HostedService(Processor processor)
    {
        Processor = processor.NotNull();
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        await Processor.Execute();
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

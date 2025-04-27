using CrossmintChallenge.Host.Services;
using Microsoft.Extensions.Hosting;

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
        await Processor.Execute2(cancellationToken);
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

using Microsoft.Extensions.Hosting;
using Serilog;

namespace CrossmintChallenge.Host;

public class Program
{
    static async Task Main(string[] args)
    {
        LogFactory.Create();
        Log.Information("starting host");
        IHost host = HostBuilder.CreateHost().Build();
        await host.StartAsync();
    }
}

using Microsoft.Extensions.Hosting;
using Serilog;

namespace CrossmintChallenge.Host;

public class Program
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        Log.Information("starting host");
        IHost host = HostBuilder.CreateHost().Build();
        await host.StartAsync();
    }
}

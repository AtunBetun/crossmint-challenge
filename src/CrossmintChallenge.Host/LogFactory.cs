using Serilog;

namespace CrossmintChallenge.Host;

public static class LogFactory
{
    public static void Create()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }
}

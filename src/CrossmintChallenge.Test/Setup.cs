using CrossmintChallenge.Clients;
using CrossmintChallenge.Host;
using CrossmintChallenge.Host.Services;
using Flurl.Http.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CrossmintChallenge.Test;

public static class Setup
{
    public static IHostBuilder Builder()
    {
        IHostBuilder builder = CrossmintChallenge.Host.HostBuilder.CreateHost();
        return builder;
    }
}

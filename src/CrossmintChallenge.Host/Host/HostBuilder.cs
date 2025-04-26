using CrossmintChallenge.Clients;
using CrossmintChallenge.Host.Services;
using Flurl.Http.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CrossmintChallenge.Host;

public static class HostBuilder
{
    public static IHostBuilder CreateHost() =>
        Microsoft
            .Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddServices().AddHttpClients();
            });

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<Processor>();
        services.AddTransient<MegaverseMapClient>();
        services.AddTransient<PollyHandler>();
        services.AddHostedService<HostedService>();
        return services;

    }

    public static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        services.AddSingleton<IFlurlClientCache>(sp =>
            new FlurlClientCache().Add(
                nameof(MegaverseMapClient),
                null,
                builder =>
                {
                    builder.AddMiddleware(
                        () => sp.GetRequiredService<PollyHandler>()
                    );
                }
            )
        );
        return services;

    }
}

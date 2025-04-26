using CrossmintChallenge.Clients;
using CrossmintChallenge.Host;
using CrossmintChallenge.Host.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CrossmintChallenge.Test;

public class IntegrationTests
{
    public IntegrationTests()
    {
        LogFactory.Create();
    }

    [Fact]
    public async Task GetMapGoal()
    {
        var builder = Setup.Builder().Build();
        Processor processor = builder.Services.GetRequiredService<Processor>();
        MegaverseClient client = builder.Services.GetRequiredService<MegaverseClient>();
        var result = await client.GetMapGoalAsync(Processor.ChallengeUrl(), Processor.CandidateId);
    }

    [Fact]
    public async Task GetMap()
    {
        var builder = Setup.Builder().Build();
        Processor processor = builder.Services.GetRequiredService<Processor>();
        MegaverseClient client = builder.Services.GetRequiredService<MegaverseClient>();
        var result = await client.GetMapAsync(Processor.ChallengeUrl(), Processor.CandidateId);
    }
}

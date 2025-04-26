using CrossmintChallenge.Clients;
using Flurl;
using Flurl.Http;
using Serilog;

namespace CrossmintChallenge.Host.Services;

public class Processor
{
    static Url challengeUrl() => new Url("https://challenge.crossmint.io/api");

    const string CandidateId = "9b5772ff-aa4e-4870-956c-9e0332789869";

    public MegaverseClient MegaverseMapClient { get; init; }

    public Processor(MegaverseClient megaverseMapClient)
    {
        MegaverseMapClient = megaverseMapClient.NotNull();
    }


    // TODO:
    // 1. Get the map goal
    // While (Map != Goal)
    //      2. get current map
    //      3. Mapper => get Deletes and Posts
    //      4. Loop doing Deletes and Posts
    //

    public async Task Execute(bool createTestScenario = true)
    {
        Log.Information("starting processor");

        Log.Information("getting goal for polyanets");
        var mapGoal = await MegaverseMapClient.GetMapGoalAsync(challengeUrl(), CandidateId);

        if (createTestScenario)
        {
            Log.Information("seeding random polyanets for test scenario");
            List<(int Row, int Col)> randomPolyanets = Mapper.GetRandomPositions();
            await CallPolyanetsAsync(randomPolyanets, MegaverseMapClient.PostPolyanetAsync);
        }

        Log.Information("deleting extra polyanets");
        MapResponse mapResponse = await MegaverseMapClient
            .GetMapAsync(challengeUrl(), CandidateId)
            .NotNull();

        int polyanetsForDeletion = mapResponse.Map.Content
            .SelectMany(row => row)
            .Count(cell => cell != null);
        while (polyanetsForDeletion > 0)
        {
            Log.Information("{@polyanetsForDeletion} to delete", polyanetsForDeletion);

            await CallPolyanetsAsync(
                Mapper.GetDeletions(mapResponse),
                MegaverseMapClient.DeletePolyanetAsync
            );
            mapResponse = await MegaverseMapClient
                .GetMapAsync(challengeUrl(), CandidateId)
                .NotNull();
            Log.Debug("{@mapResponse}", mapResponse);
            polyanetsForDeletion = mapResponse.Map.Content
                .SelectMany(row => row)
                .Count(cell => cell != null);

        }

        Log.Information("{@mapGoal}", mapGoal);
        var polyanets = Mapper.GetPolyanetPositions(mapGoal);
        Log.Information("creating polyanets for goal {@polyanets}", mapGoal);
        await CallPolyanetsAsync(polyanets, MegaverseMapClient.PostPolyanetAsync);

        Log.Information("end");
    }

    public async Task CallPolyanetsAsync(
        List<(int Row, int Col)> polyanets,
        Func<Url, int, int, string, Task<IFlurlResponse>> task
    )
    {
        Log.Information("mutating polyanets {@polyanets} {@task}", polyanets, task.Method.Name);

        const int batchSize = 10;
        for (int i = 0; i < polyanets.Count; i += batchSize)
        {
            var batch = polyanets.Skip(i).Take(batchSize).ToList();
            var tasks = batch
                .Select(poly =>
                    task(challengeUrl(), poly.Row, poly.Col, CandidateId)
                )
                .ToList();

            await Task.WhenAll(tasks);

            if (i + batchSize < polyanets.Count)
            {
                Log.Information("Batch complete, waiting 1 seconds before next batch...");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }

}

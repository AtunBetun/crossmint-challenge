using CrossmintChallenge.Clients;
using Flurl;
using Flurl.Http;
using Serilog;

namespace CrossmintChallenge.Host.Services;

public enum StarActionEnum
{
    CREATE,
    DELETE,
}

public class Processor
{
    public static Url ChallengeUrl() => new Url("https://challenge.crossmint.io/api");

    public const string CandidateId = "9b5772ff-aa4e-4870-956c-9e0332789869";

    public MegaverseClient MegaverseMapClient { get; init; }
    public MegaverseService MegaverseService { get; init; }

    public Processor(MegaverseClient megaverseMapClient, MegaverseService megaverseService)
    {
        MegaverseMapClient = megaverseMapClient.NotNull();
        MegaverseService = megaverseService.NotNull();
    }

    // # Algorithm:
    // 1. Get the map goal
    // 2. Get the map
    // While (Map != Goal)
    //      3. get current map
    //      4. Mapper => get Deletes and Posts
    //      5. Batch doing Deletes and Posts

    public async Task Execute(bool createTestScenario = true)
    {
        Log.Information("starting processor");

        Log.Information("getting goal for polyanets");
        var mapGoal = await MegaverseMapClient.GetMapGoalAsync(ChallengeUrl(), CandidateId);

        if (createTestScenario)
        {
            Log.Information("seeding random polyanets for test scenario");
            List<(int Row, int Col)> randomPolyanets = Mapper.GetRandomPositions();
            await CallPolyanetsAsync(randomPolyanets, MegaverseMapClient.PostPolyanetAsync);
        }

        Log.Information("deleting extra polyanets");
        MapResponse mapResponse = await MegaverseMapClient
            .GetMapAsync(ChallengeUrl(), CandidateId)
            .NotNull();

        int polyanetsForDeletion = mapResponse
            .Map.Content.SelectMany(row => row)
            .Count(cell => cell != null);
        while (polyanetsForDeletion > 0)
        {
            Log.Information("{@polyanetsForDeletion} to delete", polyanetsForDeletion);

            await CallPolyanetsAsync(
                Mapper.GetDeletions(mapResponse),
                MegaverseMapClient.DeletePolyanetAsync
            );
            mapResponse = await MegaverseMapClient
                .GetMapAsync(ChallengeUrl(), CandidateId)
                .NotNull();
            Log.Debug("{@mapResponse}", mapResponse);
            polyanetsForDeletion = mapResponse
                .Map.Content.SelectMany(row => row)
                .Count(cell => cell != null);
        }

        Log.Information("{@mapGoal}", mapGoal);
        var polyanets = Mapper.GetPolyanetPositions(mapGoal);
        Log.Information("creating polyanets for goal {@polyanets}", mapGoal);
        await CallPolyanetsAsync(polyanets, MegaverseMapClient.PostPolyanetAsync);

        Log.Information("end");
    }

    public async Task CallStarsAsync(
        List<(int Row, int Col, GoalItem starGoal, StarActionEnum action)> stars,
        int batchSize = 10
    )
    {
        Log.Information("calling star {@stars}", stars);

        for (int i = 0; i < stars.Count; i += batchSize)
        {
            var batch = stars.Skip(i).Take(batchSize).ToList();
            var tasks = batch
                .Select(starTask => starTask(ChallengeUrl(), starTask.Row, starTask.Col, CandidateId))
                .ToList();

            await Task.WhenAll(tasks);

            if (i + batchSize < stars.Count)
            {
                Log.Information("Batch complete, waiting 1 seconds before next batch...");
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
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
                .Select(poly => task(ChallengeUrl(), poly.Row, poly.Col, CandidateId))
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

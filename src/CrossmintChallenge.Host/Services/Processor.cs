using CrossmintChallenge.Clients;
using Flurl;
using Serilog;

namespace CrossmintChallenge.Host.Services;

public class Processor
{
    public static Url ChallengeUrl() => new Url("https://challenge.crossmint.io/api");

    public const string CandidateId = "9b5772ff-aa4e-4870-956c-9e0332789869";

    public MegaverseClient MegaverseClient { get; init; }
    public MegaverseService MegaverseService { get; init; }

    public Processor(MegaverseClient megaverseMapClient, MegaverseService megaverseService)
    {
        MegaverseClient = megaverseMapClient.NotNull();
        MegaverseService = megaverseService.NotNull();
    }

    public async Task<List<(int Row, int Col, GoalItem Item)>> StarsForUpdate()
    {
        var goalMap = await MegaverseClient.GetMapGoalAsync(ChallengeUrl(), CandidateId);
        var mapResponse = await MegaverseClient.GetMapAsync(ChallengeUrl(), CandidateId);
        var currentMap = mapResponse.ToGoalResponse();

        var updates = Mapper.StarsForUpdate(goalMap, currentMap);
        return updates;
    }

    // # Algorithm:
    // 1. Get the map goal
    // 2. Get the map
    // While (Map != Goal)
    //      3. get current map
    //      4. Mapper => get Deletes and Posts
    //      5. Batch doing Deletes and Posts

    public async Task Execute(CancellationToken cancellationToken, bool createTestScenario = false)
    {
        Log.Information("starting processor");

        if (createTestScenario)
        {
            Log.Information("seeding random stars for test scenario");
            List<(int Row, int Col, GoalItem starGoal)> randomStars = Mapper.GetRandomStars(700);
            List<Func<Task>> taskDelegates = randomStars
                .Select(x =>
                    (Func<Task>)(
                        () =>
                            MegaverseService.CreateStar(
                                x.starGoal,
                                (x.Row, x.Col),
                                ChallengeUrl(),
                                CandidateId
                            )
                    )
                )
                .ToList()
                .NotNull();

            Log.Information("running {@taskCount}", taskDelegates.Count);
            await Batch.RunBatchTasksAsync(taskDelegates);
        }

        Log.Information("getting current map and goal");
        var updates = await StarsForUpdate();
        Log.Information("{@updates}", updates);

        while (updates.Count > 0)
        {
            List<Func<Task>> taskDelegates = new List<Func<Task>>();

            var createStars = updates.Where(x => x.Item != GoalItem.SPACE).ToList();
            Log.Debug("creating stars {@createStars}", createStars);
            foreach (var x in createStars)
            {
                Func<Task> taskDelegate = () =>
                    MegaverseService.CreateStar(
                        x.Item,
                        (x.Row, x.Col),
                        ChallengeUrl(),
                        CandidateId
                    );
                taskDelegates.Add(taskDelegate);
            }

            var deleteStars = updates.Where(x => x.Item == GoalItem.SPACE).ToList();
            Log.Debug("deleting stars {@deleteStars}", deleteStars);
            foreach (var x in deleteStars)
            {
                Func<Task> taskDelegate = () =>
                    MegaverseService.DeleteStar((x.Row, x.Col), ChallengeUrl(), CandidateId);
                taskDelegates.Add(taskDelegate);
            }
            await Batch.RunBatchTasksAsync(taskDelegates);

            updates = await StarsForUpdate();
        }

        Log.Information("end");
    }

    // public async Task Execute(bool createTestScenario = true)
    // {
    //     Log.Information("starting processor");
    //
    //     if (createTestScenario)
    //     {
    //         Log.Information("seeding random polyanets for test scenario");
    //         List<(int Row, int Col)> randomPolyanets = Mapper.GetRandomPositions();
    //         await CallPolyanetsAsync(randomPolyanets, MegaverseMapClient.PostPolyanetAsync);
    //     }
    //
    //     // Delete
    //     Log.Information("deleting extra polyanets");
    //     Log.Information("getting goal for polyanets");
    //
    //     var mapGoal = await MegaverseMapClient.GetMapGoalAsync(ChallengeUrl(), CandidateId);
    //     MapResponse mapResponse = await MegaverseMapClient
    //         .GetMapAsync(ChallengeUrl(), CandidateId)
    //         .NotNull();
    //
    //     int polyanetsForDeletion = mapResponse
    //         .Map.Content.SelectMany(row => row)
    //         .Count(cell => cell != null);
    //
    //     // while maps not equal
    //     while (polyanetsForDeletion > 0)
    //     {
    //         Log.Information("{@polyanetsForDeletion} to delete", polyanetsForDeletion);
    //
    //         await CallPolyanetsAsync(
    //             Mapper.GetDeletions(mapResponse),
    //             MegaverseMapClient.DeletePolyanetAsync
    //         );
    //         mapResponse = await MegaverseMapClient
    //             .GetMapAsync(ChallengeUrl(), CandidateId)
    //             .NotNull();
    //         Log.Debug("{@mapResponse}", mapResponse);
    //         polyanetsForDeletion = mapResponse
    //             .Map.Content.SelectMany(row => row)
    //             .Count(cell => cell != null);
    //     }
    //
    //     // Creates
    //     Log.Information("{@mapGoal}", mapGoal);
    //     var polyanets = Mapper.GetPolyanetPositions(mapGoal);
    //     Log.Information("creating polyanets for goal {@polyanets}", mapGoal);
    //     await CallPolyanetsAsync(polyanets, MegaverseMapClient.PostPolyanetAsync);
    //
    //     Log.Information("end");
    // }
}

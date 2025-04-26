using System.Linq;
using CrossmintChallenge.Clients;
using Flurl;
using Flurl.Http;
using Serilog;

namespace CrossmintChallenge.Host.Services;

public class Processor
{
    static Url challengeUrl() => new Url("https://challenge.crossmint.io/api");

    const string CandidateId = "9b5772ff-aa4e-4870-956c-9e0332789869";

    public MegaverseMapClient MegaverseMapClient { get; init; }

    public Processor(MegaverseMapClient megaverseMapClient)
    {
        MegaverseMapClient = megaverseMapClient.NotNull();
    }

    public async Task Execute(bool createTestScenario = true)
    {
        Log.Information("starting processor");

        Log.Information("getting goal for polyanets");
        var mapGoal = await MegaverseMapClient.GetMapGoalAsync(challengeUrl(), CandidateId);

        if (createTestScenario)
        {
            Log.Information("seeding random polyanets for test scenario");
            List<(int Row, int Col)> randomPolyanets = GetRandomPositions();
            await CallPolyanetsAsync(randomPolyanets, MegaverseMapClient.PostPolyanetAsync);
            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        Log.Information("deleting extra polyanets");
        MapResponse mapResponse = await MegaverseMapClient
            .GetMapAsync(challengeUrl(), CandidateId)
            .NotNull();

        int polyanetsForDeletion = mapResponse
            .Map.Content.Select(listCellContent =>
                listCellContent.Where(cellContent => cellContent != null)
            )
            .ToList()
            .Count;
        while (polyanetsForDeletion > 0)
        {
            Log.Information("{@polyanetsForDeletion} to delete", polyanetsForDeletion);

            await CallPolyanetsAsync(
                GetDeletions(mapResponse),
                MegaverseMapClient.DeletePolyanetAsync
            );
            mapResponse = await MegaverseMapClient
                .GetMapAsync(challengeUrl(), CandidateId)
                .NotNull();
            Log.Debug("{@mapResponse}", mapResponse);

            polyanetsForDeletion = mapResponse
                .Map.Content.Select(listCellContent =>
                    listCellContent.Where(cellContent => cellContent != null)
                )
                .ToList()
                .Count;
        }

        // await DeleteAllPolyanetsAsync(mapResponse);

        // Log.Information("{@mapGoal}", mapGoal);
        // var polyanets = GetPolyanetPositions(mapGoal);
        // Log.Information("creating polyanets for goal {@polyanets}", mapGoal);
        // foreach (var poly in polyanets)
        // {
        //     await MegaverseMapClient.PostPolyanetAsync(
        //         challengeUrl(),
        //         poly.Row,
        //         poly.Col,
        //         CandidateId
        //     );
        // }

        Log.Information("end");
    }

    public async Task CallPolyanetsAsync(
        List<(int Row, int Col)> polyanets,
        Func<Url, int, int, string, Task<IFlurlResponse>> task
    )
    {
        Log.Information("mutating polyanets {@polyanets} {@task}", polyanets, task.Method.Name);

        const int batchSize = 5;
        for (int i = 0; i < polyanets.Count; i += batchSize)
        {
            var batch = polyanets.Skip(i).Take(batchSize).ToList();
            var tasks = batch
                .Select(poly =>
                    Retry
                        .BuildRetryPolicy()
                        .ExecuteAsync(() => task(challengeUrl(), poly.Row, poly.Col, CandidateId))
                )
                .ToList();

            await Task.WhenAll(tasks);

            if (i + batchSize < polyanets.Count)
            {
                Log.Information("Batch complete, waiting 10 seconds before next batch...");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }
    }

    public static List<(int Row, int Col)> GetRandomPositions(int count = 30, int gridSize = 11)
    {
        var random = new Random();
        var allPositions = new List<(int Row, int Col)>();

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                allPositions.Add((row, col));
            }
        }

        return allPositions.OrderBy(_ => random.Next()).Take(count).ToList();
    }

    public static List<(int Row, int Col)> GetDeletions(MapResponse response)
    {
        var positions = new List<(int, int)>();

        for (int row = 0; row < response.Map.Content.Count; row++)
        {
            for (int col = 0; col < response.Map.Content[row].Count; col++)
            {
                if (response.Map.Content[row][col] != null)
                {
                    positions.Add((row, col));
                }
            }
        }

        return positions;
    }

    public static List<(int Row, int Col)> GetPolyanetPositions(MapGoalResponse response)
    {
        var positions = new List<(int, int)>();

        for (int row = 0; row < response.Goal.Count; row++)
        {
            for (int col = 0; col < response.Goal[row].Count; col++)
            {
                if (response.Goal[row][col] == GoalItem.POLYANET)
                {
                    positions.Add((row, col));
                }
            }
        }

        return positions;
    }
}

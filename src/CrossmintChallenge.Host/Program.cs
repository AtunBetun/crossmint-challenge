using CrossmintChallenge.Clients;
using Flurl;
using Serilog;

class Program
{
    static Url challengeUrl() => new Url("https://challenge.crossmint.io/api");

    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        Log.Information("starting");

        const string candidateId = "9b5772ff-aa4e-4870-956c-9e0332789869";

        var mapResponse = await MegaverseMapClient.GetMapAsync(challengeUrl(), candidateId);
        var deletions = GetDeletions(mapResponse);
        Log.Information("deleting polyanets {@deletions}", deletions);

        foreach (var delete in deletions)
        {
            await MegaverseMapClient.DeletePolyanetAsync(challengeUrl(), delete.Row, delete.Col, candidateId);
        }


        Log.Information("getting goal for polyanets");
        var mapGoal = await MegaverseMapClient.GetMapGoalAsync(challengeUrl(), candidateId);
        var polyanets = GetPolyanetPositions(mapGoal);
        foreach (var poly in polyanets)
        {
            await MegaverseMapClient.PostPolyanetAsync(challengeUrl(), poly.Row, poly.Col, candidateId);
        }

        Log.Information("{@polyanets}", polyanets);

        Log.Information("end");
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

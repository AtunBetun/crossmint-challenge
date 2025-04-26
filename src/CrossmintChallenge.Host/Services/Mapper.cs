using CrossmintChallenge.Clients;
namespace CrossmintChallenge.Host.Services;
public static class Mapper
{

    public static List<(int Row, int Col)> GetRandomPositions(int count = 40, int gridSize = 11)
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

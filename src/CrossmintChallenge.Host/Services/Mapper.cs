using CrossmintChallenge.Clients;

namespace CrossmintChallenge.Host.Services;

public static class Mapper
{
    public static List<(int Row, int Col, GoalItem Item)> GetRandomStars(
        int count = 40,
        int gridSize = 30
    )
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

        var selectedPositions = allPositions.OrderBy(_ => random.Next()).Take(count).ToList();

        var result = selectedPositions
            .Select(pos =>
                (
                    pos.Row,
                    pos.Col,
                    (GoalItem)random.Next(1, Enum.GetValues(typeof(GoalItem)).Length)
                )
            ) // start from 1 to skip SPACE
            .ToList();

        return result;
    }

    public static MapGoalResponse ToGoalResponse(this MapResponse mapResponse)
    {
        var goal = new List<List<GoalItem>>();

        foreach (var row in mapResponse.Map.Content)
        {
            var goalRow = new List<GoalItem>();

            foreach (var cell in row)
            {
                if (cell == null)
                {
                    goalRow.Add(GoalItem.SPACE);
                }
                else
                {
                    goalRow.Add(cell.ToGoalItem());
                }
            }

            goal.Add(goalRow);
        }

        return new MapGoalResponse(goal);
    }

    public static GoalItem ToGoalItem(this CellContent cell)
    {
        return cell.Type switch
        {
            0 => GoalItem.POLYANET,
            1 => cell.Color switch
            {
                ColorEnum.blue => GoalItem.BLUE_SOLOON,
                ColorEnum.red => GoalItem.RED_SOLOON,
                ColorEnum.white => GoalItem.WHITE_SOLOON,
                ColorEnum.purple => GoalItem.PURPLE_SOLOON,
                _ => GoalItem.SPACE,
            },
            2 => cell.Direction switch
            {
                DirectionEnum.up => GoalItem.UP_COMETH,
                DirectionEnum.right => GoalItem.RIGHT_COMETH,
                DirectionEnum.down => GoalItem.DOWN_COMETH,
                DirectionEnum.left => GoalItem.LEFT_COMETH,
                _ => GoalItem.SPACE,
            },
            _ => GoalItem.SPACE,
        };
    }
}

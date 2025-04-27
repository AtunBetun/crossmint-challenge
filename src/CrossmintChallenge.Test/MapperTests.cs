using System.Text.Json;
using CrossmintChallenge.Clients;
using CrossmintChallenge.Host;
using CrossmintChallenge.Host.Services;
using Serilog;

namespace CrossmintChallenge.Test;

public class MapperTests
{
    public MapperTests()
    {
        LogFactory.Create();
    }

    public static string TestProjectPath() =>
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..");

    [Fact]
    public void StarsForUpdate()
    {
        var filledMapContent = File.ReadAllText(
            Path.Combine(TestProjectPath(), "Fixtures", "filled_map.json")
        );
        var goalMapContent = File.ReadAllText(
            Path.Combine(TestProjectPath(), "Fixtures", "goal.json")
        );
        var jsonFilledMap = JsonDocument.Parse(filledMapContent);
        var jsonGoalMap = JsonDocument.Parse(goalMapContent);

        MapGoalResponse filledMap = JsonSerializer
            .Deserialize<MapResponse>(jsonFilledMap, SerializerFactory.CreateOptions())
            .NotNull()
            .ToGoalResponse();

        MapGoalResponse goalMap = JsonSerializer
            .Deserialize<MapGoalResponse>(jsonGoalMap, SerializerFactory.CreateOptions())
            .NotNull();

        Log.Debug("{@filledMap}", filledMap);
        Log.Debug("{@goalMap}", goalMap);

        var starsForUpdate = Mapper.StarsForUpdate(goalMap, filledMap);
        Assert.NotNull(starsForUpdate);
        Assert.Contains(starsForUpdate, s => s.Row == 0 && s.Col == 2 && s.Item == GoalItem.SPACE);
        Assert.Contains(starsForUpdate, s => s.Row == 0 && s.Col == 5 && s.Item == GoalItem.SPACE);
        Assert.Contains(starsForUpdate, s => s.Row == 0 && s.Col == 8 && s.Item == GoalItem.SPACE);
        Assert.Contains(starsForUpdate, s => s.Row == 0 && s.Col == 14 && s.Item == GoalItem.SPACE);
    }

    [Fact]
    public void ToGoalItem()
    {
        var fileContent = File.ReadAllText(Path.Combine(TestProjectPath(), "Fixtures", "map.json"));
        var jsonDocument = JsonDocument.Parse(fileContent);
        MapResponse mapResponse = JsonSerializer
            .Deserialize<MapResponse>(jsonDocument, SerializerFactory.CreateOptions())
            .NotNull();
        MapGoalResponse mapGoalResponse = mapResponse.ToGoalResponse();

        Assert.Equal(GoalItem.BLUE_SOLOON, mapGoalResponse.Goal[1][1]);
        Assert.Equal(GoalItem.WHITE_SOLOON, mapGoalResponse.Goal[1][8]);
        Assert.Equal(GoalItem.WHITE_SOLOON, mapGoalResponse.Goal[1][10]);

        Assert.Equal(GoalItem.PURPLE_SOLOON, mapGoalResponse.Goal[11][10]);
        Assert.Equal(GoalItem.RED_SOLOON, mapGoalResponse.Goal[11][11]);

        Assert.Equal(GoalItem.UP_COMETH, mapGoalResponse.Goal[20][1]);
        Assert.Equal(GoalItem.DOWN_COMETH, mapGoalResponse.Goal[20][2]);
        Assert.Equal(GoalItem.LEFT_COMETH, mapGoalResponse.Goal[20][3]);
        Assert.Equal(GoalItem.RIGHT_COMETH, mapGoalResponse.Goal[20][4]);
        Assert.Equal(GoalItem.POLYANET, mapGoalResponse.Goal[20][8]);

        Assert.Equal(GoalItem.SPACE, mapGoalResponse.Goal[0][0]);
        Assert.Equal(GoalItem.SPACE, mapGoalResponse.Goal[5][5]);
        Assert.Equal(GoalItem.SPACE, mapGoalResponse.Goal[15][15]);
    }
}

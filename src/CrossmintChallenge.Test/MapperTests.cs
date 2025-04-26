using CrossmintChallenge.Clients;
using CrossmintChallenge.Host;
using CrossmintChallenge.Host.Services;
using System.Text.Json;

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
    public void ToGoalItem()
    {
        var fileContent = File.ReadAllText(Path.Combine(TestProjectPath(), "Fixtures", "map.json"));
        var jsonDocument = JsonDocument.Parse(fileContent);
        MapResponse mapResponse = JsonSerializer.Deserialize<MapResponse>(jsonDocument, SerializerFactory.CreateOptions()).NotNull();
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

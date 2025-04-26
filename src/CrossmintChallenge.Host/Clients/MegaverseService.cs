using CrossmintChallenge.Host;
using Flurl;
using Serilog;

namespace CrossmintChallenge.Clients;

public class MegaverseService
{
    public MegaverseClient MegaverseClient { get; init; }

    public MegaverseService(MegaverseClient megaverseClient)
    {
        MegaverseClient = megaverseClient.NotNull();
    }

    public Task CreateStar(
        GoalItem starGoal,
        (int Row, int Col) location,
        Url challengeUrl,
        string candidateId
    )
    {
        Log.Information("create star task {@starGoal} {@location}", starGoal, location);
        Task task = starGoal.ToCreate(location, challengeUrl, candidateId, MegaverseClient);
        return task;
    }
}

public static class MegaverseServiceExtension
{
    public static Task ToCreate(
        this GoalItem starGoal,
        (int Row, int Col) location,
        Url challengeUrl,
        string candidateId,
        MegaverseClient megaverseClient
    )
    {
        return starGoal switch
        {
            GoalItem.POLYANET => megaverseClient.PostPolyanetAsync(
                challengeUrl,
                location.Row,
                location.Col,
                candidateId
            ),
            GoalItem.BLUE_SOLOON => megaverseClient.PostSoloonAsync(
                challengeUrl,
                ColorEnum.blue,
                location.Row,
                location.Col,
                candidateId
            ),
            GoalItem.RED_SOLOON => megaverseClient.PostSoloonAsync(
                challengeUrl,
                ColorEnum.red,
                location.Row,
                location.Col,
                candidateId
            ),
            GoalItem.WHITE_SOLOON => megaverseClient.PostSoloonAsync(
                challengeUrl,
                ColorEnum.white,
                location.Row,
                location.Col,
                candidateId
            ),
            GoalItem.PURPLE_SOLOON => megaverseClient.PostSoloonAsync(
                challengeUrl,
                ColorEnum.purple,
                location.Row,
                location.Col,
                candidateId
            ),
            GoalItem.UP_COMETH => megaverseClient.PostComethAsync(
                challengeUrl,
                DirectionEnum.up,
                location.Row,
                location.Col,
                candidateId
            ),
            GoalItem.RIGHT_COMETH => megaverseClient.PostComethAsync(
                challengeUrl,
                DirectionEnum.right,
                location.Row,
                location.Col,
                candidateId
            ),
            GoalItem.DOWN_COMETH => megaverseClient.PostComethAsync(
                challengeUrl,
                DirectionEnum.down,
                location.Row,
                location.Col,
                candidateId
            ),
            GoalItem.LEFT_COMETH => megaverseClient.PostComethAsync(
                challengeUrl,
                DirectionEnum.left,
                location.Row,
                location.Col,
                candidateId
            ),
            _ => throw new NotImplementedException($"GoalItem {starGoal} not implemented"),
        };
    }
}


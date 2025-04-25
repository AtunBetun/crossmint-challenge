using Flurl;
using Flurl.Http;
using Serilog;

namespace CrossmintChallenge.Clients;

public record MapGoalResponse(List<List<GoalItem>> Goal);

public enum GoalItem
{
    SPACE,
    POLYANET,
}

public record MapResponse(Map Map);

public record Map(
    string _id,
    List<List<CellContent?>> Content,
    string CandidateId,
    int Phase,
    int __v
);

public record CellContent(int Type);

public static class MegaverseMapClient
{
    public static async Task<MapGoalResponse> GetMapGoalAsync(Url challengeUrl, string candidateId)
    {
        Url url = challengeUrl
            .AppendPathSegments("map")
            .AppendPathSegments(candidateId)
            .AppendPathSegments("goal");

        var result = await url.WithJsonSettings().GetJsonAsync<MapGoalResponse>();
        Log.Debug("{@result}", result);
        return result;
    }

    public static async Task<MapResponse> GetMapAsync(Url challengeUrl, string candidateId)
    {
        Url url = challengeUrl.AppendPathSegments("map").AppendPathSegments(candidateId);
        var result = await url.GetJsonAsync<MapResponse>();
        Log.Debug("{@result}", result);
        return result;
    }

    public static async Task DeletePolyanetAsync(
        Url challengeUrl,
        int row,
        int column,
        string candidateId
    )
    {
        Url url = challengeUrl.AppendPathSegment("polyanets");
        var formData = new Dictionary<string, string>
        {
            { "row", row.ToString() },
            { "column", column.ToString() },
            { "candidateId", candidateId },
        };
        var response = await url.WithHeader("Accept", "application/x-www-form-urlencoded")
            .DeleteUrlEncodedAsync(formData);
        Log.Debug("Deleted POLYANET at ({Row}, {Column}): {@Response}", row, column, response);
    }

    public static async Task PostPolyanetAsync(Url challengeUrl, int row, int column, string candidateId)
    {
        Url url = challengeUrl.AppendPathSegment("polyanets");
        var formData = new Dictionary<string, string>
        {
            { "row", row.ToString() },
            { "column", column.ToString() },
            { "candidateId", candidateId },
        };
        var response = await url.WithHeader("Accept", "application/x-www-form-urlencoded")
            .PostUrlEncodedAsync(formData);
        Log.Debug("Posted POLYANET at ({Row}, {Column}): {@Response}", row, column, response);
    }
}

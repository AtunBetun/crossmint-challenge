using Flurl;
using Flurl.Http;
using Serilog;

namespace CrossmintChallenge.Clients;

public enum GoalItem
{
    SPACE,
    POLYANET,
}

public record MapResponse(List<List<GoalItem>> Goal);

public static class MegaverseMapClient
{
    public static async Task<MapResponse> GetMap(Url challengeUrl, string candidateId)
    {
        Url url = challengeUrl
            .AppendPathSegments("map")
            .AppendPathSegments(candidateId)
            .AppendPathSegments("goal");

        var result = await url.GetJsonAsync<MapResponse>();
        Log.Debug("{@result}", result);

        return result;
    }
}

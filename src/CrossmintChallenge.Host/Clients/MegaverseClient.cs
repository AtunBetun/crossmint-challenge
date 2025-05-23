using CrossmintChallenge.Host;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Serilog;

namespace CrossmintChallenge.Clients;

public class MegaverseClient : IMegaverseClient
{
    public IFlurlClient FlurlClient { get; init; }

    public MegaverseClient(IFlurlClientCache flurlClientCache)
    {
        FlurlClient = flurlClientCache.NotNull().Get(nameof(MegaverseClient)).NotNull();
    }

    public async Task<MapGoalResponse> GetMapGoalAsync(Url challengeUrl, string candidateId)
    {
        Url url = challengeUrl
            .AppendPathSegments("map")
            .AppendPathSegments(candidateId)
            .AppendPathSegments("goal");

        var result = await FlurlClient
            .Request(url)
            .WithJsonSettings(SerializerFactory.CreateOptions())
            .GetJsonAsync<MapGoalResponse>();
        Log.Debug("{@result}", result);
        return result;
    }

    public async Task<MapResponse> GetMapAsync(Url challengeUrl, string candidateId)
    {
        Url url = challengeUrl.AppendPathSegments("map").AppendPathSegments(candidateId);
        var result = await FlurlClient
            .Request(url)
            .WithJsonSettings(SerializerFactory.CreateOptions())
            .GetJsonAsync<MapResponse>();
        Log.Debug("{@result}", result);
        return result;
    }

    public async Task<IFlurlResponse> PostPolyanetAsync(
        Url challengeUrl,
        int row,
        int column,
        string candidateId
    )
    {
        Url url = challengeUrl.AppendPathSegment(MegaverseStarsEnum.polyanets.ToString());
        var formData = new Dictionary<string, string>
        {
            { "row", row.ToString() },
            { "column", column.ToString() },
            { "candidateId", candidateId },
        };

        Log.Debug("Posting POLYANET at ({Row}, {Column})", row, column);
        var response = await FlurlClient
            .Request(url)
            .WithHeader("Accept", "application/x-www-form-urlencoded")
            .PostUrlEncodedAsync(formData);
        Log.Debug("Posted POLYANET at ({Row}, {Column})", row, column);
        return response;
    }

    public async Task<IFlurlResponse> DeletePolyanetAsync(
        Url challengeUrl,
        int row,
        int column,
        string candidateId
    )
    {
        Url url = challengeUrl.AppendPathSegment(MegaverseStarsEnum.polyanets.ToString());
        var formData = new Dictionary<string, string>
        {
            { "row", row.ToString() },
            { "column", column.ToString() },
            { "candidateId", candidateId },
        };

        Log.Debug("Deleting POLYANET at ({Row}, {Column})", row, column);
        var response = await FlurlClient
            .Request(url)
            .WithHeader("Accept", "application/x-www-form-urlencoded") // TODO: extract to method
            .DeleteUrlEncodedAsync(formData);

        Log.Debug("Deleted POLYANET at ({Row}, {Column})", row, column);
        return response;
    }

    public async Task<IFlurlResponse> PostSoloonAsync(
        Url challengeUrl,
        ColorEnum color,
        int row,
        int column,
        string candidateId
    )
    {
        Url url = challengeUrl.AppendPathSegment(MegaverseStarsEnum.soloons.ToString());
        var formData = new Dictionary<string, string>
        {
            { "row", row.ToString() },
            { "column", column.ToString() },
            { "color", color.ToString() },
            { "candidateId", candidateId },
        };

        Log.Debug("Posting SOLOON at ({Row}, {Column})", row, column);
        var response = await FlurlClient
            .Request(url)
            .WithHeader("Accept", "application/x-www-form-urlencoded")
            .PostUrlEncodedAsync(formData);
        Log.Debug("Posted SOLOON at ({Row}, {Column})", row, column);
        return response;
    }

    public async Task<IFlurlResponse> PostComethAsync(
        Url challengeUrl,
        DirectionEnum direction,
        int row,
        int column,
        string candidateId
    )
    {
        Url url = challengeUrl.AppendPathSegment(MegaverseStarsEnum.comeths.ToString());
        var formData = new Dictionary<string, string>
        {
            { "row", row.ToString() },
            { "column", column.ToString() },
            { "direction", direction.ToString() },
            { "candidateId", candidateId },
        };

        Log.Debug("Posting SOLOON at ({Row}, {Column})", row, column);
        var response = await FlurlClient
            .Request(url)
            .WithHeader("Accept", "application/x-www-form-urlencoded")
            .PostUrlEncodedAsync(formData);
        Log.Debug("Posted SOLOON at ({Row}, {Column})", row, column);
        return response;
    }
}

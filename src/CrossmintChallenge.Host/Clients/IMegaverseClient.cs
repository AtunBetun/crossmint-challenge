using Flurl;
using Flurl.Http;

namespace CrossmintChallenge.Clients;

public interface IMegaverseClient
{
    Task<MapGoalResponse> GetMapGoalAsync(Url challengeUrl, string candidateId);
    Task<MapResponse> GetMapAsync(Url challengeUrl, string candidateId);

    Task<IFlurlResponse> PostPolyanetAsync(
        Url challengeUrl,
        int row,
        int column,
        string candidateId
    );
    Task<IFlurlResponse> DeletePolyanetAsync(
        Url challengeUrl,
        int row,
        int column,
        string candidateId
    );

    Task<IFlurlResponse> PostSoloonAsync(
        Url challengeUrl,
        ColorEnum color,
        int row,
        int column,
        string candidateId
    );
    Task<IFlurlResponse> PostComethAsync(
        Url challengeUrl,
        DirectionEnum direction,
        int row,
        int column,
        string candidateId
    );
}

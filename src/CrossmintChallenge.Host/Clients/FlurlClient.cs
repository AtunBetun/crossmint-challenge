using Flurl.Http;

namespace CrossmintChallenge.Clients;

public class RetryFlurlClient : FlurlClient, IFlurlClient
{
    public new async Task<IFlurlResponse> SendAsync(
        IFlurlRequest request,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default(CancellationToken)
    )
    {
        return await Retry
            .BuildRetryPolicy()
            .ExecuteAsync(
                async () => await base.SendAsync(request, completionOption, cancellationToken)
            );
    }
}

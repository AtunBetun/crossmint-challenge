using System.Net;
using Polly;
using Polly.Retry;
using Serilog;

namespace CrossmintChallenge.Clients;

// https://brunomj.medium.com/net-5-0-resilient-http-client-with-polly-and-flurl-b7de936fd70c
public static class Retry
{
    public static bool IsTransientError(HttpResponseMessage response)
    {
        int[] httpStatusCodesWorthRetrying =
        {
            (int)HttpStatusCode.TooManyRequests, // 429
            (int)HttpStatusCode.RequestTimeout, // 408
            (int)HttpStatusCode.BadGateway, // 502
            (int)HttpStatusCode.ServiceUnavailable, // 503
            (int)HttpStatusCode.GatewayTimeout, // 504
        };
        return httpStatusCodesWorthRetrying.Contains((int)response.StatusCode);
    }

    public static AsyncRetryPolicy<HttpResponseMessage> BuildRetryPolicy()
    {
        var retryPolicy = Policy<HttpResponseMessage>
            .HandleResult(result => IsTransientError(result))
            .WaitAndRetryAsync(
                15,
                retryAttempt =>
                {
                    var nextAttemptIn = TimeSpan.FromSeconds(10);
                    Log.Error(
                        "Retry attempt {@retryAttempt} to make request. Next try on {@nextAttemptIn} seconds.",
                        retryAttempt,
                        nextAttemptIn
                    );
                    return nextAttemptIn;
                }
            );

        return retryPolicy;
    }
}

public class PollyHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        Log.Debug("using polly handler");
        return await Retry.BuildRetryPolicy().ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
    }
}

using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Flurl.Http;
using Flurl.Http.Configuration;
using Flurl.Http.Content;
using Polly;
using Polly.Retry;
using Serilog;

namespace CrossmintChallenge.Clients;

public static class FlurlExtensions
{
    public static IFlurlRequest WithJsonSettings(this IFlurlRequest request) =>
        request.WithSettings(settings =>
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            settings.JsonSerializer = new DefaultJsonSerializer(options);
        });

    public static Task<IFlurlResponse> DeleteUrlEncodedAsync(
        this IFlurlRequest request,
        object body,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
        CancellationToken cancellationToken = default(CancellationToken)
    )
    {
        request.EnsureClient();
        CapturedUrlEncodedContent content = new CapturedUrlEncodedContent(
            request.Settings.UrlEncodedSerializer.Serialize(body)
        );
        return request.SendAsync(HttpMethod.Delete, content, completionOption, cancellationToken);
    }

    // https://brunomj.medium.com/net-5-0-resilient-http-client-with-polly-and-flurl-b7de936fd70c
    public static bool IsTransientError(FlurlHttpException exception)
    {
        int[] httpStatusCodesWorthRetrying =
        {
            (int)HttpStatusCode.TooManyRequests, // 429
            (int)HttpStatusCode.RequestTimeout, // 408
            (int)HttpStatusCode.BadGateway, // 502
            (int)HttpStatusCode.ServiceUnavailable, // 503
            (int)HttpStatusCode.GatewayTimeout, // 504
        };

        return exception.StatusCode.HasValue
            && httpStatusCodesWorthRetrying.Contains(exception.StatusCode.Value);
    }

    public static AsyncRetryPolicy BuildRetryPolicy()
    {
        var retryPolicy = Policy
            .Handle<FlurlHttpException>(IsTransientError)
            .WaitAndRetryAsync(
                5,
                retryAttempt =>
                {
                    var nextAttemptIn = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
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

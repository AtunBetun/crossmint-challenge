using System.Text.Json;
using Flurl.Http;
using Flurl.Http.Configuration;
using Flurl.Http.Content;

namespace CrossmintChallenge.Clients;

public static class FlurlExtensions
{
    public static IFlurlRequest WithJsonSettings(
        this IFlurlRequest request,
        JsonSerializerOptions options
    ) =>
        request.WithSettings(settings =>
        {
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
}

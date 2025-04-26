using System.Text.Json;
using System.Text.Json.Serialization;
using Flurl.Http;
using Flurl.Http.Configuration;
using Flurl.Http.Content;

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

}

using System.Text.Json;

namespace CommunityDailyDiary.Tests.Infrastructure;

public static class TestHttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task<(HttpResponseMessage Response, T? Content)> SendAsync<T>(
        this HttpClient client,
        HttpRequestMessage request)
    {
        var response = await client.SendAsync(request);
        var content = default(T);

        if (response.Content.Headers.ContentLength > 0)
        {
            var contentStream = await response.Content.ReadAsStreamAsync();
            content = await JsonSerializer.DeserializeAsync<T>(contentStream, JsonOptions);
        }

        return (response, content);
    }

    public static Uri GetUriWithQueryString(string baseUri, Dictionary<string, string> queryParams)
    {
        var queryString = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
        return new Uri($"{baseUri}?{queryString}", UriKind.Relative);
    }
}
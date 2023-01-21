using Serilog.Sinks.Http;

namespace Notification.Api.Clients;

public class CustomHttpClient : IHttpClient
{
    private readonly HttpClient httpClient;

    public CustomHttpClient()
    {
        httpClient = new HttpClient();
    }

    public void Configure(IConfiguration configuration)
    {
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
    {
        using var content = new StreamContent(contentStream);
        content.Headers.Add("Content-Type", "application/json");

        var response = await httpClient
            .PostAsync(requestUri, content)
            .ConfigureAwait(false);

        return response;
    }

    public void Dispose()
    {
        httpClient?.Dispose();
    }
}
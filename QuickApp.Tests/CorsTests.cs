using System.Net;

namespace QuickApp.Tests;

public class CorsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CorsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Cors_Rejects_Request_From_NonAllowed_Origin()
    {
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/customer");
        request.Headers.Add("Origin", "https://malicious-site.com");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await _client.SendAsync(request);

        // The response should not contain Access-Control-Allow-Origin for non-allowed origins
        Assert.False(
            response.Headers.Contains("Access-Control-Allow-Origin") &&
            response.Headers.GetValues("Access-Control-Allow-Origin").Contains("https://malicious-site.com"),
            "CORS should not allow requests from non-configured origins");
    }

    [Fact]
    public async Task Cors_Does_Not_Echo_Arbitrary_Origins()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/customer");
        request.Headers.Add("Origin", "https://evil.example.com");

        var response = await _client.SendAsync(request);

        if (response.Headers.Contains("Access-Control-Allow-Origin"))
        {
            var allowedOrigin = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            Assert.NotEqual("https://evil.example.com", allowedOrigin);
        }
    }
}

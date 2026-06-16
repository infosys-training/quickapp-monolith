using System.Net;

namespace QuickApp.Tests;

public class RateLimitingTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public RateLimitingTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task TokenEndpoint_Returns_429_After_Threshold_Exceeded()
    {
        var client = _factory.CreateClient();

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["username"] = "test",
            ["password"] = "test",
            ["scope"] = "openid"
        });

        // Send requests up to and beyond the rate limit (10 per minute)
        HttpResponseMessage? lastResponse = null;
        for (int i = 0; i < 15; i++)
        {
            lastResponse = await client.PostAsync("/connect/token", content);
            if (lastResponse.StatusCode == HttpStatusCode.TooManyRequests)
                break;
        }

        Assert.NotNull(lastResponse);
        Assert.Equal(HttpStatusCode.TooManyRequests, lastResponse.StatusCode);
    }
}

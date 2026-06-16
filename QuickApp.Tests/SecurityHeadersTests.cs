namespace QuickApp.Tests;

public class SecurityHeadersTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SecurityHeadersTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Response_Contains_XContentTypeOptions_Header()
    {
        var response = await _client.GetAsync("/api/customer");

        Assert.True(response.Headers.Contains("X-Content-Type-Options"));
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").First());
    }

    [Fact]
    public async Task Response_Contains_XFrameOptions_Header()
    {
        var response = await _client.GetAsync("/api/customer");

        Assert.True(response.Headers.Contains("X-Frame-Options"));
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").First());
    }

    [Fact]
    public async Task Response_Contains_ReferrerPolicy_Header()
    {
        var response = await _client.GetAsync("/api/customer");

        Assert.True(response.Headers.Contains("Referrer-Policy"));
        Assert.Equal("strict-origin-when-cross-origin", response.Headers.GetValues("Referrer-Policy").First());
    }

    [Fact]
    public async Task Response_Contains_PermissionsPolicy_Header()
    {
        var response = await _client.GetAsync("/api/customer");

        Assert.True(response.Headers.Contains("Permissions-Policy"));
        Assert.Equal("camera=(), microphone=(), geolocation=()", response.Headers.GetValues("Permissions-Policy").First());
    }
}

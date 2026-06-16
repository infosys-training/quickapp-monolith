using System.Net;

namespace QuickApp.Tests;

public class AuthenticationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthenticationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CustomerController_Returns_401_For_Unauthenticated_Requests()
    {
        var response = await _client.GetAsync("/api/customer");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CustomerController_Post_Returns_401_For_Unauthenticated_Requests()
    {
        var content = new StringContent("\"test\"", System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/customer", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

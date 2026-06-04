using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OrderIntegrationTests;

/// <summary>
/// Integration smoke tests that verify order creation flows through both services.
/// Set ORDER_SERVICE_URL and MONOLITH_URL environment variables before running.
/// Defaults: order-service at http://localhost:5003, monolith at http://localhost:5225.
/// </summary>
public class OrderSmokeTest : IDisposable
{
    private readonly HttpClient _orderServiceClient;
    private readonly HttpClient _monolithClient;

    public OrderSmokeTest()
    {
        var orderServiceUrl = Environment.GetEnvironmentVariable("ORDER_SERVICE_URL")
                              ?? "http://localhost:5003";
        var monolithUrl = Environment.GetEnvironmentVariable("MONOLITH_URL")
                          ?? "http://localhost:5225";

        _orderServiceClient = new HttpClient { BaseAddress = new Uri(orderServiceUrl) };
        _monolithClient = new HttpClient { BaseAddress = new Uri(monolithUrl) };
    }

    [Fact]
    public async Task OrderService_HealthCheck_ReturnsHealthy()
    {
        var response = await _orderServiceClient.GetAsync("/healthz");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OrderService_CreateAndGet_ReturnsCreatedOrder()
    {
        var request = new
        {
            Discount = 10.50m,
            Comments = "Smoke test order",
            CashierId = "test-cashier",
            CustomerId = 1,
            OrderDetails = new[]
            {
                new { UnitPrice = 99.99m, Quantity = 2, Discount = 0m, ProductId = 1 }
            }
        };

        var createResponse = await _orderServiceClient.PostAsJsonAsync("/api/order", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal(10.50m, created.Discount);
        Assert.Equal("Smoke test order", created.Comments);
        Assert.Equal(1, created.CustomerId);
        Assert.Single(created.OrderDetails);

        var getResponse = await _orderServiceClient.GetAsync($"/api/order/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
    }

    [Fact]
    public async Task Monolith_CreateOrderViaProxy_FlowsThroughOrderService()
    {
        var request = new
        {
            Discount = 5.00m,
            Comments = "Monolith proxy smoke test",
            CashierId = "test-cashier",
            CustomerId = 1,
            OrderDetails = new[]
            {
                new { UnitPrice = 50.00m, Quantity = 1, Discount = 0m, ProductId = 1 }
            }
        };

        var createResponse = await _monolithClient.PostAsJsonAsync("/api/order", request);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Monolith proxy smoke test", created.Comments);

        var directGet = await _orderServiceClient.GetAsync($"/api/order/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, directGet.StatusCode);

        var directOrder = await directGet.Content.ReadFromJsonAsync<OrderResponse>();
        Assert.NotNull(directOrder);
        Assert.Equal(created.Id, directOrder.Id);
        Assert.Equal("Monolith proxy smoke test", directOrder.Comments);
    }

    public void Dispose()
    {
        _orderServiceClient.Dispose();
        _monolithClient.Dispose();
    }

    private record OrderDetailResponse(int Id, decimal UnitPrice, int Quantity, decimal Discount, int ProductId, int OrderId);
    private record OrderResponse(int Id, decimal Discount, string? Comments, string? CashierId, int CustomerId,
        DateTime CreatedDate, DateTime UpdatedDate, List<OrderDetailResponse> OrderDetails);
}

using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Softtek_Invoice_Back.Data;
using Softtek_Invoice_Back.Presentation.DTOs;
using Xunit;
using FluentAssertions;

namespace Softtek_Invoice_Back.Tests.Integration;

/// <summary>
/// Integration tests for OrdersController
/// </summary>
public class OrdersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public OrdersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString());
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_WithValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            BranchId = "BR001",
            ItemId = "ITEM001",
            Quantity = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<OrderResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.BranchId.Should().Be(request.BranchId);
        result.Data.ItemId.Should().Be(request.ItemId);
        result.Data.Quantity.Should().Be(request.Quantity);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            BranchId = "",
            ItemId = "ITEM001",
            Quantity = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<OrderResponse>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task CreateOrder_WithZeroQuantity_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            BranchId = "BR001",
            ItemId = "ITEM001",
            Quantity = 0
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}


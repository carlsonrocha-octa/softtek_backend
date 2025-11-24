using Softtek_Invoice_Back.Domain.Interfaces;

namespace Softtek_Invoice_Back.Infrastructure.Sap;

/// <summary>
/// Mock implementation of SAP API client for simulation
/// </summary>
public class MockSapApiClient : ISapApiClient
{
    private readonly ILogger<MockSapApiClient> _logger;

    public MockSapApiClient(ILogger<MockSapApiClient> logger)
    {
        _logger = logger;
    }

    public async Task SendOrderToSapAsync(Guid orderId, string branchId, string itemId, int quantity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Mock SAP API: Sending order to SAP - OrderId: {OrderId}, BranchId: {BranchId}, ItemId: {ItemId}, Quantity: {Quantity}",
            orderId,
            branchId,
            itemId,
            quantity);

        // Simulate API call delay
        await Task.Delay(100, cancellationToken);

        // Simulate successful SAP integration
        _logger.LogInformation("Mock SAP API: Order {OrderId} successfully processed by SAP", orderId);
    }
}


namespace Softtek_Invoice_Back.Infrastructure.Sap;

/// <summary>
/// SAP API client interface (mock)
/// </summary>
public interface ISapApiClient
{
    Task SendOrderToSapAsync(Guid orderId, string branchId, string itemId, int quantity, CancellationToken cancellationToken = default);
}


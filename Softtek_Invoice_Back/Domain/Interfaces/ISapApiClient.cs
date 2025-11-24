namespace Softtek_Invoice_Back.Domain.Interfaces;

/// <summary>
/// SAP API client interface
/// </summary>
public interface ISapApiClient
{
    Task SendOrderToSapAsync(Guid orderId, string branchId, string itemId, int quantity, CancellationToken cancellationToken = default);
}

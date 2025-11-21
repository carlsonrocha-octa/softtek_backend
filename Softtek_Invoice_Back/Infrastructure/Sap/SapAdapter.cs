using Softtek_Invoice_Back.Domain.Events;
using Softtek_Invoice_Back.Domain.Interfaces;

namespace Softtek_Invoice_Back.Infrastructure.Sap;

/// <summary>
/// SAP adapter implementation (mock) for processing order events
/// </summary>
public class SapAdapter : ISapAdapter
{
    private readonly ILogger<SapAdapter> _logger;
    private readonly ISapApiClient _sapApiClient;

    public SapAdapter(ILogger<SapAdapter> logger, ISapApiClient sapApiClient)
    {
        _logger = logger;
        _sapApiClient = sapApiClient;
    }

    public async Task ProcessOrderCreatedEventAsync(OrderCreatedEvent orderEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing OrderCreatedEvent for OrderId: {OrderId}, BranchId: {BranchId}, ItemId: {ItemId}, Quantity: {Quantity}",
            orderEvent.OrderId,
            orderEvent.BranchId,
            orderEvent.ItemId,
            orderEvent.Quantity);

        try
        {
            await _sapApiClient.SendOrderToSapAsync(
                orderEvent.OrderId,
                orderEvent.BranchId,
                orderEvent.ItemId,
                orderEvent.Quantity,
                cancellationToken);

            _logger.LogInformation("Order {OrderId} successfully sent to SAP", orderEvent.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order {OrderId} in SAP adapter", orderEvent.OrderId);
            throw;
        }
    }
}


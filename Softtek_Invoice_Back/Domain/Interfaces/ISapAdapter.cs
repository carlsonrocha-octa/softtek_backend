using Softtek_Invoice_Back.Domain.Events;

namespace Softtek_Invoice_Back.Domain.Interfaces;

/// <summary>
/// SAP integration adapter interface
/// </summary>
public interface ISapAdapter
{
    Task ProcessOrderCreatedEventAsync(OrderCreatedEvent orderEvent, CancellationToken cancellationToken = default);
}


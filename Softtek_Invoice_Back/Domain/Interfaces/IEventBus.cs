namespace Softtek_Invoice_Back.Domain.Interfaces;

/// <summary>
/// Event bus interface for publishing and subscribing to events
/// </summary>
public interface IEventBus
{
    Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class;
    void Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : class;
}


using Softtek_Invoice_Back.Domain.Interfaces;

namespace Softtek_Invoice_Back.Infrastructure.EventBus;

/// <summary>
/// In-memory event bus implementation for simulation
/// </summary>
public class InMemoryEventBus : IEventBus
{
    private readonly Dictionary<Type, List<Func<object, CancellationToken, Task>>> _handlers = new();
    private readonly ILogger<InMemoryEventBus> _logger;

    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : class
    {
        var eventType = typeof(T);
        _logger.LogInformation("Publishing event {EventType} with data: {@EventData}", eventType.Name, eventData);

        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            foreach (var handler in handlers)
            {
                try
                {
                    await handler(eventData, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling event {EventType}", eventType.Name);
                }
            }
        }
        else
        {
            _logger.LogWarning("No handlers registered for event type {EventType}", eventType.Name);
        }
    }

    public void Subscribe<T>(Func<T, CancellationToken, Task> handler) where T : class
    {
        var eventType = typeof(T);
        
        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<Func<object, CancellationToken, Task>>();
        }

        _handlers[eventType].Add((obj, ct) => handler((T)obj, ct));
        _logger.LogInformation("Subscribed handler for event type {EventType}", eventType.Name);
    }
}


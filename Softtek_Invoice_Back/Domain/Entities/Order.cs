namespace Softtek_Invoice_Back.Domain.Entities;

/// <summary>
/// Represents an order for supplies (insumos)
/// </summary>
public class Order
{
    public Guid Id { get; set; }
    public string BranchId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
}

/// <summary>
/// Order status enumeration
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    SentToSap = 2,
    Completed = 3,
    Failed = 4
}


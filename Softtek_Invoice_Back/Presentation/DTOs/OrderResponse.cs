namespace Softtek_Invoice_Back.Presentation.DTOs;

/// <summary>
/// Response DTO for order operations
/// </summary>
public class OrderResponse
{
    public Guid Id { get; set; }
    public string BranchId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}


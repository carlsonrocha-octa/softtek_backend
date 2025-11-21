namespace Softtek_Invoice_Back.Presentation.DTOs;

/// <summary>
/// Request DTO for creating an order
/// </summary>
public class CreateOrderRequest
{
    public string BranchId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}


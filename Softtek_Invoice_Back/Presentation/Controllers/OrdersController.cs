using Microsoft.AspNetCore.Mvc;
using Softtek_Invoice_Back.Domain.Entities;
using Softtek_Invoice_Back.Domain.Interfaces;
using Softtek_Invoice_Back.Presentation.DTOs;

namespace Softtek_Invoice_Back.Presentation.Controllers;

/// <summary>
/// Controller for order operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IOrderService orderService,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="request">Order creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(
                request.BranchId,
                request.ItemId,
                request.Quantity,
                cancellationToken);

            var response = new OrderResponse
            {
                Id = order.Id,
                BranchId = order.BranchId,
                ItemId = order.ItemId,
                Quantity = order.Quantity,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString()
            };

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.Id },
                ApiResponse<OrderResponse>.SuccessResponse(response, "Order created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<OrderResponse>.ErrorResponse("An error occurred while creating the order"));
        }
    }

    /// <summary>
    /// Gets an order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> GetOrder(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
            
            if (order == null)
            {
                return NotFound(ApiResponse<OrderResponse>.ErrorResponse("Order not found"));
            }

            var response = new OrderResponse
            {
                Id = order.Id,
                BranchId = order.BranchId,
                ItemId = order.ItemId,
                Quantity = order.Quantity,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString()
            };

            return Ok(ApiResponse<OrderResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", id);
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<OrderResponse>.ErrorResponse("An error occurred while retrieving the order"));
        }
    }

    /// <summary>
    /// Gets all orders
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of orders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponse>>>> GetAllOrders(
        CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
            
            var response = orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                BranchId = o.BranchId,
                ItemId = o.ItemId,
                Quantity = o.Quantity,
                CreatedAt = o.CreatedAt,
                Status = o.Status.ToString()
            });

            return Ok(ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all orders");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<IEnumerable<OrderResponse>>.ErrorResponse("An error occurred while retrieving orders"));
        }
    }
}


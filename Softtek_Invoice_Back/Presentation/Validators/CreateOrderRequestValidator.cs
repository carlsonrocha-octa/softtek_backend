using FluentValidation;
using Softtek_Invoice_Back.Presentation.DTOs;

namespace Softtek_Invoice_Back.Presentation.Validators;

/// <summary>
/// Validator for CreateOrderRequest
/// </summary>
public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("BranchId is required")
            .MaximumLength(50)
            .WithMessage("BranchId must not exceed 50 characters");

        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("ItemId is required")
            .MaximumLength(50)
            .WithMessage("ItemId must not exceed 50 characters");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero");
    }
}


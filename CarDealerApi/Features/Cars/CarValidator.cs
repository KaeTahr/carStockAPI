using FastEndpoints;
using FluentValidation;

namespace Features.Cars;

public class AddCarValidator : Validator<AddCarRequest>
{
    public AddCarValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty().WithMessage("Make is required.");
        
        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.");

        RuleFor(x => x.Year)
            .NotNull().WithMessage("Year is required.")
            .NotEmpty().WithMessage("Year is required.")
            .InclusiveBetween(1886, DateTime.Now.Year)
            .WithMessage($"Year must be between 1886 and {DateTime.Now.Year + 1}.");

        RuleFor(x => x.Price)
            .NotNull().WithMessage("Price is required.")
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.");
        
        RuleFor(x => x.Stock)
            .NotNull().WithMessage("Stock is required.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative.");
    }

    public class UpdateStockValidator : Validator<UpdateStockRequest>
    {
        public UpdateStockValidator()
        {
            RuleFor(x => x.Stock)
                .NotEmpty().WithMessage("Stock is required.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock cannot be negative.");
        }
    }
}
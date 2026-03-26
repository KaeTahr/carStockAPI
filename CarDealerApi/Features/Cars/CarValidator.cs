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
            .InclusiveBetween(1886, DateTime.Now.Year)
            .WithMessage($"Year must be between 1886 and {DateTime.Now.Year + 1}.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative.");
    }

    public class UpdateStockValidator : Validator<UpdateStockRequest>
    {
        public UpdateStockValidator()
        {
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock cannot be negative.");
        }
    }
}
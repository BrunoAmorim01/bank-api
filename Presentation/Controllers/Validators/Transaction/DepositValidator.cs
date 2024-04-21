using FluentValidation;

namespace api.Presentation.Controllers.Validators;

public class DepositValidator : AbstractValidator<DepositDto>
{
    public DepositValidator()
    {
        RuleFor(x => x.Amount)
        .GreaterThanOrEqualTo(20)
        .WithMessage("The minimum amount to deposit is 20")
        .NotEmpty();

    }

}

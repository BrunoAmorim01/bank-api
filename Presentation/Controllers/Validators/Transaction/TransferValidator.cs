using FluentValidation;

namespace api.Presentation.Controllers.Validators;

public class TransferValidator : AbstractValidator<TransferDto>
{
    public TransferValidator()
    {
        RuleFor(x => x.Amount)
        .GreaterThanOrEqualTo(20)
        .WithMessage("The minimum amount to deposit is 20")
        .NotEmpty();

        RuleFor(x => x.EmailDestination)
        .EmailAddress()
        .When(x => x.AccountDestination == null)     
        .NotEmpty()   
        .WithMessage("Invalid email address");        

        RuleFor(x => x.AccountDestination)
        .SetValidator(new AccountValidator())        
        .When(x => string.IsNullOrEmpty(x.EmailDestination))
        .WithMessage("Invalid account destination");
    }

}

public class AccountValidator : AbstractValidator<AccountDto>
{
    public AccountValidator()
    {
        RuleFor(x => x.AccountDigit)
        .NotEmpty()
        .Length(1, 1)
        .Matches("^[0-9]*$");

        RuleFor(x => x.AccountNumber)
        .NotEmpty()
        .Length(1, 10)
        .Matches("^[0-9]*$");
    }
}

using api.Presentation.Controllers.Dtos.Users;
using FluentValidation;

namespace api.Presentation.Controllers.Validators.Users;

public class UserValidator : AbstractValidator<RegisterUserDto>
{
    public UserValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty()
        .MinimumLength(3);

        RuleFor(x => x.Password)
        .NotEmpty()
        .MinimumLength(6);

        RuleFor(x => x.PasswordConfirmation)
        .NotEmpty()
        .MinimumLength(6)
        .Equal(x => x.Password);

        RuleFor(x => x.Email)
        .NotEmpty()
        .EmailAddress();
    }

}

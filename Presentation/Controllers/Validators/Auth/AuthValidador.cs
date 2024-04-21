using api.Presentation.Controllers.Dtos;
using FluentValidation;

namespace api.Presentation.Controllers.Validators
{
    public class AuthValidador : AbstractValidator<AuthDto>
    {
        public AuthValidador()
        {
            RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

            RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
        }

    }
};



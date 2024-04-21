using FluentValidation;
using ValidationException = api.Infrastructure.Exceptions.ValidationException;

namespace api.Presentation.Controllers.Validators
{
    public class Validator<T>
    {
        public async Task<bool> Validate(object body, IValidator<T> validator)
        {
            var validationResult = await validator.ValidateAsync((T)body);

            if (!validationResult.IsValid)
                throw new ValidationException("Validation failed", [.. validationResult.Errors]);

            return validationResult.IsValid;
        }
    }
}
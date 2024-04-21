using FluentValidation.Results;

namespace api.Infrastructure.Exceptions;

public class ValidationException : ApiKnowException
{

    public ValidationException(string message, ValidationFailure[]? error) : base(message, error)
    {
        Errors = error;
    }

}

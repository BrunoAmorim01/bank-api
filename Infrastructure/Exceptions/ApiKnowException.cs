using FluentValidation.Results;

namespace api.Infrastructure.Exceptions;

public class ApiKnowException(string? message, ValidationFailure[]? error) : SystemException(message)
{
    public ValidationFailure[]? Errors { get; set; } = error;
}

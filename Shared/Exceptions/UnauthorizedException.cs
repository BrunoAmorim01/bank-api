using api.Infrastructure.Exceptions;

namespace api.Shared.Exceptions
{
    public class UnauthorizedException(string message) : ApiKnowException(message, null)
    {
    }
};



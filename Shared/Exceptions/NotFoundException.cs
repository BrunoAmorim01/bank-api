using api.Infrastructure.Exceptions;

namespace api.Shared.Exceptions;

public class NotFoundException(string message) : ApiKnowException(message, null)
{
}

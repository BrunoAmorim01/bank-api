using api.Infrastructure.Exceptions;

namespace api.Shared.Exceptions;

public class AlreadyExistsException(string message) : ApiKnowException(message, null)
{


}

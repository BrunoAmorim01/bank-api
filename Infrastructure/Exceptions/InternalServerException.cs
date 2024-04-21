namespace api;

public class InternalServerException
{
    public string Message { get; set; } = string.Empty;

    public InternalServerException(string message)
    {
        Message = message;
    }

}

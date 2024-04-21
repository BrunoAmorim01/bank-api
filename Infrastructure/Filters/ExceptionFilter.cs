using System.Net;
using api.Infrastructure.Exceptions;
using api.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Infrastructure.Filters;

public class ExceptionFilter(ILogger<ExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var result = context.Exception is ApiKnowException;
        if (!result) ThrowApiKnowException(context);

        HandleApiException(context);
    }
    private static void HandleApiException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ValidationException ex:
                var errors = ex.Errors;
                if (errors != null)
                {
                    var formatErrors = errors.Select(x =>
                    {
                        return new
                        {
                            field = x.PropertyName,
                            message = x.ErrorMessage
                        };
                    });

                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Result = new ObjectResult(new
                    {
                        message = ex.Message,
                        errors = formatErrors
                    });
                }
                break;

            case NotFoundException:
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                ResponseSimpleObject(context);
                break;

            case UnauthorizedException:
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                ResponseSimpleObject(context);
                break;

            case AlreadyExistsException:
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                ResponseSimpleObject(context);
                break;
        }
    }

    private static void ResponseSimpleObject(ExceptionContext context)
    {
        context.Result = new ObjectResult(new
        {
            message = context.Exception.Message
        });
    }

    private void ThrowApiKnowException(ExceptionContext context)
    {
        logger.LogError(context.Exception, "An error occurred");
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Result = new ObjectResult(new InternalServerException("Internal server error"));
    }
}

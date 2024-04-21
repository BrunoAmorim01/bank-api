using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using api.Presentation.Controllers.Dtos;
using ValidationException = api.Infrastructure.Exceptions.ValidationException;
using api.Application.Users.UseCases;

namespace api.Presentation.Controllers.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IValidator<AuthDto> validator, LoginUserUseCase loginUserUseCase) : ControllerBase
{
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] AuthDto body)
    {
        await Validate(body);
        var response = await loginUserUseCase.Execute(body.Email, body.Password);
        return Ok(response);

    }

    private async Task<bool> Validate(AuthDto body)
    {
        var validationResult = await validator.ValidateAsync(body);

        if (!validationResult.IsValid)
            throw new ValidationException("Validation failed", [.. validationResult.Errors]);

        return validationResult.IsValid;


    }

}

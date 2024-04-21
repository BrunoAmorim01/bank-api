using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using api.Application.Users;
using api.Presentation.Controllers.Dtos.Users;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ValidationException = api.Infrastructure.Exceptions.ValidationException;


namespace api.Presentation.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IValidator<RegisterUserDto> validator, RegisterUserUseCase registerUserUseCase, GetUserByIdUseCase getUserByIdUseCase) : ControllerBase
{
    private readonly IValidator<RegisterUserDto> _validator = validator;

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto body)
    {
        await Validate(body);
        var response = await registerUserUseCase.Execute(body.Name, body.Password, body.Email);
        return Ok(response);
    }

    [Authorize]
    [HttpGet]
    [Route("me")]
    public async Task<IActionResult> GetUserById()
    {
        var userIdClaim = User.Claims.First(c => c.Type == ClaimTypes.UserData);        
        var response = await getUserByIdUseCase.Execute(Guid.Parse(userIdClaim.Value));
        return Ok(response);
    }

    private async Task<bool> Validate(RegisterUserDto body)
    {
        var validationResult = await _validator.ValidateAsync(body);

        if (!validationResult.IsValid)
            throw new ValidationException("Validation failed", [.. validationResult.Errors]);

        return validationResult.IsValid;


    }
}

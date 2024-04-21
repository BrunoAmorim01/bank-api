
using System.Security.Claims;
using api.Application.Transactions;
using api.Presentation.Controllers.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TransactionsController(IValidator<DepositDto> validator, CreateDepositUseCase createDepositUseCase) : ControllerBase
{

    [HttpPost]
    [Route("deposit")]
    public async Task<IActionResult> CreateTransaction([FromBody] DepositDto body)
    {
        await new Validator<DepositDto>().Validate(body, validator);
        var userIdClaim = User.Claims.First(c => c.Type == ClaimTypes.UserData);
        var response = await createDepositUseCase.Execute(new Guid(userIdClaim.Value), body.Amount);
        return Created(string.Empty,response);

    }
}

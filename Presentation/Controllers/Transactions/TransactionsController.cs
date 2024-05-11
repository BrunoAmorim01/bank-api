
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
public class TransactionsController(
    IValidator<DepositDto> depositValidator,
    IValidator<TransferDto> transferValidator,
    CreateDepositUseCase createDepositUseCase,
    CreateTransferUseCase createTransferUseCase
    ) : ControllerBase
{

    [HttpPost]
    [Route("deposit")]
    public async Task<IActionResult> CreateDeposit([FromBody] DepositDto body)
    {
        await new Validator<DepositDto>().Validate(body, depositValidator);
        var userIdClaim = User.Claims.First(c => c.Type == ClaimTypes.UserData);
        var response = await createDepositUseCase.Execute(new Guid(userIdClaim.Value), body.Amount);
        return Created(string.Empty, response);
    }

    [HttpPost]
    [Route("transfer")]
    public async Task<IActionResult> CreateTransfer([FromBody] TransferDto body)
    {
        await new Validator<TransferDto>().Validate(body, transferValidator);
        var userIdClaim = User.Claims.First(c => c.Type == ClaimTypes.UserData);




#pragma warning disable CS8601 // Possible null reference assignment.
        var transferData = new TransferData
        {
            Amount = body.Amount,
            EmailDestination = body.EmailDestination,
            AccountDestination = new AccountTransfer
            {
                AccountDigit = body.AccountDestination?.AccountDigit,
                AccountNumber = body.AccountDestination?.AccountNumber
            }
        };
#pragma warning restore CS8601 // Possible null reference assignment.

        var response = await createTransferUseCase.Execute(new Guid(userIdClaim.Value), transferData);

        return Ok();
    }
}

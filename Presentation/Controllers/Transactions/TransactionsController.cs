
using System.Security.Claims;
using api.Application.Transactions;
using api.Application.Transactions.Interfaces;
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
    CreateTransferUseCase createTransferUseCase,
    ListTransactionsUseCase listTransactionsUseCase,
    ExportListTransactionsUseCase exportListTransactionsUseCase
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

        var response = await createTransferUseCase.Execute(new Guid(userIdClaim.Value), transferData);

        return Created(string.Empty, response);
    }

    [HttpGet]
    public async Task<IActionResult> ListTransactions([FromQuery] TransactionFindQueryDto query)
    {
        var userIdClaim = User.Claims.First(c => c.Type == ClaimTypes.UserData);
        var queryParams = new TransactionsListRequest
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            Skip = query.Skip,
            Take = query.Take,
            TransactionType = query.TransactionType,
            TransactionStatus = query.TransactionStatus
        };

        var response = await listTransactionsUseCase.Execute(new Guid(userIdClaim.Value), queryParams);

        return Ok(response);
    }

    [HttpGet]
    [Route("export")]
    public async Task<IActionResult> ExportListTransactions([FromQuery] TransactionFindQueryDto query)
    {
        var userIdClaim = User.Claims.First(c => c.Type == ClaimTypes.UserData);
        var queryParams = new TransactionsListRequest
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            TransactionType = query.TransactionType,
            TransactionStatus = query.TransactionStatus
        };

        var response = await exportListTransactionsUseCase.Execute(new Guid(userIdClaim.Value), queryParams);
        
        return File(response, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "transactions.xlsx");
        
    }
}

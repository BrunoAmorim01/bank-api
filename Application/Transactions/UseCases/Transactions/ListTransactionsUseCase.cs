using api.Application.Transactions.Interfaces;
using api.Domain.Enums;
using api.Domain.Repositories;
using Microsoft.OpenApi.Extensions;

namespace api.Application.Transactions;
public class ListTransactionsUseCase(
    ITransactionRepository transactionRepository
    )
{
    public async Task<object[]> Execute(Guid userId, TransactionsListRequest query)
    {
        var transactions = await transactionRepository.ListByUserId(userId, new FindQuery
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            Skip = query.Skip,
            Take = query.Take,
            TransactionType = query.TransactionType,
            TransactionStatus = query.TransactionStatus
        });

        var response = transactions.Select(x => new
        {
            x.Id,
            x.CreatedAt,
            amount = x.Value / 100,
            TransactionType = x.TransactionType.GetDisplayName().ToUpper(),
            TransactionStatus = x.TransactionStatus.GetDisplayName().ToUpper(),
            BankDestination = new
            {

                x.BankDestination.AccountDigit,
                x.BankDestination.AccountNumber
            },
            BankOrigin = new
            {
                x.BankOrigin.AccountDigit,
                x.BankOrigin.AccountNumber
            },
            UserDestination = new
            {
                x.UserDestination.Id,
                x.UserDestination.Name,
            },
            UserOrigin = new
            {
                x.UserOrigin.Id,
                x.UserOrigin.Name,
            }
        }).ToArray();

        return response;
    }

}

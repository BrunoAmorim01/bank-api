using api.Domain.Repositories;
using Microsoft.OpenApi.Extensions;

namespace api.Application.Transactions;

public class QueryParams
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
public class ListTransactionsUseCase(
    ITransactionRepository transactionRepository
    )
{
    public async Task<Object[]> Execute(Guid userId, QueryParams query)
    {
        var transactions = await transactionRepository.ListByUserId(userId, new FindQuery
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate
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
                x.UserDestination.Name,
            },
            UserOrigin = new
            {
                x.UserOrigin.Name,
            }
        }).ToArray();

        return response;
    }

}

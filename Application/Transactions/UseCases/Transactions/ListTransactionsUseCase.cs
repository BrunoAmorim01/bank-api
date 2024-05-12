using api.Domain.Repositories;

namespace api.Application.Transactions;

public class ListTransactionsUseCase(
    ITransactionRepository transactionRepository
    )
{
    public async Task<Object[]> Execute(Guid userId)
    {
        var transactions = await transactionRepository.ListByUserId(userId);

        var response = transactions.Select(x => new
        {
            x.Id,
            x.CreatedAt,
            amount = x.Value / 100,
            x.TransactionType,
            x.TransactionStatus,
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

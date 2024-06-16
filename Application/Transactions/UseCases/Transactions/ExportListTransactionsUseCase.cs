using api.Application.Transactions.Interfaces;
using api.Domain.Repositories;
using api.Domain.Services.Pdf;
using Microsoft.AspNetCore.Mvc;

namespace api.Application.Transactions;


public class ExportListTransactionsUseCase(
    ITransactionRepository transactionRepository,
    IPdfService pdfService
    )
{
    public async Task<byte[]> Execute(Guid userId, TransactionsListRequest query)
    {
        var transactions = await transactionRepository.ListByUserId(userId, new FindQuery
        {
            StartDate = query.StartDate,
            EndDate = query.EndDate,
            TransactionType = query.TransactionType,
            TransactionStatus = query.TransactionStatus
        });

        var response = transactions.Select(x => new ExportListTransactions
        {
            CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(x.CreatedAt, TimeZoneInfo.FindSystemTimeZoneById("Central Brazilian Standard Time")),
            TransactionType = x.TransactionType,
            TransactionStatus = x.TransactionStatus,
            Value = x.Value,
            FromUser = x.UserOrigin.Name,
            ToUser = x.UserDestination.Name
        }).ToArray();


        var pdfContent = pdfService.GeneratePdf(response);

        return pdfContent;
    }

}


﻿using api.Domain.Enums;

namespace api.Application.Transactions.Interfaces;

public class TransactionsListRequest
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
    public TransactionTypeEnum[]? TransactionType { get; set; }
    public TransactionStatusEnum[]? TransactionStatus { get; set; }

}

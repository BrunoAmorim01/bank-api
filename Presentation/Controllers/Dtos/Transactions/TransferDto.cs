namespace api.Presentation;

public class AccountDto
{
    public required string AccountDigit { get; set; }
    public required string AccountNumber { get; set; }
}
public class TransferDto
{
    public decimal Amount { get; set; }
    public string? EmailDestination { get; set; }
    public required AccountDto AccountDestination { get; set; }

}

namespace api.Presentation;

public class AccountDto
{
    public string? AccountDigit { get; set; }
    public string? AccountNumber { get; set; }
}
public class TransferDto
{
    public decimal Amount { get; set; }
    public string? EmailDestination { get; set; }
    public AccountDto? AccountDestination { get; set; }

}

namespace api.Application.Banks
{
    public class BankRegisterResponse
    {
        public Guid Id { get; set; }
        public required string AccountNumber { get; set; }
        public required string AccountDigit { get; set; }
    }
}

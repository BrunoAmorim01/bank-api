
using api.Domain.Enums;

namespace api.Domain.Services.Pdf;
public class ExportListTransactions
{
    public required DateTime CreatedAt { get; set; }
    public TransactionTypeEnum TransactionType { get; set; }
    public TransactionStatusEnum TransactionStatus { get; set; }
    public int Value { get; set; }
    public required string FromUser { get; set; }
    public required string ToUser { get; set; }
}
public interface IPdfService
{
     byte[] GeneratePdf(ExportListTransactions[] data);

}

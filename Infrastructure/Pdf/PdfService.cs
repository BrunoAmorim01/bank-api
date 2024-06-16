using api.Domain.Services.Pdf;
using ClosedXML.Excel;

namespace api.Infrastructure.Pdf;

public class PdfService : IPdfService
{
    public byte[] GeneratePdf(ExportListTransactions[] data)
    {

        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Transactions");
        var currentRow = 1;

        worksheet.Cell(currentRow, 1).Value = "Data";
        worksheet.Cell(currentRow, 2).Value = "Tipo de Transação";
        worksheet.Cell(currentRow, 3).Value = "Status";
        worksheet.Cell(currentRow, 4).Value = "Valor";
        worksheet.Cell(currentRow, 5).Value = "De";
        worksheet.Cell(currentRow, 6).Value = "Para";

        foreach (var item in data)
        {
            currentRow++;
            worksheet.Cell(currentRow, 1).Value = item.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
            worksheet.Cell(currentRow, 2).Value = item.TransactionType.ToString();
            worksheet.Cell(currentRow, 3).Value = item.TransactionStatus.ToString();
            worksheet.Cell(currentRow, 4).Value = (item.Value / 100).ToString("C2");            
            worksheet.Cell(currentRow, 5).Value = item.FromUser;
            worksheet.Cell(currentRow, 6).Value = item.ToUser;
        }

        var stream = new MemoryStream
        {
            Position = 0
        };
        workbook.SaveAs(stream);

        var streamArray = stream.ToArray();
                
        return streamArray;
    }
}



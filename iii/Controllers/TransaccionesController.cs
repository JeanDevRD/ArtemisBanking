using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enum;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly ICreditCardService _creditCardService;
        private readonly ILoanService _loanService;

        public TransactionsController(
            ITransactionService transactionService,
            ISavingsAccountService savingsAccountService,
            ICreditCardService creditCardService,
            ILoanService loanService)
        {
            _transactionService = transactionService;
            _savingsAccountService = savingsAccountService;
            _creditCardService = creditCardService;
            _loanService = loanService;
        }

        public async Task<IActionResult> Index(
            int? type,
            DateTime? from,
            DateTime? to,
            int? productId,
            string reference,
            int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pageNumber = page ?? 1;

            var transactions = await _transactionService.GetAllWithInclude();
            var accounts = await _savingsAccountService.GetAllAsync();
            var cards = await _creditCardService.GetAllAsync();
            var loans = await _loanService.GetAllAsync();

            var myProducts = accounts.Where(c => c.UserId == userId)
                .Select(c => new { c.Id, Name = c.AccountNumber, Type = "Account" })
                .Concat(cards.Where(t => t.UserId == userId)
                    .Select(t => new { t.Id, Name = $"Card ****{t.CardNumber.Substring(t.CardNumber.Length - 4)}", Type = "Card" }))
                .Concat(loans.Where(p => p.UserId == userId)
                    .Select(p => new { p.Id, Name = $"Loan {p.LoanNumber}", Type = "Loan" }))
                .ToList();

            var query = transactions.AsQueryable();

            if (type.HasValue)
                query = query.Where(t => t.Type == type.Value);

            if (from.HasValue)
                query = query.Where(t => t.Date >= from.Value);

            if (to.HasValue)
                query = query.Where(t => t.Date <= to.Value.AddDays(1).AddSeconds(-1));

            if (productId.HasValue)
            {
                var account = accounts.FirstOrDefault(c => c.Id == productId.Value);
                if (account != null)
                    query = query.Where(t => t.SavingsAccountId == productId.Value);
            }

            if (!string.IsNullOrEmpty(reference))
                query = query.Where(t => t.Id.ToString().Contains(reference) ||
                                       t.Source.Contains(reference) ||
                                       t.Beneficiary.Contains(reference) ||
                                       t.Amount.ToString("F2").Contains(reference));

            var result = query
                .Where(t => t.SavingsAccount != null && t.SavingsAccount.UserId == userId) // 
                .OrderByDescending(t => t.Date)
                .ToPagedList(pageNumber, 20);

            ViewBag.Type = type;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");
            ViewBag.ProductId = productId;
            ViewBag.Reference = reference;
            ViewBag.Products = myProducts;

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> ExportPdf(int? type, DateTime? from, DateTime? to, int? productId, string reference)
        {
            var transactions = await FilterTransactions(type, from, to, productId, reference);
            var pdfBytes = GeneratePdf(transactions);
            return File(pdfBytes, "application/pdf", $"Transactions_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel(int? type, DateTime? from, DateTime? to, int? productId, string reference)
        {
            var transactions = await FilterTransactions(type, from, to, productId, reference);
            var excelBytes = GenerateExcel(transactions);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       $"Transactions_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        private async Task<List<TransactionDto>> FilterTransactions(int? type, DateTime? from, DateTime? to, int? productId, string reference)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transactions = await _transactionService.GetAllWithInclude();
            var accounts = await _savingsAccountService.GetAllAsync();

            var query = transactions.AsQueryable();

            if (type.HasValue) query = query.Where(t => t.Type == type.Value);
            if (from.HasValue) query = query.Where(t => t.Date >= from.Value);
            if (to.HasValue) query = query.Where(t => t.Date <= to.Value.AddDays(1).AddSeconds(-1));
            if (productId.HasValue)
            {
                var account = accounts.FirstOrDefault(c => c.Id == productId.Value);
                if (account != null)
                    query = query.Where(t => t.SavingsAccountId == productId.Value);
            }
            if (!string.IsNullOrEmpty(reference))
                query = query.Where(t => t.Id.ToString().Contains(reference) ||
                                       t.Source.Contains(reference) ||
                                       t.Beneficiary.Contains(reference));

            return query.Where(t => t.SavingsAccount != null && t.SavingsAccount.UserId == userId)
                        .OrderByDescending(t => t.Date)
                        .ToList();
        }

        private byte[] GeneratePdf(List<TransactionDto> transactions)
        {
            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            document.Add(new Paragraph("Transaction History")
                .SetFontSize(16)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

            document.Add(new Paragraph($"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFontSize(10).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

            var table = new Table(7).UseAllAvailableWidth();
            table.AddHeaderCell("Date");
            table.AddHeaderCell("Type");
            table.AddHeaderCell("Source");
            table.AddHeaderCell("Destination");
            table.AddHeaderCell("Amount");
            table.AddHeaderCell("Status");
            table.AddHeaderCell("Reference");

            foreach (var t in transactions)
            {
                table.AddCell(t.Date.ToString("dd/MM/yyyy HH:mm"));
                table.AddCell(((TypeTransaction)t.Type).ToString());
                table.AddCell(t.Source);
                table.AddCell(t.Beneficiary);
                table.AddCell(t.Amount.ToString("C2"));
                table.AddCell(((StatusTransaction)t.Status).ToString());
                table.AddCell(t.Id.ToString());
            }

            document.Add(table);
            document.Close();
            return memoryStream.ToArray();
        }

        private byte[] GenerateExcel(List<TransactionDto> transactions)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Transactions");

            sheet.Cells[1, 1].Value = "Transaction History";
            sheet.Cells[1, 1, 1, 7].Merge = true;
            sheet.Cells[1, 1].Style.Font.Bold = true;
            sheet.Cells[1, 1].Style.Font.Size = 16;

            sheet.Cells[2, 1].Value = $"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}";
            sheet.Cells[2, 1, 2, 7].Merge = true;

            var headers = new[] { "Date", "Type", "Source", "Destination", "Amount", "Status", "Reference" };
            for (int i = 0; i < headers.Length; i++)
                sheet.Cells[4, i + 1].Value = headers[i];

            sheet.Cells[4, 1, 4, 7].Style.Font.Bold = true;

            int row = 5;
            foreach (var t in transactions)
            {
                sheet.Cells[row, 1].Value = t.Date.ToString("dd/MM/yyyy HH:mm");
                sheet.Cells[row, 2].Value = ((TypeTransaction)t.Type).ToString();
                sheet.Cells[row, 3].Value = t.Source;
                sheet.Cells[row, 4].Value = t.Beneficiary;
                sheet.Cells[row, 5].Value = t.Amount;
                sheet.Cells[row, 5].Style.Numberformat.Format = "$#,##0.00";
                sheet.Cells[row, 6].Value = ((StatusTransaction)t.Status).ToString();
                sheet.Cells[row, 7].Value = t.Id;
                row++;
            }

            sheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }
    }
}

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
    public class TransaccionesController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly ICreditCardService _creditCardService;
        private readonly ILoanService _loanService;

        public TransaccionesController(
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
            int? tipo,
            DateTime? desde,
            DateTime? hasta,
            int? productoId,
            string referencia,
            int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pageNumber = page ?? 1;

            var transacciones = await _transactionService.GetAllWithInclude();
            var cuentas = await _savingsAccountService.GetAllAsync();
            var tarjetas = await _creditCardService.GetAllAsync();
            var prestamos = await _loanService.GetAllAsync();

            var misProductos = cuentas.Where(c => c.UserId == userId)
                .Select(c => new { c.Id, Name = c.AccountNumber, Type = "Cuenta" })
                .Concat(tarjetas.Where(t => t.UserId == userId)
                    .Select(t => new { t.Id, Name = $"Tarjeta ****{t.CardNumber.Substring(t.CardNumber.Length - 4)}", Type = "Tarjeta" }))
                .Concat(prestamos.Where(p => p.UserId == userId)
                    .Select(p => new { p.Id, Name = $"Préstamo {p.LoanNumber}", Type = "Préstamo" }))
                .ToList();

            var query = transacciones.AsQueryable();

            if (tipo.HasValue)
                query = query.Where(t => t.Type == tipo.Value);

            if (desde.HasValue)
                query = query.Where(t => t.Date >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(t => t.Date <= hasta.Value.AddDays(1).AddSeconds(-1));

            if (productoId.HasValue)
            {
                var cuenta = cuentas.FirstOrDefault(c => c.Id == productoId.Value);
                if (cuenta != null)
                    query = query.Where(t => t.SavingsAccountId == productoId.Value);
            }

            if (!string.IsNullOrEmpty(referencia))
                query = query.Where(t => t.Id.ToString().Contains(referencia) ||
                                       t.Source.Contains(referencia) ||
                                       t.Beneficiary.Contains(referencia) ||
                                       t.Amount.ToString("F2").Contains(referencia));

            var resultado = query
                .Where(t => t.SavingsAccount != null && t.SavingsAccount.UserId == userId) // 
                .OrderByDescending(t => t.Date)
                .ToPagedList(pageNumber, 20);

            ViewBag.Tipo = tipo;
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
            ViewBag.ProductoId = productoId;
            ViewBag.Referencia = referencia;
            ViewBag.Productos = misProductos;

            return View(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> ExportarPdf(int? tipo, DateTime? desde, DateTime? hasta, int? productoId, string referencia)
        {
            var transacciones = await FiltrarTransacciones(tipo, desde, hasta, productoId, referencia);
            var pdfBytes = GenerarPdf(transacciones);
            return File(pdfBytes, "application/pdf", $"Transacciones_{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> ExportarExcel(int? tipo, DateTime? desde, DateTime? hasta, int? productoId, string referencia)
        {
            var transacciones = await FiltrarTransacciones(tipo, desde, hasta, productoId, referencia);
            var excelBytes = GenerarExcel(transacciones);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       $"Transacciones_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        private async Task<List<TransactionDto>> FiltrarTransacciones(int? tipo, DateTime? desde, DateTime? hasta, int? productoId, string referencia)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transacciones = await _transactionService.GetAllWithInclude();
            var cuentas = await _savingsAccountService.GetAllAsync();

            var query = transacciones.AsQueryable();

            if (tipo.HasValue) query = query.Where(t => t.Type == tipo.Value);
            if (desde.HasValue) query = query.Where(t => t.Date >= desde.Value);
            if (hasta.HasValue) query = query.Where(t => t.Date <= hasta.Value.AddDays(1).AddSeconds(-1));
            if (productoId.HasValue)
            {
                var cuenta = cuentas.FirstOrDefault(c => c.Id == productoId.Value);
                if (cuenta != null)
                    query = query.Where(t => t.SavingsAccountId == productoId.Value);
            }
            if (!string.IsNullOrEmpty(referencia))
                query = query.Where(t => t.Id.ToString().Contains(referencia) ||
                                       t.Source.Contains(referencia) ||
                                       t.Beneficiary.Contains(referencia));

            return query.Where(t => t.SavingsAccount != null && t.SavingsAccount.UserId == userId)
                        .OrderByDescending(t => t.Date)
                        .ToList();
        }

        private byte[] GenerarPdf(List<TransactionDto> transacciones)
        {
            using var memoryStream = new MemoryStream();
            var writer = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            document.Add(new Paragraph("Historial de Transacciones")
                .SetFontSize(16)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

            document.Add(new Paragraph($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFontSize(10).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

            var table = new Table(7).UseAllAvailableWidth();
            table.AddHeaderCell("Fecha");
            table.AddHeaderCell("Tipo");
            table.AddHeaderCell("Origen");
            table.AddHeaderCell("Destino");
            table.AddHeaderCell("Monto");
            table.AddHeaderCell("Estado");
            table.AddHeaderCell("Referencia");

            foreach (var t in transacciones)
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

        private byte[] GenerarExcel(List<TransactionDto> transacciones)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Transacciones");

            sheet.Cells[1, 1].Value = "Historial de Transacciones";
            sheet.Cells[1, 1, 1, 7].Merge = true;
            sheet.Cells[1, 1].Style.Font.Bold = true;
            sheet.Cells[1, 1].Style.Font.Size = 16;

            sheet.Cells[2, 1].Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            sheet.Cells[2, 1, 2, 7].Merge = true;

            var headers = new[] { "Fecha", "Tipo", "Origen", "Destino", "Monto", "Estado", "Referencia" };
            for (int i = 0; i < headers.Length; i++)
                sheet.Cells[4, i + 1].Value = headers[i];

            sheet.Cells[4, 1, 4, 7].Style.Font.Bold = true;

            int row = 5;
            foreach (var t in transacciones)
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
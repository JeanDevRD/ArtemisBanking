using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Globalization;
using System.IO;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class TransaccionesController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransaccionesController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index(string filtro = "", string tipo = "", DateTime? desde = null, DateTime? hasta = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var transacciones = await _transactionService.GetAllAsync();

            // Filtrar por usuario
            var lista = transacciones
                .Where(t => t.SavingsAccount?.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToList();

            // Filtros
            if (!string.IsNullOrEmpty(filtro))
                lista = lista.Where(t =>
                    t.Beneficiary.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                    t.Amount.ToString().Contains(filtro) ||
                    t.Source.Contains(filtro, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (!string.IsNullOrEmpty(tipo))
                lista = lista.Where(t => t.Type.ToString() == tipo).ToList();

            if (desde.HasValue)
                lista = lista.Where(t => t.Date >= desde.Value).ToList();

            if (hasta.HasValue)
                lista = lista.Where(t => t.Date <= hasta.Value).ToList();

            ViewBag.Filtro = filtro;
            ViewBag.Tipo = tipo;
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");

            return View(lista.Take(20).ToList());
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var transaccion = await _transactionService.GetByIdAsync(id);
            if (transaccion == null)
                return NotFound();

            return PartialView("_Detalle", transaccion);
        }

        [HttpGet]
        public async Task<IActionResult> ExportarPDF(string filtro = "", string tipo = "", DateTime? desde = null, DateTime? hasta = null)
        {
            var transacciones = await _transactionService.GetAllAsync();
            var lista = Filtrar(transacciones, filtro, tipo, desde, hasta);

            using (var stream = new MemoryStream())
            {
                Document pdf = new Document(PageSize.A4);
                PdfWriter.GetInstance(pdf, stream);
                pdf.Open();

                var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var fontHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var fontRow = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                pdf.Add(new Paragraph("Historial de Transacciones", fontTitle));
                pdf.Add(new Paragraph($"Generado el {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}\n\n"));

                PdfPTable table = new PdfPTable(6);
                table.AddCell(new Phrase("Fecha", fontHeader));
                table.AddCell(new Phrase("Tipo", fontHeader));
                table.AddCell(new Phrase("Origen", fontHeader));
                table.AddCell(new Phrase("Destino", fontHeader));
                table.AddCell(new Phrase("Monto", fontHeader));
                table.AddCell(new Phrase("Estado", fontHeader));

                foreach (var t in lista)
                {
                    table.AddCell(new Phrase(t.Date.ToShortDateString(), fontRow));
                    table.AddCell(new Phrase(t.Type.ToString(), fontRow));
                    table.AddCell(new Phrase(t.Source, fontRow));
                    table.AddCell(new Phrase(t.Beneficiary, fontRow));
                    table.AddCell(new Phrase(t.Amount.ToString("C2"), fontRow));
                    table.AddCell(new Phrase(t.Status.ToString(), fontRow));
                }

                pdf.Add(table);
                pdf.Close();

                return File(stream.ToArray(), "application/pdf", "Transacciones.pdf");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcel(string filtro = "", string tipo = "", DateTime? desde = null, DateTime? hasta = null)
        {
            var transacciones = await _transactionService.GetAllAsync();
            var lista = Filtrar(transacciones, filtro, tipo, desde, hasta);

            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("Transacciones");
                sheet.Cells[1, 1].Value = "Fecha";
                sheet.Cells[1, 2].Value = "Tipo";
                sheet.Cells[1, 3].Value = "Origen";
                sheet.Cells[1, 4].Value = "Destino";
                sheet.Cells[1, 5].Value = "Monto";
                sheet.Cells[1, 6].Value = "Estado";

                int row = 2;
                foreach (var t in lista)
                {
                    sheet.Cells[row, 1].Value = t.Date.ToShortDateString();
                    sheet.Cells[row, 2].Value = t.Type.ToString();
                    sheet.Cells[row, 3].Value = t.Source;
                    sheet.Cells[row, 4].Value = t.Beneficiary;
                    sheet.Cells[row, 5].Value = t.Amount;
                    sheet.Cells[row, 6].Value = t.Status;
                    row++;
                }

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Transacciones.xlsx");
            }
        }

        private List<TransactionDto> Filtrar(List<TransactionDto> lista, string filtro, string tipo, DateTime? desde, DateTime? hasta)
        {
            if (!string.IsNullOrEmpty(filtro))
                lista = lista.Where(t =>
                    t.Beneficiary.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                    t.Amount.ToString().Contains(filtro) ||
                    t.Source.Contains(filtro, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            if (!string.IsNullOrEmpty(tipo))
                lista = lista.Where(t => t.Type.ToString() == tipo).ToList();

            if (desde.HasValue)
                lista = lista.Where(t => t.Date >= desde.Value).ToList();

            if (hasta.HasValue)
                lista = lista.Where(t => t.Date <= hasta.Value).ToList();

            return lista;
        }
    }
}


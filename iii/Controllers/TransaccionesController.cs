using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Transacciones;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Reflection.Metadata;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class TransaccionesController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransaccionesController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index(string? tipo, string? producto, DateTime? desde, DateTime? hasta, string? buscar, int page = 1)
        {
            var lista = await _transactionService.GetAllAsync();
            var query = lista.AsQueryable();

            if (!string.IsNullOrEmpty(tipo))
                query = query.Where(t => t.Type.ToString() == tipo);

            if (desde.HasValue)
                query = query.Where(t => t.Date >= desde.Value);
            if (hasta.HasValue)
                query = query.Where(t => t.Date <= hasta.Value);

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(t =>
                    t.Source.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    t.Beneficiary.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    t.Amount.ToString().Contains(buscar));

            const int pageSize = 20;
            var paginated = query
                .OrderByDescending(t => t.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new TransaccionFiltroViewModel
            {
                Transacciones = paginated,
                Tipo = tipo,
                Producto = producto,
                FechaDesde = desde,
                FechaHasta = hasta,
                Buscar = buscar
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ExportarExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var lista = await _transactionService.GetAllAsync();
            using var package = new ExcelPackage();
            var hoja = package.Workbook.Worksheets.Add("Transacciones");

            hoja.Cells[1, 1].Value = "Fecha";
            hoja.Cells[1, 2].Value = "Tipo";
            hoja.Cells[1, 3].Value = "Origen";
            hoja.Cells[1, 4].Value = "Beneficiario";
            hoja.Cells[1, 5].Value = "Monto";
            hoja.Cells[1, 6].Value = "Estado";

            int fila = 2;
            foreach (var t in lista)
            {
                hoja.Cells[fila, 1].Value = t.Date.ToString("dd/MM/yyyy");
                hoja.Cells[fila, 2].Value = t.Type;
                hoja.Cells[fila, 3].Value = t.Source;
                hoja.Cells[fila, 4].Value = t.Beneficiary;
                hoja.Cells[fila, 5].Value = t.Amount;
                hoja.Cells[fila, 6].Value = t.Status;
                fila++;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Transacciones.xlsx");
        }
        [HttpGet]
        public async Task<IActionResult> ExportarPdf()
        {
            var lista = await _transactionService.GetAllAsync();
            using var stream = new MemoryStream();

            var documento = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 25, 25);
            PdfWriter.GetInstance(documento, stream);
            documento.Open();

            // ✅ Crear fuente correctamente con FontFactory
            var fuenteTitulo = iTextSharp.text.FontFactory.GetFont("Helvetica", 16, iTextSharp.text.Font.BOLD);
            var fuenteNormal = iTextSharp.text.FontFactory.GetFont("Helvetica", 11, iTextSharp.text.Font.NORMAL);

            var titulo = new iTextSharp.text.Paragraph("Historial de Transacciones", fuenteTitulo)
            {
                Alignment = iTextSharp.text.Element.ALIGN_CENTER
            };
            documento.Add(titulo);
            documento.Add(new iTextSharp.text.Paragraph(" ", fuenteNormal));

            var tabla = new iTextSharp.text.pdf.PdfPTable(5)
            {
                WidthPercentage = 100
            };

            // Encabezados
            tabla.AddCell(new iTextSharp.text.Phrase("Fecha", fuenteNormal));
            tabla.AddCell(new iTextSharp.text.Phrase("Tipo", fuenteNormal));
            tabla.AddCell(new iTextSharp.text.Phrase("Origen", fuenteNormal));
            tabla.AddCell(new iTextSharp.text.Phrase("Beneficiario", fuenteNormal));
            tabla.AddCell(new iTextSharp.text.Phrase("Monto", fuenteNormal));

            // Filas
            foreach (var t in lista)
            {
                tabla.AddCell(new iTextSharp.text.Phrase(t.Date.ToString("dd/MM/yyyy"), fuenteNormal));
                tabla.AddCell(new iTextSharp.text.Phrase(t.Type.ToString(), fuenteNormal));
                tabla.AddCell(new iTextSharp.text.Phrase(t.Source, fuenteNormal));
                tabla.AddCell(new iTextSharp.text.Phrase(t.Beneficiary, fuenteNormal));
                tabla.AddCell(new iTextSharp.text.Phrase(t.Amount.ToString("C"), fuenteNormal));
            }

            documento.Add(tabla);
            documento.Close();

            stream.Position = 0;
            return File(stream.ToArray(), "application/pdf", "Transacciones.pdf");
        }


    }
}

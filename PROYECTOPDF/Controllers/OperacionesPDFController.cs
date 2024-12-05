// PROYECTOPDF/Controllers/OperacionesPDFController.cs
using Microsoft.AspNetCore.Mvc;
using NegocioPDF.Repositories;
using System.Security.Claims;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;

namespace PROYECTOPDF.Controllers
{
    public class OperacionesPDFController : Controller
    {
        private readonly OperacionesPDFRepository _operacionesPDFRepository;

        public OperacionesPDFController(OperacionesPDFRepository operacionesPDFRepository)
        {
            _operacionesPDFRepository = operacionesPDFRepository;
        }

        [HttpGet]
        public IActionResult Operaciones()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FusionarPDF()
        {
            return View();
        }
         [HttpGet]
        public IActionResult CortarPDF()
        {
            return View();
        }

        
[HttpPost]
public IActionResult FusionarArchivosPDF(IFormFile archivo1, IFormFile archivo2)
{
    var usuarioId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

    // Validar si se puede realizar la operación
    var puedeRealizarOperacion = _operacionesPDFRepository.RegistrarOperacionPDF(usuarioId, "Fusionar");
    if (!puedeRealizarOperacion)
    {
        TempData["Error"] = "No se puede fusionar: ¡Has excedido el límite de operaciones para tu suscripción básica!";
        return RedirectToAction("FusionarPDF");
    }

    // Proceder con la fusión de PDF
    if (archivo1 == null || archivo2 == null || archivo1.Length == 0 || archivo2.Length == 0)
    {
        TempData["Error"] = "Debe seleccionar dos archivos PDF válidos.";
        return RedirectToAction("FusionarPDF");
    }

    try
    {
        var rutaArchivoFusionado = Path.Combine(Path.GetTempPath(), "PDFFusionado.pdf");

        using (var pdf1Stream = archivo1.OpenReadStream())
        using (var pdf2Stream = archivo2.OpenReadStream())
        {
            var outputDocument = new PdfDocument();
            var inputDocument1 = PdfReader.Open(pdf1Stream, PdfDocumentOpenMode.Import);
            CopyPages(inputDocument1, outputDocument);

            var inputDocument2 = PdfReader.Open(pdf2Stream, PdfDocumentOpenMode.Import);
            CopyPages(inputDocument2, outputDocument);

            outputDocument.Save(rutaArchivoFusionado);
        }

        var fileBytes = System.IO.File.ReadAllBytes(rutaArchivoFusionado);
        return File(fileBytes, "application/pdf", "PDFFusionado.pdf");
    }
    catch (Exception ex)
    {
        TempData["Error"] = $"Error al fusionar los archivos PDF: {ex.Message}";
        return RedirectToAction("FusionarPDF");
    }
}
        private void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }

       

        
[HttpPost]
public IActionResult CortarArchivoPDF(string rutaArchivoTemp, int startPage, int endPage)
{
    // Obtener el ID del usuario logueado
    var usuarioId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

    // Validar si el usuario puede realizar la operación
    var puedeRealizarOperacion = _operacionesPDFRepository.RegistrarOperacionPDF(usuarioId, "Cortar");
    if (!puedeRealizarOperacion)
    {
        TempData["Error"] = "No se puede cortar: ¡Has excedido el límite de operaciones para tu suscripción básica!";
        return RedirectToAction("CortarPDF");
    }

    if (string.IsNullOrEmpty(rutaArchivoTemp) || !System.IO.File.Exists(rutaArchivoTemp))
    {
        TempData["Error"] = "Debe seleccionar un archivo PDF válido.";
        return RedirectToAction("CortarPDF");
    }

    try
    {
        // Crear un nombre temporal para el archivo cortado
        var rutaArchivoCortado = Path.Combine(Path.GetTempPath(), "PDFCortado.pdf");

        var inputDocument = PdfReader.Open(rutaArchivoTemp, PdfDocumentOpenMode.Import);
        var outputDocument = new PdfDocument();

        // Validar que el rango de páginas sea válido
        if (startPage < 1 || endPage > inputDocument.PageCount || startPage > endPage)
        {
            TempData["Error"] = "El rango de páginas es inválido.";
            return RedirectToAction("CortarPDF");
        }

        // Copiar las páginas seleccionadas al nuevo documento
        for (int pageIndex = startPage; pageIndex <= endPage; pageIndex++)
        {
            outputDocument.AddPage(inputDocument.Pages[pageIndex - 1]);
        }

        // Guardar el archivo cortado
        outputDocument.Save(rutaArchivoCortado);

        // Descargar el archivo cortado
        var fileBytes = System.IO.File.ReadAllBytes(rutaArchivoCortado);
        return File(fileBytes, "application/pdf", "PDFCortado.pdf");
    }
    catch (Exception ex)
    {
        TempData["Error"] = $"Error al cortar el archivo PDF: {ex.Message}";
        return RedirectToAction("CortarPDF");
    }
}
[HttpPost]
public IActionResult ObtenerTotalPaginas(IFormFile pdfFile)
{
    if (pdfFile == null || pdfFile.Length == 0)
    {
        return Json(new { success = false, error = "Debe seleccionar un archivo PDF válido." });
    }

    try
    {
        using (var pdfStream = pdfFile.OpenReadStream())
        {
            var inputDocument = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Import);
            var totalPages = inputDocument.PageCount;

            return Json(new { success = true, totalPaginas = totalPages });
        }
    }
    catch (Exception ex)
    {
        return Json(new { success = false, error = $"Error al obtener el número de páginas: {ex.Message}" });
    }
}
[HttpPost]
public IActionResult CargarArchivoTemporal(IFormFile pdfFile)
{
    if (pdfFile == null || pdfFile.Length == 0)
    {
        return Json(new { success = false, error = "Debe seleccionar un archivo PDF válido." });
    }

    try
    {
        // Crear un nombre temporal para el archivo
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.pdf");

        // Guardar el archivo temporalmente
        using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            pdfFile.CopyTo(stream);
        }

        // Obtener el número de páginas
        var inputDocument = PdfReader.Open(tempFilePath, PdfDocumentOpenMode.Import);
        var totalPages = inputDocument.PageCount;

        return Json(new { success = true, totalPaginas = totalPages, rutaArchivoTemp = tempFilePath });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, error = $"Error al cargar el archivo: {ex.Message}" });
    }
}
    }
}
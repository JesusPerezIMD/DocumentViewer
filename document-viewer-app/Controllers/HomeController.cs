﻿using document_viewer_app.Models;
using document_viewer_app.Reports;
using document_viewer_app.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using static document_viewer_app.Services.DocumentViewer;

namespace document_viewer_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DocumentViewerApiService _documentViewerApiService;

        public HomeController(DocumentViewerApiService documentViewerApiService, ILogger<HomeController> logger)
        {
            _documentViewerApiService = documentViewerApiService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            string secretKey = HttpContext.Request.Query["k"];

            if (!string.IsNullOrEmpty(secretKey))
            {
                return VerDocumento(secretKey).GetAwaiter().GetResult();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerDocumento(string secretKey)
        {
            try
            {
                var response = await _documentViewerApiService.Auth(new AuthRequest { secretKey = secretKey });
                if (response.status == "OK")
                {
                    string extension = Path.GetExtension(response.fileUrl);

                    if (extension == ".docx")
                    {
                        string encryptedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(response.fileUrl));
                        return RedirectToAction("RichEdit", new { nombreArchivo = encryptedValue });
                    }
                    else if (extension == ".pdf")
                    {
                        string encryptedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(response.fileUrl));
                        return RedirectToAction("Reporting", new { nombreArchivo = encryptedValue });
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return Content($"Error al obtener el documento: {response.message}");
                }
            }
            catch (Exception ex)
            {
                return Content($"Error al obtener el documento: {ex.Message}");
            }
        }

        public IActionResult Reporting(string nombreArchivo = "") //dotnet_core_tutorial.pdf
        {
            if (string.IsNullOrEmpty(nombreArchivo))
            {
                return Content("Error: no se especificó ningún archivo.");
            }

            string decryptedValue = Encoding.UTF8.GetString(Convert.FromBase64String(nombreArchivo));
            string urlCompleta = decryptedValue;
            var report = new TestReport(urlCompleta);

            return View(report); // Pasar el objeto TestReport creado como modelo de la vista
        }

        public async Task<IActionResult> RichEdit(string nombreArchivo = "") //Documento.docx
        {
            if (string.IsNullOrEmpty(nombreArchivo))
            {
                return Content("Error: no se especificó ningún archivo.");
            }

            string decryptedValue = Encoding.UTF8.GetString(Convert.FromBase64String(nombreArchivo));
            string fileUrl = decryptedValue;

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(fileUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    return View(content);
                }
                else
                {
                    return Content("Error al descargar el archivo");
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
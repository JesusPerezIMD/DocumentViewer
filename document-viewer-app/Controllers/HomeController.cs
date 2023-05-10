using document_viewer_app.Models;
using document_viewer_app.Reports;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace document_viewer_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index([FromQuery] string nombreArchivo = "")
        {
            if (string.IsNullOrEmpty(nombreArchivo))
            {
                return View();
            }

            string extension = Path.GetExtension(nombreArchivo);

            if (extension == ".docx")
            {
                string encryptedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(nombreArchivo));
                return RedirectToAction("RichEdit", new { nombreArchivo = encryptedValue });
            }
            else if (extension == ".pdf")
            {
                string encryptedValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(nombreArchivo));
                return RedirectToAction("Reporting", new { nombreArchivo = encryptedValue });
            }
            else
            {
				return RedirectToAction("ErrorSite");
			}
        }

		public IActionResult ErrorSite()
		{
			return View();
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
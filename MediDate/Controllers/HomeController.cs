using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;

namespace MediDate.Controllers
{
    public class HomeController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Index()
        {
            
            return View(_database.Medicos.GetAll());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string txtBuscar)
        {
            
            if (String.IsNullOrEmpty(txtBuscar))
            {

                return View(_database.Medicos.GetAll());
            }
            else
            {
                var result = _database.Medicos.BusquedaSuccess(txtBuscar);

                if (!result.Success)
                {
                    TempData["AlertMessage"] = result.Message;
                    return View(_database.Medicos.GetAll());
                }
                else
                {
                    return View(_database.Medicos.Busqueda(txtBuscar));
                }
            }

        }

        /// <summary>
        /// Enlista las Especialidades y Servicios para la lista desplegable del buscador
        /// </summary>
        /// <param name="txtBuscar"></param>
        /// <returns>Lista de Especialidades y Servicios</returns>
        [HttpGet]
        public JsonResult GetDropdownData(string txtBuscar)
        {
            var especialidades = _database.Especialidades.GetDescripciones(txtBuscar);
            var servicios = _database.Servicios.GetDescripciones(txtBuscar);

            var especialidadStrings = especialidades.Select(e => e.Descripcion).ToList();
            var servicioStrings = servicios.Select(s => s.Descripcion).ToList();

            var dropdownData = new List<string>();
            dropdownData.AddRange(especialidadStrings);
            dropdownData.AddRange(servicioStrings);

            return Json(dropdownData);
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
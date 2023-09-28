using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
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
            ViewBag.Busqueda = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
            ViewBag.Busqueda2 = new SelectList(_database.Medicos.GetAll(), "IdMedico", "NombreCompleto");

            return View(_database.Medicos.GetAll());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string txtBuscar)
        {
            ViewBag.Busqueda = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
           
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
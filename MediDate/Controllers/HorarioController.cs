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
    public class HorarioController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<HorarioController> _logger;

        public HorarioController(ILogger<HorarioController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Horario horario)
        {
            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                int IdMedico = Int32.Parse(strMedico);
                horario.IdMedico = IdMedico;

                //Agregamos el horario
                var result = _database.Horarios.Create(horario);

                if (!result.Success)
                {

                    TempData["AlertMessage"] = "Error al registrar el horario. Intente de nuevo. ";
                    return View(horario);
                }

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("IndexMedico", "Cita");
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                return RedirectToAction("Index", "Medico");
            }

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
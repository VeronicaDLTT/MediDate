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

        public IActionResult Edit(int IdHorario)
        {
            //Buscamos el Horario por su Id
            var horario = _database.Horarios.GetById(IdHorario);

            if (horario == null)
            {
                return NotFound();
            }
            else
            {
                return View(horario);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Horario horario)
        {
            //Actualizamos el Horario
            var result = _database.Horarios.Edit(horario);

            if (!result.Success)
            {

                TempData["AlertMessage"] = "Error al modificar el horario. Intente de nuevo. ";
                return View(horario);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Details", "Horario");
        }

        public IActionResult Details()
        {
            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                int IdMedico = Int32.Parse(strMedico);
                
                //Buscamos el horario por IdMedico
                var horario = _database.Horarios.GetByIdMedico(IdMedico);

                if (horario == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(horario);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cargar el horario de atención. ";
                return RedirectToAction("IndexMedico", "Cita");
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
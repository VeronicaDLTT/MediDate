using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;

namespace MediDate.Controllers
{
    public class ConsultorioController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<ConsultorioController> _logger;

        public ConsultorioController(ILogger<ConsultorioController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Index()
        {
           return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Consultorio consultorio)
        {

            try
            {

                //Obtenemos los datos temporales de MedicoController
                Medico medico = new Medico();

                string Email = TempData["Email"] as string;
                string Pass = TempData["Pass"] as string;

                medico.Nombre = TempData["Nombre"] as string;
                medico.PrimerApellido = TempData["PrimerApellido"] as string;
                medico.SegundoApellido = TempData["SegundoApellido"] as string;
                medico.IdEspecialidad = (int)TempData["IdEspecialidad"];
                medico.NumCedula = TempData["NumCedula"] as string;
                medico.Telefono = (int)TempData["Telefono"];


                //Creamos el Usuario Medico
                var result = _database.Usuarios.CreateMedicos(Email, Pass, medico, consultorio);

                if (!result.Success)
                {
                    TempData["AlertMessage"] = "No fue posible guardar la información del Consultorio";
                    return View(consultorio);
                }
                else
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index", "Home");
                }

                ////Obtenemos el ultimo IdMedico que se creó
                //var medico = _database.Medicos.GetLastUser();

                //consultorio.IdMedico = medico.IdMedico;

                ////Crear Consultorio en la tabla Consultorios
                //var result = _database.Consultorios.Create(consultorio);

            }
            catch (Exception e)
            {
                //Console.WriteLine("Excepción: " + e.Message);
                TempData["ErrorMessage"] = "No fue posible guardar la información del Consultorio" + e.Message;
                return RedirectToAction("Index", "Home");
            }
            
        }
    }
}
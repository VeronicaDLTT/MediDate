using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;

namespace MediDate.Controllers
{
    public class ConsultorioController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<ConsultorioController> _logger;
        //Variables para almacenar los datos de MedicoController
        public Medico medico1 = new Medico();
        public string Email1;
        public string Pass1;
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
                Email1 = TempData["Email"] as string;
                Pass1 = TempData["Pass"] as string;

                medico1.Nombre = TempData["Nombre"] as string;
                medico1.PrimerApellido = TempData["PrimerApellido"] as string;
                medico1.SegundoApellido = TempData["SegundoApellido"] as string;
                medico1.IdEspecialidad = (int)TempData["IdEspecialidad"];
                medico1.NumCedula = TempData["NumCedula"] as string;
                medico1.Telefono = TempData["Telefono"] as string;


                //Creamos el Usuario Medico
                var result = _database.Usuarios.CreateMedicos(Email1, Pass1, medico1, consultorio);

                if (!result.Success)
                {
                    TempData["AlertMessage"] = "No fue posible guardar la información del Usuario.";
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
                TempData["ErrorMessage"] = "No fue posible guardar la información del Usuario. " + e.Message;
                return RedirectToAction("Index", "Home");
            }
            
        }
    }
}
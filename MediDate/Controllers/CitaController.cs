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
    public class CitaController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<CitaController> _logger;

        public CitaController(ILogger<CitaController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult IndexMedico()
        {
            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                int IdMedico = Int32.Parse(strMedico);

                return View(_database.Citas.GetAllByMedico(IdMedico));
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult IndexPaciente()
        {
            if (Request.Cookies.TryGetValue("IdPaciente", out string strPaciente))
            {
                int IdPaciente = Int32.Parse(strPaciente);

                return View(_database.Citas.GetAllByPaciente(IdPaciente));
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                return RedirectToAction("Index", "Paciente");
            }

        }

        public IActionResult CreatePaciente()
        {
            //Verificamos si hay un Paciente que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdPaciente", out string strIdPaciente))
            {
                //Verificamos si existe un valor en IdMedico
                if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
                {
                    int IdPaciente = Int32.Parse(strIdPaciente);
                    int IdMedico = Int32.Parse(strIdMedico);

                    //Buscamos los datos del Paciente y del Medico
                    var paciente = _database.Pacientes.GetById(IdPaciente);
                    var medico = _database.Medicos.GetById(IdMedico);

                    Cita cita = new Cita();
                    cita.NombreMedico = medico.NombreCompleto;
                    cita.NombrePaciente = paciente.NombreCompleto;

                    return View(cita);
                }
                else
                {
                    return RedirectToAction("Index", "Paciente");
                }
                    
            }
            else
            {
                
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePaciente(Cita cita)
        {
            //Verificamos si hay un Paciente que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdPaciente", out string strIdPaciente))
            {
                //Verificamos si existe un valor en IdMedico
                if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
                {
                    int IdPaciente = Int32.Parse(strIdPaciente);
                    int IdMedico = Int32.Parse(strIdMedico);

                    cita.IdPaciente = IdPaciente;
                    cita.IdMedico = IdMedico;
                    cita.Estado = 1;

                    //Creamos la cita
                    var result = _database.Citas.Create(cita);

                    //Si no se pudo crear la cita
                    if (!result.Success)
                    {
                        TempData["ErrorMessage"] = "Error al agendar la cita.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = result.Message;
                        
                    }
                    return RedirectToAction("Index", "Paciente");
                }
                else
                {
                    return RedirectToAction("Index", "Paciente");
                }

            }
            else
            {

                return RedirectToAction("Index", "Home");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
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
    public class PacienteController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<PacienteController> _logger;

        public PacienteController(ILogger<PacienteController> logger)
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
        public IActionResult Create(Paciente paciente, string Email, string Pass)
        {

            try
            {

                //Verificar si el correo ingresado ya existe
                var resultEmail = _database.Usuarios.VerificarEmail(Email);

                //Si no existe el correo
                if (!resultEmail.Success)
                {
                    //Creamos el Usuario Paciente
                    var result = _database.Usuarios.CreatePacientes(Email, Pass, paciente);

                    if (!result.Success)
                    {
                        TempData["AlertMessage"] = "No fue posible crear el Usuario.";
                        return View(paciente);
                    }
                    else
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    //Si el correo si existe
                    TempData["AlertMessage"] = resultEmail.Message;
                    return View(paciente);
                }

            }
            catch (Exception e)
            {
                //Console.WriteLine("Excepción: " + e.Message);
                TempData["ErrorMessage"] = "No fue posible crear el Usuario. " + e.Message;
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult Details()
        {
            //Guardamos el dato de Email
            if (Request.Cookies.TryGetValue("Email", out string strEmail))
            {
                ViewBag.Email = strEmail;
            }

            //Buscamos la información por el IdPaciente
            if (Request.Cookies.TryGetValue("IdPaciente", out string strPaciente))
            {
                int IdPaciente = Int32.Parse(strPaciente);

                var paciente = _database.Pacientes.GetById(IdPaciente);

                if (paciente == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(paciente);
                }

            }
            else
            {
                TempData["ErrorMessage"] = "Error al cargar Configuración de la Cuenta";
                return RedirectToAction("Index", "Paciente");
            }
        }

        public IActionResult Edit()
        {
            
            //Buscamos la información por el IdPaciente
            if (Request.Cookies.TryGetValue("IdPaciente", out string strPaciente))
            {
                int IdPaciente = Int32.Parse(strPaciente);

                var paciente = _database.Pacientes.GetById(IdPaciente);

                

                if (paciente == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(paciente);
                }

            }
            else
            {
                TempData["ErrorMessage"] = "Error al cargar Editar Cuenta";
                return RedirectToAction("Details", "Paciente");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Paciente paciente)
        {
            try
            {
                //Guardamos el IdPaciente
                if (Request.Cookies.TryGetValue("IdPaciente", out string strPaciente))
                {
                    paciente.IdPaciente = Int32.Parse(strPaciente);

                    var result = _database.Pacientes.Edit(paciente);

                    if (!result.Success)
                    {
                        TempData["AlertMessage"] = "La información no se pudo actualizar.";
                        return View(paciente);
                    }

                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Details", "Paciente");

                }
                else
                {
                    TempData["ErrorMessage"] = "Error al actualizar Editar Cuenta";
                    return RedirectToAction("Index", "Paciente");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Error al actualizar la información. " + e.Message;
                return RedirectToAction("IndexPaciente", "Cita");
            }
        }
    }
}
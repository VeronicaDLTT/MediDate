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
    public class MedicoController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<MedicoController> _logger;

        public MedicoController(ILogger<MedicoController> logger)
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
            ViewBag.IdEspecialidad = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Medico medico, string Email, string Pass)
        {

            try
            {

                //Verificar si el correo ingresado ya existe
                var result = _database.Usuarios.VerificarEmail(Email);

                //Si no existe el correo
                if (!result.Success)
                {
                    //Guardamos los datos del Medico temporalmente
                    
                    TempData["Nombre"] = medico.Nombre;
                    TempData["PrimerApellido"] = medico.PrimerApellido;
                    TempData["SegundoApellido"] = medico.SegundoApellido;
                    TempData["IdEspecialidad"] = medico.IdEspecialidad;
                    TempData["NumCedula"] = medico.NumCedula;
                    TempData["Telefono"] = medico.Telefono;

                    //Mostramos la vista para agregar la información del Consultorio
                    return RedirectToAction("Create", "Consultorio");

                }
                else
                {
                    //Si el correo si existe
                    TempData["AlertMessage"] = result.Message;
                    ViewBag.IdEspecialidad = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
                    return View(medico);
                }

                

                ////Crear Usuario en la tabla Usuarios
                //var resultUsuario = _database.Usuarios.Create(Email, Pass, 'M');

                //if (!resultUsuario.Success)
                //{
                //    TempData["AlertMessage"] = "No fue posible crear el Usuario";
                //    ViewBag.IdEspecialidad = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
                //    return View(medico);
                //}
                //else
                //{
                //    //Obtenemos el ultimo IdUsuario que se creó
                //    var usuario = _database.Usuarios.GetLastUser();

                //    medico.IdUsuario = usuario.IdUsuario;

                //    //Crear Medico en la tabla Medicos
                //    var result = _database.Medicos.Create(medico);

                //    if (!result.Success)
                //    {
                //        TempData["AlertMessage"] = "No fue posible crear el Usuario";
                //        ViewBag.IdEspecialidad = new SelectList(_database.Especialidades.GetAll(), "IdEspecialidad", "Descripcion");
                //        return View(medico);
                //    }
                //    else
                //    {
                //        TempData["SuccessMessage"] = result.Message;
                //        //Mostramos la vista para agregar la información del Consultorio
                //        return RedirectToAction("Create", "Consultorio");
                //    }
                //}
            }
            catch (Exception e)
            {
                //Console.WriteLine("Excepción: " + e.Message);
                TempData["ErrorMessage"] = "No fue posible crear el Usuario" + e.Message;
                return RedirectToAction("Index", "Home");
            }
            
        }
    }
}
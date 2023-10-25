using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using System;
using System.Diagnostics;
using System.Globalization;
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
            try
            {
                if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
                {
                    int IdMedico = Int32.Parse(strMedico);

                    //Obtenemos el Horario de atención por IdMedico
                    var horario = _database.Horarios.GetByIdMedico(IdMedico);
                    ViewBag.HoraInicio = horario.HoraInicio.ToString("yyyy-MM-dd HH:mm:ss");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");

                    return View(_database.Citas.GetAllByMedico(IdMedico));
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                    return RedirectToAction("Index", "Medico");
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "No fue posible Iniciar Sesión. " + e.Message;
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult ObtenerCitas(string opcion, DateTime fecha1, DateTime fecha2)
        {
            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                int IdMedico = Int32.Parse(strMedico);

                DateTime dateValue = DateTime.Now;

                if (((fecha1.Year == 1) && (fecha1.Day == 1) && (fecha1.Month == 1)) && ((fecha2.Year == 1) && (fecha2.Day == 1) && (fecha2.Month == 1)))
                {
                    fecha1 = dateValue;
                    fecha2 = dateValue.AddDays(6);
                }

                ViewBag.Fecha = fecha1.ToString("dd/MM/yyyy");
                ViewBag.Fecha2 = fecha2.ToString("dd/MM/yyyy");

                // Lógica para obtener citas según la opción seleccionada
                if (opcion == "porSemana")
                {
                    var citasPorSemana = _database.Citas.GetAllByMedicoSemana(IdMedico, fecha1, fecha2);

                    // Obtenemos el Horario de atención por IdMedico
                    var horario = _database.Horarios.GetByIdMedico(IdMedico);
                    ViewBag.HoraInicio = horario.HoraInicio.ToString("yyyy-MM-dd HH:mm:ss");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");

                    // Devolver una vista parcial o datos JSON según sea necesario
                    return PartialView("_CitasPorSemana", citasPorSemana);
                }
                else if (opcion == "porDia")
                {
                    var citasPorDia = _database.Citas.GetAllByMedicoDia(IdMedico, fecha1);

                    // Obtenemos el Horario de atención por IdMedico
                    var horario = _database.Horarios.GetByIdMedico(IdMedico);
                    ViewBag.HoraInicio = horario.HoraInicio.ToString("yyyy-MM-dd HH:mm:ss");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");

                    // Devolver una vista parcial o datos JSON según sea necesario
                    return PartialView("_CitasPorDia", citasPorDia);
                }
                else if (opcion == "todas")
                {
                    var citasTodas = _database.Citas.GetAllByMedico(IdMedico);

                    // Devolver una vista parcial o datos JSON según sea necesario
                    return PartialView("_CitasTodas", citasTodas);
                }
                else
                {
                    TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                    return RedirectToAction("Index", "Medico");
                }

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

                    //Buscamos el horario de atencion
                    var horario = _database.Horarios.GetByIdMedico(IdMedico);

                    Cita cita = new Cita();
                    cita.NombreMedico = medico.NombreCompleto;

                    ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico), "IdDetServicio", "Servicio");
                    ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                    var selectList = ViewBag.IdDetServicio as SelectList;

                    if (selectList == null || selectList.Count() == 0)
                    {
                        TempData["AlertMessage"] = "Por el momento el especialista no esta disponible para agendar citas. ";
                        return RedirectToAction("Index", "Paciente");
                    }

                    return View(cita);
                }
                else
                {
                    return RedirectToAction("Index", "Paciente");
                }
                    
            }
            else
            {
                TempData["IniciarSesion"] = "Inicia sesión o Registrate para agendar citas. ";
                return RedirectToAction("Login", "Usuario");
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

                    //Buscamos el horario de atencion
                    var horario = _database.Horarios.GetByIdMedico(IdMedico);

                    cita.IdPaciente = IdPaciente;
                    cita.IdMedico = IdMedico;
                    cita.Estado = 1;

                    //Verificamos si se puede agendar la cita
                    var verificar = _database.Citas.Verificar(cita);

                    if (!verificar.Success)
                    {
                        //Creamos la cita
                        var result = _database.Citas.Create(cita);

                        //Si no se pudo crear la cita
                        if (!result.Success)
                        {
                            TempData["ErrorMessage"] = "Error al agendar la cita.";
                            ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico), "IdDetServicio", "Servicio");
                            ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                            ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");
                            return View(cita);
                        }
                        else
                        {
                            TempData["SuccessMessage"] = result.Message;
                            return RedirectToAction("IndexPaciente", "Cita");
                        }
                        
                    }
                    else
                    {
                        TempData["AlertMessage"] = verificar.Message;
                        ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico), "IdDetServicio", "Servicio");
                        ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                        ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");
                        return View(cita);
                    }
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

        public IActionResult CreateMedico()
        {
            //Verificamos si hay un Medico que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {

                int IdMedico = Int32.Parse(strIdMedico);

                //Buscamos el horario de atencion
                var horario = _database.Horarios.GetByIdMedico(IdMedico);

                ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico),"IdDetServicio","Servicio");
                ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                var selectList = ViewBag.IdDetServicio as SelectList;

                if (selectList == null || selectList.Count() == 0)
                {
                    TempData["AlertMessage"] = "No puedes agendar citas ya que no tienes Servicios registrados. Agrega un Servicio para poder agendar. ";
                    return RedirectToAction("Index", "DetServicio");
                }

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Medico");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateMedico(Cita cita, string txtBuscar)
        {
            
            //Verificamos si existe un valor en IdMedico
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {
                var strPaciente = txtBuscar.Split('-');
                string strIdPaciente = strPaciente[0];

                int IdMedico = Int32.Parse(strIdMedico);
                int IdPaciente = Int32.Parse(strIdPaciente);

                //Buscamos el horario de atencion
                var horario = _database.Horarios.GetByIdMedico(IdMedico);

                cita.IdPaciente = IdPaciente;
                cita.IdMedico = IdMedico;
                cita.Estado = 1;

                //Verificamos si se puede agendar la cita
                var verificar = _database.Citas.Verificar(cita);

                if (!verificar.Success)
                {
                    //Creamos la cita
                    var result = _database.Citas.Create(cita);

                    //Si no se pudo crear la cita
                    if (!result.Success)
                    {
                        TempData["ErrorMessage"] = "Error al agendar la cita.";
                        ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico), "IdDetServicio", "Servicio");
                        ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                        ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");
                        return View(cita);
                    }
                    else
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("IndexMedico", "Cita");

                    }
                }
                else
                {
                    TempData["AlertMessage"] = verificar.Message;
                    ViewBag.IdDetServicio = new SelectList(_database.DetServicios.GetAllByMedico(IdMedico), "IdDetServicio", "Servicio");
                    ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");
                    return View(cita);
                }
            }
            else
            {
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult EditPaciente(int IdCita)
        {
            //Verificamos si hay un Paciente que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdPaciente", out string strIdPaciente))
            {
                
                //Buscamos los datos de la Cita
                var cita = _database.Citas.GetById(IdCita);

                //Buscamos el horario de atencion
                var horario = _database.Horarios.GetByIdMedico(cita.IdMedico);

                ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                cita.Fecha = DateTime.Now.AddDays(1);

                return View(cita);
            }
            else
            {

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPaciente(Cita cita)
        {
            //Verificamos si hay un Paciente que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdPaciente", out string strIdPaciente))
            {

                //Verificamos si se puede reagendar la cita
                var verificar = _database.Citas.Verificar(cita);

                if (!verificar.Success)
                {
                    cita.Estado = 1;

                    //Reagendamos la cita
                    var result = _database.Citas.Edit(cita);

                    if (!result.Success)
                    {
                        //No se logro reagendar la cita
                        TempData["AlertMessage"] = "Error al reagendar la cita, intente de nuevo. ";

                        //Buscamos el horario de atencion
                        var horario = _database.Horarios.GetByIdMedico(cita.IdMedico);

                        ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                        ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                        return View(cita);
                    }
                    else
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("IndexPaciente", "Cita");
                    }

                }
                else
                {
                    //No se puede reagendar, ya existe una cita en esa fecha y hora
                    TempData["AlertMessage"] = verificar.Message;

                    //Buscamos el horario de atencion
                    var horario = _database.Horarios.GetByIdMedico(cita.IdMedico);

                    ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                    return View(cita);
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult EditMedico(int IdCita)
        {
            //Verificamos si hay un Medico que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {

                //Buscamos los datos de la Cita
                var cita = _database.Citas.GetById(IdCita);

                //Buscamos el horario de atencion
                var horario = _database.Horarios.GetByIdMedico(cita.IdMedico);

                ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                cita.Fecha = DateTime.Now.AddDays(1);

                return View(cita);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMedico(Cita cita)
        {
            //Verificamos si hay un Medico que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {

                //Verificamos si se puede reagendar la cita
                var verificar = _database.Citas.Verificar(cita);

                if (!verificar.Success)
                {
                    cita.Estado = 1;

                    //Reagendamos la cita
                    var result = _database.Citas.Edit(cita);

                    if (!result.Success)
                    {
                        //No se logro reagendar la cita
                        TempData["AlertMessage"] = "Error al reagendar la cita, intente de nuevo. ";

                        //Buscamos el horario de atencion
                        var horario = _database.Horarios.GetByIdMedico(cita.IdMedico);

                        ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                        ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                        return View(cita);
                    }
                    else
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("IndexMedico", "Cita");
                    }

                }
                else
                {
                    //No se puede reagendar, ya existe una cita en esa fecha y hora
                    TempData["AlertMessage"] = verificar.Message;

                    //Buscamos el horario de atencion
                    var horario = _database.Horarios.GetByIdMedico(cita.IdMedico);

                    ViewBag.HoraInicio = horario.HoraInicio.ToString("HH:mm");
                    ViewBag.HoraFin = horario.HoraFin.AddHours(-1).ToString("HH:mm");

                    return View(cita);
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult EditEstado(int Estado, int IdCita)
        {
            //Verificamos si hay un Medico que ha iniciado sesion
            if (Request.Cookies.TryGetValue("IdMedico", out string strIdMedico))
            {

                //Actualizamos el estadp
                var result = _database.Citas.EditEstado(IdCita, Estado);

                if (!result.Success)
                {
                    //No se logro editar el estado la cita
                    TempData["AlertMessage"] = "Error al cambiar el estado de la cita. ";
                }
                else
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                return RedirectToAction("IndexMedico", "Cita");

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public JsonResult GetDropdownData(string txtBuscar)
        {
            var pacientes = _database.Pacientes.GetAllPacientesCorreos(txtBuscar);

            var pacientesStrings = pacientes.Select(e => e.NombreCompleto).ToList();

            var dropdownData = new List<string>();
            dropdownData.AddRange(pacientesStrings);

            return Json(dropdownData);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
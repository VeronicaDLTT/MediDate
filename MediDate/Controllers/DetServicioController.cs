using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;

namespace MediDate.Controllers
{
    public class DetServicioController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<DetServicioController> _logger;

        public DetServicioController(ILogger<DetServicioController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Index()
        {
            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                int IdMedico = Int32.Parse(strMedico);

                return View(_database.DetServicios.GetAllByIdMedico(IdMedico));
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                return RedirectToAction("Index", "Medico");
            }

        }

        public IActionResult Create()
        {
            ViewBag.Servicios = new SelectList(_database.Servicios.GetAll(), "IdServicio", "Descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DetServicio detServicio, string txtBuscar)
        {
            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                int IdMedico = Int32.Parse(strMedico);

                //Buscamos la informacion del Consultorio por el IdMedico
                var consultorio = _database.Consultorios.GetByIdMedico(IdMedico);

                detServicio.IdMedico = IdMedico;
                detServicio.IdConsultorio = consultorio.IdConsultorio;

                if (detServicio.Servicio != null)
                {


                    //Agregamos el Servicio en la tabla Servicios y DetServicios
                    var result = _database.DetServicios.Create2(detServicio);

                    if (!result.Success)
                    {

                        TempData["AlertMessage"] = "Error al registrar el servicio. Intente de nuevo. ";
                        ViewBag.Servicios = new SelectList(_database.Servicios.GetAll(), "IdServicio", "Descripcion");

                        return View(detServicio);
                    }

                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index", "DetServicio");
                }
                else
                {

                    //Agregamos en la tabla DetServicios
                    var result = _database.DetServicios.Create(detServicio);

                    if (!result.Success)
                    {

                        TempData["AlertMessage"] = "Error al registrar el servicio. Intente de nuevo. ";
                        ViewBag.Servicios = new SelectList(_database.Servicios.GetAll(), "IdServicio", "Descripcion");

                        return View(detServicio);
                    }

                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index", "DetServicio");
                }

            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo cargar la lista de Citas";
                return RedirectToAction("Index", "Medico");
            }
        }

        public IActionResult Edit (int IdDetServicio)
        {
            //Buscamos el DetServicio por su Id
            var detServicio = _database.DetServicios.GetById(IdDetServicio);

            if (detServicio == null)
            {
                return NotFound();
            }
            else
            {
                return View(detServicio);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(DetServicio detServicio)
        {
            //Actualizamos la Descripcion en la tabla de Servicios
            Servicio servicio = new Servicio();
            servicio.IdServicio = detServicio.IdServicio;
            servicio.Descripcion = detServicio.Servicio;

            var result = _database.Servicios.Edit(servicio);

            //Actualizamos el Costo en la tabla de DetServicios
            var result2 = _database.DetServicios.Edit(detServicio);

            if (!result.Success && !result2.Success)
            {
                TempData["AlertMessage"] = "Error al modificar el servicio. Intente de nuevo. ";
                return View(detServicio);
            }
            else
            {
                TempData["SuccessMessage"] = result2.Message;
                return RedirectToAction("Index", "DetServicio");
            }
        }

        public IActionResult Delete(int IdDetServicio)
        {
            //Buscamos el DetServicio por su Id
            var detServicio = _database.DetServicios.GetById(IdDetServicio);

            if (detServicio == null)
            {
                return NotFound();
            }
            else
            {
                return View(detServicio);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int IdDetServicio)
        {
            //Eliminamos el servicio
            var result = _database.DetServicios.Delete(IdDetServicio);

            if (!result.Success)
            {
                TempData["AlertMessage"] = "Error al eliminar el servicio. Intente de nuevo. ";
                return View(IdDetServicio);
            }
            else
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index", "DetServicio");
            }
        }

        [HttpGet]
        public JsonResult GetDropdownData(string txtBuscar)
        {
            var servicios = _database.Servicios.GetAllIdServicioDesc(txtBuscar);

            var serviciosStrings = servicios.Select(e => e.Descripcion).ToList();

            var dropdownData = new List<string>();
            dropdownData.AddRange(serviciosStrings);

            return Json(dropdownData);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
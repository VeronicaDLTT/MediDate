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
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DetServicio detServicio, string txtBuscar)
        {
            if (Request.Cookies.TryGetValue("IdMedico", out string strMedico))
            {
                var arrayServicios = txtBuscar.Split('-');

                int IdMedico = Int32.Parse(strMedico);

                //Buscamos la informacion del Consultorio por el IdMedico
                var consultorio = _database.Consultorios.GetByIdMedico(IdMedico);

                detServicio.IdMedico = IdMedico;
                detServicio.IdConsultorio = consultorio.IdConsultorio;

                if (arrayServicios[0].Equals(txtBuscar))
                {
                    
                    detServicio.Servicio = txtBuscar;

                    //Agregamos el Servicio en la tabla Servicios y DetServicios
                    var result = _database.DetServicios.Create2(detServicio);

                    if (!result.Success)
                    {

                        TempData["AlertMessage"] = "Error al registrar el servicio. Intente de nuevo. ";
                        return View(detServicio);
                    }

                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index", "DetServicio");
                }
                else
                {
                    string strIdServicio = arrayServicios[0];
                    detServicio.IdServicio = Int32.Parse(strMedico);

                    //Agregamos en la tabla DetServicios
                    var result = _database.DetServicios.Create(detServicio);

                    if (!result.Success)
                    {

                        TempData["AlertMessage"] = "Error al registrar el servicio. Intente de nuevo. ";
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
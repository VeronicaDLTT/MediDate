using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Packaging;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Reflection.Emit;
using System.Web.Helpers;

namespace MediDate.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger)
        {
            _logger = logger;
            _database = new Database();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Usuario usuario)
        {

            try
            {

                //Valida si el usuario tiene acceso al sistema
                var result = _database.Usuarios.Login(usuario);

                //Si tiene acceso muestra la pagina principal
                if (result.Success)
                {
                    var resultUsuario = _database.Usuarios.DatosUsuario(usuario);

                    if (!resultUsuario.Equals(null))
                    {

                        Response.Cookies.Append("IdUsuario", resultUsuario.IdUsuario.ToString());
                        Response.Cookies.Append("TipoUsuario", resultUsuario.TipoUsuario.ToString());
                        Response.Cookies.Append("Email", resultUsuario.Email);

                        //Si el Tipo de Usuario es M muestro la pagina principal de Citas Medico
                        if (resultUsuario.TipoUsuario == 'M')
                        {
                            Response.Cookies.Append("IdMedico", resultUsuario.IdMedico.ToString());
                            return RedirectToAction("IndexMedico", "Cita");
                        }
                        else if (resultUsuario.TipoUsuario == 'P')
                        {
                            //Si el Tipo de Usuario es P muestro la pagina principal de Citas Paciente
                            Response.Cookies.Append("IdPaciente", resultUsuario.IdPaciente.ToString());
                            return RedirectToAction("IndexPaciente", "Cita");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No hay información disponible. ";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["AlertMessage"] = result.Message;
                    return View(usuario);
                }

            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Error al ingresar. " + e.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult LogOut()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Restablecer()
        {
            return View();
        }
    }
}
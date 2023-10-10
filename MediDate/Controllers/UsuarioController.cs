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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restablecer(string Email)
        {
              
            MailMessage email = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            var emailDestino = Email;
            var emailOrigen = "a348759@gmail.com";
            var passOrigen = "jump4n5q8";

            email.To.Add(new MailAddress(emailDestino));
            email.From = new MailAddress(emailOrigen);
            email.Subject = "Restablecer contraseña";
            email.SubjectEncoding = System.Text.Encoding.UTF8;
            email.Body = "Click <a href=\"http://localhost:7141/Usuario/Actualizar\">here</a> to access the update password.";
            email.IsBodyHtml = true;
            email.Priority = MailPriority.Normal;

            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587; // Puerto SMTP de Gmail
            //smtp.Timeout = 60000; // Tiempo de espera en milisegundos (60 segundos)
            smtp.EnableSsl = true; // Habilita SSL para Gmail
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(emailOrigen, passOrigen);


            try
            {
                smtp.Send(email);
                email.Dispose();
                TempData["SuccessMessage"] = "Correo electrónico fue enviado satisfactoriamente.";

                return RedirectToAction("Login", "Usuario");
            }
            catch (SmtpException exm)
            {
                TempData["ErrorMessage"] = "Error al enviar el correo. STMP. No. Error: " + exm.StatusCode + " Message: " + exm.Message;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                
                TempData["ErrorMessage"] = "Error al enviar el correo. " + e.Message;
                return RedirectToAction("Index", "Home");
            }

            
            
        }
    }
}
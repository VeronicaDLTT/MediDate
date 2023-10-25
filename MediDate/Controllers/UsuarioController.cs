using MediDate.Models;
using MediDate.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using MediDate.Services;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Intrinsics.Arm;
using Aes = System.Security.Cryptography.Aes;
using System.Runtime.Intrinsics.X86;

namespace MediDate.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly Database _database;
        private readonly ILogger<UsuarioController> _logger;
        private readonly IEmailService _emailService;
        
        private static readonly string EncryptionKey = GenerateRandomKeyAndIV();

        public UsuarioController(ILogger<UsuarioController> logger, IEmailService emailService)
        {
            _logger = logger;
            _database = new Database();
            _emailService = emailService;  
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

                        //Si el Tipo de Usuario es M
                        if (resultUsuario.TipoUsuario == 'M')
                        {
                            
                            Response.Cookies.Append("IdMedico", resultUsuario.IdMedico.ToString());

                            //Verifico si el Medico ya tiene un Horario de Atencion registrado
                            var horario = _database.Horarios.GetByIdMedico((int)resultUsuario.IdMedico);

                            if(horario == null)
                            {
                                //Si el Medico no tiene un horario le mostramos la vista para guardar el horario
                                return RedirectToAction("Create", "Horario");
                            }
                            else
                            {
                                //Si si tiene un horario registrado

                                //Actualizamos todas las citas anteriores a la fecha actual que siguen
                                //en Estado = 1 - En espera para cambiarlo a 3 = Pendiente
                                var editCitas = _database.Citas.EditEstadoPendiente((int)resultUsuario.IdMedico);

                                //le mostramos la vista de sus Citas
                                return RedirectToAction("IndexMedico", "Cita");
                            }
                            
                        }
                        else if (resultUsuario.TipoUsuario == 'P')
                        {
                            //Si el Tipo de Usuario es P muestro Index Paciente
                            Response.Cookies.Append("IdPaciente", resultUsuario.IdPaciente.ToString());
                            return RedirectToAction("Index", "Paciente");
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
            Correo request = new Correo();

            //Verificamos si el correo ingresado es valido
            var resultEmail = _database.Usuarios.VerificarEmail(Email);

            //Si el correo es valido, enviamos el correo para restablecer la contraseña
            if (resultEmail.Success)
            {
                string emailEncrypt = Encrypt(Email);

                request.Destinatario = Email;
                request.Asunto = "Restablecer contraseña - MediDate";
                request.Mensaje = "Abrir el siguiente <a href=\"https://localhost:7141/Usuario/Actualizar?email=" + emailEncrypt + "\">enlace</a> para restablecer su contraseña.";

                _emailService.SendMail(request);

                TempData["SuccessMessage"] = "Se ha enviado un correo a su cuenta para restablecer su contraseña";
                return RedirectToAction("Login", "Usuario");
            }
            else
            {
                //Si el correo no es valido
                TempData["AlertMessage"] = "El correo ingreado no es valido, intente con otro";
                return View();
            }
            
        }

        public IActionResult Actualizar(string Email)
        {
            string emailOriginal = Decrypt(Email);
            //Guardamos el Email
            Response.Cookies.Append("Email", emailOriginal);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Actualizar(string Pass1, string Pass2)
        {
            //Si las contraseñas son iguales
            if(Pass1 == Pass2)
            {
                //Obtenemos el valor del Email
                if (Request.Cookies.TryGetValue("Email", out string strEmail))
                {


                    //Actualizamos la contraseña
                    var result = _database.Usuarios.RestablecerPass(strEmail, Pass1);

                    if (!result.Success)
                    {
                        //Si no se logro actualizar la contraseña
                        TempData["AlertMessage"] = "Error al restablecer la contraseña";
                        return View(Pass1, Pass2);
                    }
                    else
                    {
                        foreach (var cookie in Request.Cookies.Keys)
                        {
                            Response.Cookies.Delete(cookie);
                        }

                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("Login", "Usuario");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al actualizar la contraseña";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                //Si las contraseñas no son iguales
                TempData["AlertMessage"] = "Las contraseñas no son iguales.";
                return View();
            }
            
        }

        public static string Encrypt(string clearText)
        {

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x64, 0x65, 0x64, 0x65, 0x64, 0x65, 0x64, 0x65, 0x64 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x64, 0x65, 0x64, 0x65, 0x64, 0x65, 0x64, 0x65, 0x64 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string GenerateRandomKeyAndIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                string key = Convert.ToBase64String(aes.Key);
                string iv = Convert.ToBase64String(aes.IV);

                return $"{key}:{iv}";
            }
        }
    }
}
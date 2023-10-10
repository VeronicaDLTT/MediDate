using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Usuarios: BaseQuery
    {
        public Usuarios() : base(){}

        public int? IdUsuario;
        public char? TipoUsuario;
        public int? IdMedico;
        public int? IdPaciente;
        public string? Email;

        /// <summary>
        /// Crear nuevo Usuario Medico en la tabla Usuarios
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <param name="paciente"></param>
        /// <returns>Mensaje si se creo o no el Usuario</returns>
        public BaseResult CreatePacientes(string Email, string Password, Paciente paciente)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_usuarios 1,'',@Email,@Password,'P',@FechaNacimiento,'',@Nombre,@Apellido,'','',@Telefono",
                    new{Email,Password,paciente.FechaNacimiento,paciente.Nombre,paciente.Apellido,paciente.Telefono});
            }
        }

        /// <summary>
        /// Crear nuevo Usuario Medico en la tabla Usuarios
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <param name="consultorio"></param>
        /// <returns>Mensaje si se creo o no el Usuario</returns>
        public BaseResult CreateMedicos(string Email, string Password,Medico medico, Consultorio consultorio)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_usuarios 2,'',@Email,@Password,'M',''," +
                    "@IdEspecialidad,@Nombre,@PrimerApellido,@SegundoApellido,@NumCedula,@Telefono," +
                    "@Descripcion,@Calle,@Colonia,@NumExterior,@CodigoPostal",
                    new { Email, Password,
                          medico.IdEspecialidad, medico.Nombre, medico.PrimerApellido, medico.SegundoApellido, medico.NumCedula, medico.Telefono,
                          consultorio.Descripcion, consultorio.Calle, consultorio.Colonia, consultorio.NumExterior, consultorio.CodigoPostal});
            }
        }

        /// <summary>
        /// Verificar si el correo que se envia ya existe
        /// </summary>
        /// <param name="Email"></param>
        /// <returns>Regrsa 1 si si existe el correo o 0 si no existe</returns>
        public BaseResult VerificarEmail(string Email)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>( "sp_usuarios 5,'',@Email",new{ Email,});
            }
        }

        /// <summary>
        /// Obtiene el ultimo Usuario de la tabla Usuarios
        /// </summary>
        /// <returns>Ultimo Usuario</returns>
        public Usuario GetLastUser()
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Usuario>("sp_usuarios 6");
            }
        }

        /// <summary>
        /// Valida al usuario para saber si tiene acceso al sistema
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns>Regresa 0 si no tiene acceso y 1 si tiene acceso</returns>
        public BaseResult Login(Usuario usuario)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>("sp_usuarios 7,'',@Email,@Password", new { usuario.Email, usuario.Password });
            }
        }

        /// <summary>
        /// Obtiene los datos del Usuario que ingresa
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns>Datos IdUsuario, TipoUsuario, IdMedico o IdPaciente</returns>
        public BaseUsuario DatosUsuario(Usuario usuario)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseUsuario>("sp_usuarios 8,'',@Email,@Password", new { usuario.Email, usuario.Password });
            }
        }
    }
}

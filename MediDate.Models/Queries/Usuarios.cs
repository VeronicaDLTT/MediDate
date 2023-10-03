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

        /// <summary>
        /// Crear nuevo Usuario en la tabla Usuarios
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <param name="TipoUsuario"></param>
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
    }
}

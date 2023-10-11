using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Pacientes: BaseQuery
    {
        public Pacientes() : base(){}

        /// <summary>
        /// Mostrar todos los Pacientes de la tabla Pacientes
        /// </summary>
        /// <returns>Lista de Pacientes</returns>
        public List<Paciente> GetAll()
        {
            var Pacientes = new List<Paciente>();

            using(var db = GetConnection())
            {
                Pacientes = db.Query<Paciente>("sp_pacientes 5").ToList();
            }

            return Pacientes;
        }

        /// <summary>
        /// Busca un Paciente en la tabla Pacientes por el IdPaciente
        /// </summary>
        /// <param name="IdPaciente"></param>
        /// <returns>Paciente que coincida con el IdPaciente</returns>
        public Paciente GetById(int IdPaciente)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Paciente>("sp_pacientes 4,@IdPaciente", new { IdPaciente });
            }
        }

        public BaseResult Edit(Paciente paciente)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_pacientes 3,@IdPaciente,'',@Nombre,@Apellido,@FechaNacimiento,@Telefono",
                    new { paciente.IdPaciente, paciente.Nombre, paciente.Apellido, paciente.FechaNacimiento, paciente.Telefono });
            }
        }

    }
}

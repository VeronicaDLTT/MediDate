using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Especialidades: BaseQuery
    {
        public Especialidades() : base(){}

        /// <summary>
        /// Mostrar todos las Especialidaddes de la tabla Especialidades
        /// </summary>
        /// <returns>Lista de Especialidades</returns>
        public List<Especialidad> GetAll()
        {
            var medicos = new List<Especialidad>();

            using(var db = GetConnection())
            {
                medicos = db.Query<Especialidad>("sp_especialidades 5,'',''").ToList();
            }

            return medicos;
        }

        public Especialidad GetById(int IdEspecialidad)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Especialidad>("sp_productos 4,@IdEspecialidad,''", new { IdEspecialidad });
            }
        }
    }
}

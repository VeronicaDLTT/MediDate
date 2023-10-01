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
            var especialidades = new List<Especialidad>();

            using(var db = GetConnection())
            {
                especialidades = db.Query<Especialidad>("sp_especialidades 5,'',''").ToList();
            }

            return especialidades;
        }

        public Especialidad GetById(int IdEspecialidad)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Especialidad>("sp_especialidades 4,@IdEspecialidad,''", new { IdEspecialidad });
            }
        }

        /// <summary>
        /// Busca todas las especialidades que coincidan con el parametro Descripcion
        /// </summary>
        /// <param name="Descripcion"></param>
        /// <returns>Lista de Especialidades</returns>
        public List<Especialidad> GetDescripciones(string Descripcion)
        {
            var especialidades = new List<Especialidad>();

            using (var db = GetConnection())
            {
                especialidades = db.Query<Especialidad>("sp_especialidades 6,'',@Descripcion", new {Descripcion}).ToList();
            }

            return especialidades;
        }
    }
}

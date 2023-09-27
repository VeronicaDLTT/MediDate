using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Medicos: BaseQuery
    {
        public Medicos() : base(){}

        /// <summary>
        /// Mostrar todos los Medicos de la tabla Medicos
        /// </summary>
        /// <returns>Lista de Medicos</returns>
        public List<Medico> GetAll()
        {
            var medicos = new List<Medico>();

            using(var db = GetConnection())
            {
                medicos = db.Query<Medico>("sp_medicos 5,'','','','','','',''").ToList();
            }

            return medicos;
        }
    }
}

using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class DetServicios: BaseQuery
    {
        public DetServicios() : base(){}

        /// <summary>
        /// Mostrar todos los DetServicios de la tabla DetServicios por IdMedico
        /// </summary>
        /// <returns>Lista de DetServicios</returns>
        public List<DetServicio> GetAllByMedico(int IdMedico)
        {
            var detServicios = new List<DetServicio>();

            using(var db = GetConnection())
            {
                detServicios = db.Query<DetServicio>("sp_detServicios 6,'','','',@IdMedico", new {IdMedico}).ToList();
            }

            return detServicios;
        }

        /// <summary>
        /// Busca un DetServicio por IdDetServicio
        /// </summary>
        /// <param name="IdDetServicio"></param>
        /// <returns>Registro de DetServicio que conincida con IdDetServicio</returns>
        public Servicio GetById(int IdDetServicio)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Servicio>("sp_detServicios 4,@IdDetServicio,''", new { IdDetServicio });
            }
        }
        
    }
}

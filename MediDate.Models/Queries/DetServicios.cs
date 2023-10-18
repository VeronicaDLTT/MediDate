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
                detServicios = db.Query<DetServicio>("sp_detServicios 7,'','','',@IdMedico", new {IdMedico}).ToList();
            }

            return detServicios;
        }

        public List<DetServicio> GetAllByIdMedico(int IdMedico)
        {
            var detServicios = new List<DetServicio>();

            using (var db = GetConnection())
            {
                detServicios = db.Query<DetServicio>("sp_detServicios 6,'','','',@IdMedico", new { IdMedico }).ToList();
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

        public BaseResult Create(DetServicio detServicio)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_detServicios 1,'',@IdServicio,@IdConsultorio,@IdMedico,@Costo",
                    new { detServicio.IdServicio, detServicio.IdConsultorio, detServicio.IdMedico, detServicio.Costo });
            }
        }

        public BaseResult Create2(DetServicio detServicio)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_detServicios 8,'',@IdServicio,@IdConsultorio,@IdMedico,@Costo, @Servicio",
                    new { detServicio.IdServicio, detServicio.IdConsultorio, detServicio.IdMedico, detServicio.Costo, detServicio.Servicio });
            }
        }

    }
}

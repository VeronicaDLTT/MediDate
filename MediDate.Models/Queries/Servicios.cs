using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Servicios: BaseQuery
    {
        public Servicios() : base(){}

        /// <summary>
        /// Mostrar todos los Servicios de la tabla Servicios
        /// </summary>
        /// <returns>Lista de Servicios</returns>
        public List<Servicio> GetAll()
        {
            var servicios = new List<Servicio>();

            using(var db = GetConnection())
            {
                servicios = db.Query<Servicio>("sp_servicios 5,'',''").ToList();
            }

            return servicios;
        }

        public List<Servicio> GetAllIdServicioDesc(string Descripcion)
        {
            var servicios = new List<Servicio>();

            using (var db = GetConnection())
            {
                servicios = db.Query<Servicio>("sp_servicios 7,'',@Descripcion", new {Descripcion}).ToList();
            }

            return servicios;
        }

        public Servicio GetById(int IdServicio)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Servicio>("sp_servicios 4,@IdServicio,''", new { IdServicio });
            }
        }

        /// <summary>
        /// Busca todos los Servicios que coincidan con el parametro Descripcion
        /// </summary>
        /// <param name="Descripcion"></param>
        /// <returns>Lista de Servicios</returns>
        public List<Servicio> GetDescripciones(string Descripcion)
        {
            var servicios = new List<Servicio>();

            using (var db = GetConnection())
            {
                servicios = db.Query<Servicio>("sp_servicios 6,'',@Descripcion", new {Descripcion}).ToList();
            }

            return servicios;
        }

        public BaseResult Edit(Servicio servicio)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_servicios 3,@IdServicio,@Descripcion",
                    new { servicio.IdServicio, servicio.Descripcion });
            }
        }
    }
}

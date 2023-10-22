using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Consultorios: BaseQuery
    {
        public Consultorios() : base(){}

        /// <summary>
        /// Mostrar todos los Consultorios de la tabla Consultorios
        /// </summary>
        /// <returns>Lista de Consultorios</returns>
        public List<Consultorio> GetAll()
        {
            var medicos = new List<Consultorio>();

            using(var db = GetConnection())
            {
                medicos = db.Query<Consultorio>("sp_consultorios 5").ToList();
            }

            return medicos;
        }

        /// <summary>
        /// Busca un Consultorio en la tabla Consultorios por el IdConsultorio
        /// </summary>
        /// <param name="IdConsultorio"></param>
        /// <returns>Consultorio que coincida con el IdConsultorio</returns>
        public Consultorio GetById(int IdConsultorio)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Consultorio>("sp_consultorios 4,@IdConsultorio", new { IdConsultorio });
            }
        }

        public Consultorio GetByIdMedico(int IdMedico)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Consultorio>("sp_consultorios 6,'',@IdMedico", new { IdMedico });
            }
        }

        /// <summary>
        /// Crea un nuevo Consultorio en la tabla Consultorios
        /// </summary>
        /// <param name="Consultorio"></param>
        /// <returns>Mensaje si se agregó el Consultorio o no se pudo agregar</returns>
        public BaseResult Create(Consultorio consultorio)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>("sp_consultorios 1,'',@IdMedico,@Descripcion,@Calle,@Colonia,@NumExterior,@CodigoPostal", 
                                                            new { consultorio.IdMedico, consultorio.Descripcion, consultorio.Calle, consultorio.Colonia, consultorio.NumExterior, consultorio.CodigoPostal });
            }
        }

        public BaseResult Edit(Consultorio consultorio)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>("sp_consultorios 3,@IdConsultorio,'',@Descripcion,@Calle,@Colonia,@NumExterior,@CodigoPostal",
                                                            new { consultorio.IdConsultorio, consultorio.Descripcion, consultorio.Calle, consultorio.Colonia, consultorio.NumExterior, consultorio.CodigoPostal });
            }
        }
    }
}

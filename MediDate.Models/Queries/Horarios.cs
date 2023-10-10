using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Horarios: BaseQuery
    {
        public Horarios() : base(){}

        /// <summary>
        /// Mostrar todos los Horarios de la tabla Horarios
        /// </summary>
        /// <returns>Lista de Horarios</returns>
        public List<Horario> GetAll()
        {
            var horarios = new List<Horario>();

            using(var db = GetConnection())
            {
                horarios = db.Query<Horario>("sp_horarios 5,").ToList();
            }

            return horarios;
        }

        /// <summary>
        /// Busca un Horario por IdHorario
        /// </summary>
        /// <param name="IdHorario"></param>
        /// <returns>Registro de Horario que conincida con IdHorario</returns>
        public Servicio GetById(int IdHorario)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Servicio>("sp_horarios 4,@IdHorario,''", new { IdHorario });
            }
        }

        /// <summary>
        /// Busca un Horario por IdMedico
        /// </summary>
        /// <param name="IdMedico"></param>
        /// <returns>Registro de Horario que conincida con IdMedico</returns>
        public Servicio GetByIdMedico(int IdMedico)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Servicio>("sp_horarios 6,'',@IdMedico", new { IdMedico });
            }
        }

        
    }
}

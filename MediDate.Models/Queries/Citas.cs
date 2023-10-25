using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Citas: BaseQuery
    {
        public Citas() : base(){}

        /// <summary>
        /// Mostrar todas las Citas en estado de Espera de la tabla Citas por cada Medico
        /// </summary>
        /// <returns>Lista de Citas</returns>
        public List<Cita> GetAllByMedicoSemana(int IdMedico, DateTime fecha1, DateTime fecha2)
        {
            var citas = new List<Cita>();

            using(var db = GetConnection())
            {
                citas = db.Query<Cita>("sp_citas 5,'',@IdMedico,'','','','','',@Fecha1,@Fecha2", new {IdMedico, fecha1, fecha2}).ToList();
            }

            return citas;
        }

        public List<Cita> GetAllByMedicoDia(int IdMedico, DateTime Fecha1)
        {
            var citas = new List<Cita>();

            using (var db = GetConnection())
            {
                citas = db.Query<Cita>("sp_citas 10,'',@IdMedico,'','','','','',@Fecha1", new { IdMedico, Fecha1 }).ToList();
            }

            return citas;
        }

        public List<Cita> GetAllByMedico(int IdMedico)
        {
            var citas = new List<Cita>();

            using (var db = GetConnection())
            {
                citas = db.Query<Cita>("sp_citas 9,'',@IdMedico", new { IdMedico }).ToList();
            }

            return citas;
        }

        /// <summary>
        /// Mostrar todas las Citas en estado de Espera de la tabla Citas por cada Paciente
        /// </summary>
        /// <returns>Lista de Citas</returns>
        public List<Cita> GetAllByPaciente(int IdPaciente)
        {
            var citas = new List<Cita>();

            using (var db = GetConnection())
            {
                citas = db.Query<Cita>("sp_citas 6,'','',@IdPaciente", new { IdPaciente }).ToList();
            }

            return citas;
        }

        /// <summary>
        /// Busca una Cita por el IdCita
        /// </summary>
        /// <param name="IdCita"></param>
        /// <returns>Cita</returns>
        public Cita GetById(int IdCita)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Cita>("sp_citas 4,@IdCita,''", new { IdCita });
            }
        }

        public BaseResult Create(Cita cita)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_citas 1,'',@IdMedico,@IdPaciente,@IdDetServicio,@Fecha,@Hora,@Estado", 
                    new {cita.IdMedico, cita.IdPaciente, cita.IdDetServicio, cita.Fecha, cita.Hora, cita.Estado });
            }
        }

        public BaseResult Verificar(Cita cita)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_citas 8,'',@IdMedico,@IdPaciente,'',@Fecha,@Hora,''",
                    new { cita.IdMedico, cita.IdPaciente, cita.Fecha, cita.Hora});
            }
        }

        public BaseResult Edit(Cita cita)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_citas 3,@IdCita,'','','',@Fecha,@Hora,@Estado",
                    new { cita.IdCita, cita.Fecha, cita.Hora, cita.Estado });
            }
        }

        public BaseResult EditEstado(int IdCita, int Estado)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_citas 7,@IdCita,'','','','','',@Estado",
                    new {IdCita, Estado });
            }
        }

        public BaseResult EditEstadoPendiente(int IdMedico)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_citas 11, '', @IdMedico", new { IdMedico });
            }
        }
    }
}

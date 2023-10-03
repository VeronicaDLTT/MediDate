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
                medicos = db.Query<Medico>("sp_medicos 5").ToList();
            }

            return medicos;
        }

        /// <summary>
        /// Busca un Medico en la tabla Medicos por el IdMedico
        /// </summary>
        /// <param name="IdMedico"></param>
        /// <returns>Medico que coincida con el IdMedico</returns>
        public Medico GetById(int IdMedico)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Medico>("sp_medicos 4,@IdMedico", new { IdMedico });
            }
        }

        /// <summary>
        /// Crea un nuevo Medico en la tabla Medicos
        /// </summary>
        /// <param name="Medico"></param>
        /// <returns>Mensaje si se agregó el Medico o no se pudo agregar</returns>
        public BaseResult Create(Medico medico)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>("sp_medicos 1,'',@IdUsuario,@IdEspecialidad,@Nombre,@PrimerApellido,@SegundoApellido,@NumCedula,@Telefono", 
                                                            new { medico.IdUsuario, medico.IdEspecialidad, medico.Nombre, medico.PrimerApellido, medico.SegundoApellido, medico.NumCedula, medico.Telefono });
            }
        }

        /// <summary>
        /// Busca los medicos por nombre, especialidad y servicio que ofrecen
        /// </summary>
        /// <param name="TextoBusqueda"></param>
        /// <returns>Lista de Medicos</returns>
        public List<Medico> Busqueda(string TextoBusqueda)
        {
            var medicos = new List<Medico>();

            using (var db = GetConnection())
            {
                medicos = db.Query<Medico>("sp_busqueda 2,@TextoBusqueda", new { TextoBusqueda }).ToList();
            }

            return medicos;
        }

        /// <summary>
        /// Nos indica si hay registro de medicos dependiendo del nombre, especialidad o servicio
        /// </summary>
        /// <param name="TextoBusqueda"></param>
        /// <returns>Mensaje que nos indica si hay registros o no</returns>
        public BaseResult BusquedaSuccess(string TextoBusqueda)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>("sp_busqueda 1,@TextoBusqueda", new { TextoBusqueda });
            }
        }

        /// <summary>
        /// Obtiene el ultimo Médico de la tabla Medicos
        /// </summary>
        /// <returns>Ultimo Médico</returns>
        public Medico GetLastUser()
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Medico>("sp_medicos 6");
            }
        }
    }
}

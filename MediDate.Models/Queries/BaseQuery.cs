using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class BaseQuery
    {
        /// <summary>
        /// Guardar la cadena de conexión internamente para todas las llamadas a DB
        /// </summary>
        internal string _connectionString;

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseQuery()
        {
            _connectionString = Environment.GetEnvironmentVariable("MediDate_ConnStr");
        }

        /// <summary>
        /// Obtener la cadena de conexión
        /// </summary>
        /// <returns>SqlConnection</returns>
        public IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public class BaseResult
        {
            /// <summary>
            /// Tiene el valor de si fue exitoso el proceso en la DB
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// Mensaje de error si es que existiera alguno
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// ID del objeto recien creado
            /// </summary>
            public int ObjectID { get; set; }
        }
    }
}

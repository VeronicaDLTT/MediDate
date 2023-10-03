using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Database
    {
        public Usuarios Usuarios { get; set; }
        public Medicos Medicos { get; set; }
        public Especialidades Especialidades {get; set; }
        public Servicios Servicios { get; set; }
        public Consultorios Consultorios { get; set; }

        public Database()
        {
            Usuarios = new Usuarios();
            Medicos = new Medicos();
            Especialidades = new Especialidades();
            Servicios = new Servicios();
            Consultorios = new Consultorios();
        }
    }
}

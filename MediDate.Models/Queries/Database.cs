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
        public Pacientes Pacientes { get; set; }
        public Medicos Medicos { get; set; }
        public Especialidades Especialidades {get; set; }
        public Servicios Servicios { get; set; }
        public DetServicios DetServicios { get; set; }
        public Consultorios Consultorios { get; set; }
        public Citas Citas { get; set; }
        public Horarios Horarios { get; set; }
        public Database()
        {
            Usuarios = new Usuarios();
            Pacientes = new Pacientes();
            Medicos = new Medicos();
            Especialidades = new Especialidades();
            Servicios = new Servicios();
            DetServicios = new DetServicios();
            Consultorios = new Consultorios();
            Citas = new Citas();
            Horarios = new Horarios();
        }
    }
}

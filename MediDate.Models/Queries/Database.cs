using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Database
    {
        public Medicos Medicos { get; set; }
        public Especialidades Especialidades {get; set; }

        public Database()
        {
            Medicos = new Medicos();
            Especialidades = new Especialidades();
        }
    }
}

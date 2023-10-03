using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models
{
    public class Consultorio
    {
        public int IdConsultorio { get; set; }
        public int IdMedico { get; set; }
        public string Descripcion { get; set; }
        public string Calle { get; set; }
        public string Colonia { get; set; }
        public string? NumExterior { get; set; }
        public string CodigoPostal { get; set; }
    }
}

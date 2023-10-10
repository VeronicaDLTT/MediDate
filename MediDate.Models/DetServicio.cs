using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models
{
    public class DetServicio
    {
        public int IdDetServicio { get; set; }
        public int IdServicio { get; set; }
        public int IdConsultorio { get; set; }
        public int IdMedico { get; set; }
        public double Costo { get; set; }
    }
}

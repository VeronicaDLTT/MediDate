using System.Globalization;

namespace MediDate.Models
{
    public class Cita
    {

        public int IdCita { get; set; }
        public int IdMedico { get; set; }
        public string? NombreMedico { get; set; }
        public int IdPaciente { get; set; }
        public string? NombrePaciente { get; set; }
        public int IdDetServicio { get; set; }
        public string Servicio { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Hora { get; set; }
        public int Estado { get; set; }
        public string DescEstado { get; set; }
    }
}
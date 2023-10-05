using System.Globalization;

namespace MediDate.Models
{
    public class Medico
    {

        public int IdMedico { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public string NombreCompleto { get; set; }
        public int IdEspecialidad { get; set; }
        public string Especialidad { get; set; }
        public string NumCedula { get; set; }
        public string? Telefono { get; set; }

    }
}
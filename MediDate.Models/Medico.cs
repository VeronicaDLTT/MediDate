﻿using System.Globalization;

namespace MediDate.Models
{
    public class Medico
    {

        public int IdMedico { get; set; }
        public string NombreCompleto { get; set; }
        public int IdEspecialidad { get; set; }
        public string Especialidad { get; set; }
        public string NumCedula { get; set; }
        public int Telefono { get; set; }

    }
}
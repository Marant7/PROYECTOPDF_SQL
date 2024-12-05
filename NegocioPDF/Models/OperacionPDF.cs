// NegocioPDF/Models/OperacionPDF.cs
using System;

namespace NegocioPDF.Models
{
    public class OperacionPDF
    {
        public int Id { get; set; } // ID de la operación
        public int UsuarioId { get; set; } // ID del usuario que realizó la operación
        public string TipoOperacion { get; set; } // Tipo de operación (e.g., "Fusionar", "Cortar")
        public DateTime FechaOperacion { get; set; } // Fecha y hora en que se realizó la operación
    }
}

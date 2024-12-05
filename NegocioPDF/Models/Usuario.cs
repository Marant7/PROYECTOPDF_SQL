// NegocioPDF/Models/Usuario.cs

namespace NegocioPDF.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }

        public static implicit operator Usuario(int v)
        {
            throw new NotImplementedException();
        }
    }
}

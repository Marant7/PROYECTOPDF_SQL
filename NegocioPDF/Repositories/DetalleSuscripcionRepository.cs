using Dapper;
using Microsoft.Data.SqlClient;
using NegocioPDF.Models;
using System.Linq;

namespace NegocioPDF.Repositories
{
    public class DetalleSuscripcionRepository
    {
        private readonly string _connectionString;

        public DetalleSuscripcionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DetalleSuscripcion ObtenerPorUsuarioId(int usuarioId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = @"
                    SELECT ds.*, 
                           u.Id AS UsuarioId, 
                           u.Nombre, 
                           u.Correo, 
                           u.Password 
                    FROM detalles_suscripciones ds
                    INNER JOIN Usuarios u ON ds.usuario_id = u.Id
                    WHERE ds.usuario_id = @UsuarioId
                    ORDER BY ds.Id DESC";

                var detalle = connection.Query<DetalleSuscripcion, Usuario, DetalleSuscripcion>(
                    query,
                    (detalleSuscripcion, usuario) =>
                    {
                        detalleSuscripcion.Usuario = usuario;
                        return detalleSuscripcion;
                    },
                    new { UsuarioId = usuarioId },
                    splitOn: "UsuarioId"
                ).FirstOrDefault();

                return detalle;
            }
        }

        public void ActualizarSuscripcion(DetalleSuscripcion suscripcion)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"
                    UPDATE detalles_suscripciones 
                    SET tipo_suscripcion = @tipo_suscripcion,
                        fecha_inicio = @fecha_inicio,
                        fecha_final = @fecha_final,
                        precio = @precio,
                        operaciones_realizadas = @operaciones_realizadas
                    WHERE usuario_id = @UsuarioId";

                connection.Execute(query, suscripcion);
            }
        }
    }
}
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using NegocioPDF.Models;
using System.Linq;

namespace NegocioPDF.Repositories
{
    public class OperacionesPDFRepository
    {
        private readonly string _connectionString;

        public OperacionesPDFRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool RegistrarOperacionPDF(int usuarioId, string tipoOperacion)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var suscripcion = connection.QueryFirstOrDefault<DetalleSuscripcion>(
                            "SELECT * FROM detalles_suscripciones WHERE usuario_id = @UsuarioId",
                            new { UsuarioId = usuarioId },
                            transaction
                        );

                        if (suscripcion.tipo_suscripcion == "basico" && suscripcion.operaciones_realizadas >= 5)
                        {
                            return false;
                        }

                        connection.Execute(
                            @"INSERT INTO operaciones_pdf (usuario_id, TipoOperacion, fecha_operacion) 
                              VALUES (@UsuarioId, @TipoOperacion, GETDATE())",
                            new { UsuarioId = usuarioId, TipoOperacion = tipoOperacion },
                            transaction
                        );

                        if (suscripcion.tipo_suscripcion == "basico")
                        {
                            connection.Execute(
                                @"UPDATE detalles_suscripciones 
                                  SET operaciones_realizadas = operaciones_realizadas + 1 
                                  WHERE usuario_id = @UsuarioId",
                                new { UsuarioId = usuarioId },
                                transaction
                            );
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public IEnumerable<OperacionPDF> ObtenerOperacionesPorUsuario(int usuarioId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<OperacionPDF>(
                    @"SELECT * FROM operaciones_pdf 
                      WHERE usuario_id = @UsuarioId 
                      ORDER BY fecha_operacion DESC",
                    new { UsuarioId = usuarioId }
                ).ToList();
            }
        }

        public int ContarOperacionesRealizadas(int usuarioId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QuerySingle<int>(
                    "SELECT COUNT(*) FROM operaciones_pdf WHERE usuario_id = @UsuarioId",
                    new { UsuarioId = usuarioId }
                );
            }
        }

        public bool ValidarOperacion(int usuarioId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var resultado = connection.QueryFirstOrDefault(
                    @"SELECT operaciones_realizadas, tipo_suscripcion 
                      FROM detalles_suscripciones 
                      WHERE usuario_id = @UsuarioId",
                    new { UsuarioId = usuarioId }
                );

                return !(resultado.tipo_suscripcion == "basico" && resultado.operaciones_realizadas >= 5);
            }
        }
    }
}
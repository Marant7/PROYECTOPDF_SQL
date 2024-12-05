using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Dapper;
using NegocioPDF.Models;

namespace NegocioPDF.Repositories
{
    public class UsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Usuario Login(string correo, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var usuario = connection.QueryFirstOrDefault<Usuario>(
                    "SELECT * FROM Usuarios WHERE Correo = @Correo AND Password = @Password",
                    new { Correo = correo, Password = password });

                return usuario;
            }
        }

        public void RegistrarUsuario(Usuario usuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var existeUsuario = connection.QueryFirstOrDefault<Usuario>(
                            "SELECT * FROM Usuarios WHERE Correo = @Correo",
                            new { Correo = usuario.Correo },
                            transaction);

                        if (existeUsuario != null)
                        {
                            throw new Exception("El correo ya está registrado");
                        }

                        var sqlInsertUser = "INSERT INTO Usuarios (Nombre, Correo, Password) VALUES (@Nombre, @Correo, @Password); SELECT SCOPE_IDENTITY();";
                        usuario.Id = connection.QuerySingle<int>(sqlInsertUser, usuario, transaction);

                        var sqlInsertSubscription = @"
                            INSERT INTO detalles_suscripciones (usuario_id, tipo_suscripcion, fecha_inicio, fecha_final, precio, operaciones_realizadas)
                            VALUES (@UsuarioId, 'basico', GETDATE(), DATEADD(YEAR, 1, GETDATE()), 0.00, 0)";
                        
                        connection.Execute(sqlInsertSubscription, new { UsuarioId = usuario.Id }, transaction);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public Usuario ObtenerUsuarioPorId(int idUsuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<Usuario>(
                    "SELECT * FROM Usuarios WHERE Id = @IdUsuario",
                    new { IdUsuario = idUsuario });
            }
        }

        public IEnumerable<Usuario> ObtenerUsuarios()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Query<Usuario>("SELECT * FROM Usuarios");
            }
        }
    }
}
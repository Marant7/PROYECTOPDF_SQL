// PROYECTOPDF/Controllers/SuscripcionController.cs
using Microsoft.AspNetCore.Mvc;
using NegocioPDF.Models;
using NegocioPDF.Repositories;
using System.Security.Claims;

namespace PROYECTOPDF.Controllers
{
    public class SuscripcionController : Controller
    {
        private readonly DetalleSuscripcionRepository _detalleSuscripcionRepository;

        public SuscripcionController(DetalleSuscripcionRepository detalleSuscripcionRepository)
        {
            _detalleSuscripcionRepository = detalleSuscripcionRepository;
        }

        // Acción para mostrar el menú principal
        public IActionResult MenuPrincipal()
        {
            // Obtener el ID del usuario logueado
            var usuarioId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Obtener el detalle de suscripción del usuario
            var detalleSuscripcion = _detalleSuscripcionRepository.ObtenerPorUsuarioId(usuarioId);

            if (detalleSuscripcion == null)
            {
                TempData["Mensaje"] = "No se encontró la suscripción para el usuario.";
                return RedirectToAction("Login", "Auth");
            }

            // Pasar el nombre, el tipo de suscripción y las operaciones realizadas a la vista
            ViewBag.NombreUsuario = detalleSuscripcion.Usuario.Nombre;
            ViewBag.TipoSuscripcion = !string.IsNullOrEmpty(detalleSuscripcion.tipo_suscripcion) 
                ? detalleSuscripcion.tipo_suscripcion 
                : "No especificado";
            ViewBag.OperacionesRealizadas = detalleSuscripcion.operaciones_realizadas;
            ViewBag.MaxOperaciones = detalleSuscripcion.tipo_suscripcion.Equals("basico", StringComparison.OrdinalIgnoreCase) ? 5 : (int?)null;

            return View();
                    }// PROYECTOPDF/Controllers/SuscripcionController.cs
            public IActionResult ComprarPremium()
            {
                            // Obtener el ID del usuario logueado
                var usuarioId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Obtener la fecha de inicio (hoy) y la fecha final (30 días después)
                var fechaInicio = DateTime.Now;
                var fechaFinal = fechaInicio.AddDays(30);

                // Pasar los datos a la vista
                ViewBag.FechaInicio = fechaInicio.ToString("yyyy-MM-dd");
                ViewBag.FechaFinal = fechaFinal.ToString("yyyy-MM-dd");
                ViewBag.Precio = 50.00m; // Precio fijo de la suscripción

                return View();
            }
                
    
                            [HttpPost]
            public IActionResult ConfirmarCompra()
            {
                // Obtener el ID del usuario logueado
                var usuarioId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Actualizar la suscripción a 'premium'
                var suscripcion = new DetalleSuscripcion
                {
                    UsuarioId = usuarioId, // Asigna el usuarioId a la propiedad UsuarioId
                    tipo_suscripcion = "premium",
                    fecha_inicio = DateTime.Now,
                    fecha_final = DateTime.Now.AddDays(30),
                    precio = 50.00m,
                    operaciones_realizadas = 0
                };

                _detalleSuscripcionRepository.ActualizarSuscripcion(suscripcion);

                TempData["Mensaje"] = "¡Felicitaciones! Has comprado la suscripción Premium.";
                return RedirectToAction("MenuPrincipal");
            }
         }
}   






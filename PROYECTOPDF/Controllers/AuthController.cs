using Microsoft.AspNetCore.Mvc; // Para trabajar con Controladores y MVC
using Microsoft.AspNetCore.Authentication; // Para manejar la autenticación
using Microsoft.AspNetCore.Authentication.Cookies; // Para manejar la autenticación basada en cookies
using System.Security.Claims; // Para trabajar con los claims de autenticación
using NegocioPDF.Repositories; // Referencia a tu repositorio de usuarios
using NegocioPDF.Models; // Referencia al modelo de Usuario
using System.Collections.Generic; // Para trabajar con listas y colecciones
using System; // Para trabajar con tipos básicos como DateTimeOffset

namespace PROYECTOPDF.Controllers
{
    public class AuthController : Controller
    {
        private readonly UsuarioRepository _usuarioRepository;

        public AuthController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string correo, string password)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "El correo y la contraseña son requeridos");
                return View();
            }

            var usuario = _usuarioRepository.Login(correo, password);

            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Email, usuario.Correo)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                TempData["Mensaje"] = $"Bienvenido, {usuario.Nombre}!";
                return RedirectToAction("MenuPrincipal", "Suscripcion");
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos");
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Mensaje"] = "Has cerrado sesión correctamente.";
            return RedirectToAction("Login");
        }
    }
}
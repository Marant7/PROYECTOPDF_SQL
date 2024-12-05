using Microsoft.AspNetCore.Mvc; // Para el uso de Controller y IActionResult
using NegocioPDF.Repositories;  // Para UsuarioRepository
using NegocioPDF.Models;        // Para el modelo Usuario


public class RegistrationController : Controller
{
    private readonly UsuarioRepository _usuarioRepository;

    public RegistrationController(UsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet]
    public IActionResult Registrarse()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Registrarse(Usuario usuario)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _usuarioRepository.RegistrarUsuario(usuario);
                TempData["Mensaje"] = "Usuario registrado con éxito. Ahora puedes iniciar sesión.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }
        return View(usuario);
    }
}

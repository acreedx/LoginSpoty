using LoginAplicacion.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Nodes;
using Microsoft.IdentityModel.Tokens;
using LoginAplicacion.Models;
using LoginAplicacion.Model;
using System.Globalization;

namespace LoginAplicacion.Controllers
{
    public class LoginController : Controller
    {
        public string siteKey = "6Ldzq6MpAAAAAA7oW1ienKWNZggfMpeVgS-Ze3RW";
        //6LeAt6MpAAAAAC_TekX1ZHMCxs0430fZPJVwAVjf
        public string secretKey = "6Ldzq6MpAAAAALrgV7K0l4dHvRyv4orMjIgfwdEK";
        //6LeAt6MpAAAAAL-25GwboNoOchtKebtoUH8vzcvM
        public readonly SpotifyBdContext _contexto = new SpotifyBdContext();
        PasswordValidator validator = new PasswordValidator();

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.SiteKey = siteKey;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Usuario usuario)
        {
            try
            {
                var response = Request.Form["g-recaptcha-response"];
                var client = new WebClient();
                var result = client
                    .DownloadString(
                        string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response)
                    );
                var obj = JsonObject.Parse(result);
                var status = (bool)obj["success"];
                if (!status)
                {
                    ViewBag.SiteKey = siteKey;
                    ViewBag.Captcha = "Error al completar el recaptcha";
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.SiteKey = siteKey;
                ViewBag.Captcha = "Error al completar el recaptcha";
                return View();
            }
            if (usuario.NombreUsuario.IsNullOrEmpty() || usuario.Password.IsNullOrEmpty())
            {
                ViewBag.SiteKey = siteKey;
                ViewBag.Error = "Debe completar los campos";
                return View();
            }
            Usuario usuario1 = _contexto.Usuarios
                .Where(e => e.NombreUsuario == usuario.NombreUsuario)
                .FirstOrDefault();
            if (usuario1 != null)
            {
                if (usuario1.Estado != 0)
                {
                    if (usuario1.Password == usuario.Password)
                    {
                        if (usuario1.Estado == 2)
                        {
                            return RedirectToAction("ChangePassword", "Login");
                        }
                        usuario1.IntentosPassword = 0;
                        var claims = new List<Claim> {
                            new Claim(ClaimTypes.Name, usuario1.NombreCompleto),
                            new Claim(ClaimTypes.Surname, usuario1.NombreUsuario)
                        };
                        var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        _contexto.Update(usuario1);
                        _contexto.SaveChanges();
                        AudiUsuario audi = new AudiUsuario();
                        audi.Id = 0;
                        audi.UsuariosIdUsuario = usuario1.Id;
                        audi.FecUltAct = DateTime.Now;
                        audi.AccionRealizada = "Inicio de sesión";
                        _contexto.Add(audi);
                        _contexto.SaveChanges();
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        if (usuario1.IntentosPassword >= 3)
                        {
                            usuario1.Estado = 0;
                            ViewBag.SiteKey = siteKey;
                            ViewBag.Bloqueado = "Usuario bloqueado, hable con un administrador";
                            _contexto.Update(usuario1);
                            _contexto.SaveChanges();
                            AudiUsuario audi = new AudiUsuario();
                            audi.Id = 0;
                            audi.UsuariosIdUsuario = usuario1.Id;
                            audi.FecUltAct = DateTime.Now;
                            audi.AccionRealizada = "Bloqueo de cuenta";
                            _contexto.Add(audi);
                            _contexto.SaveChanges();
                            return View();
                        }
                        usuario1.IntentosPassword++;
                        _contexto.Update(usuario1);
                        _contexto.SaveChanges();
                        ViewBag.SiteKey = siteKey;
                        ViewBag.Error = "Nombre de usuario o clave incorrectos";
                        return View();
                    }
                }
                if (usuario1.Estado == 0)
                {
                    ViewBag.SiteKey = siteKey;
                    ViewBag.Bloqueado = "Usuario bloqueado, hable con un administrador";
                    return View();
                }
            }
            ViewBag.SiteKey = siteKey;
            ViewBag.Error = "Nombre de usuario o clave incorrectos";
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(UsuarioCreacion usuario)
        {
            if (usuario.NombreUsuario.IsNullOrEmpty() || usuario.Password.IsNullOrEmpty() || usuario.NuevoPassword.IsNullOrEmpty() || usuario.ConfirmarNuevoPassword.IsNullOrEmpty())
            {
                ViewBag.Error = "Complete los campos";
                return View();
            }
            if (usuario.NuevoPassword != usuario.ConfirmarNuevoPassword)
            {
                ViewBag.Error = "El nuevo password y la confirmacion deben coincidir"; 
                return View();
            }
            if (usuario.NuevoPassword == usuario.Password)
            {
                ViewBag.Error = "El nuevo password debe ser distinto al password actual";
                return View();
            }
            if (!validator.EsPasswordSegura(usuario.NuevoPassword))
            {
                ViewBag.Error = "La contraseña debe contener al menos un número, una letra y 10 caracteres";
                return View();
            }
            Usuario usuario1 = _contexto.Usuarios
                .Where(e => e.NombreUsuario == usuario.NombreUsuario)
                .FirstOrDefault();
            if (usuario1 != null)
            {
                 if (usuario1.Password == usuario.Password)
                 {
                     if (usuario1.Estado == 0)
                     {
                         ViewBag.SiteKey = siteKey;
                         ViewBag.Bloqueado = "Usuario bloqueado, hable con un administrador";
                         return View();
                     }
                     usuario1.IntentosPassword = 0;
                     usuario1.Estado = 1;
                     usuario1.Password = usuario.NuevoPassword;
                     usuario1.FechaDeActualizacion = DateTime.Now;
                     _contexto.Update(usuario1);
                     _contexto.SaveChanges();
                    AudiUsuario audi = new AudiUsuario();
                    audi.Id = 0;
                    audi.UsuariosIdUsuario = usuario1.Id;
                    audi.FecUltAct = DateTime.Now;
                    audi.AccionRealizada = "Cambio de contraseña";
                    _contexto.Add(audi);
                    _contexto.SaveChanges();
                    return RedirectToAction("Index", "Login");
                 }
                 else
                 {
                     if (usuario1.IntentosPassword >= 3)
                     {
                         usuario1.Estado = 0;
                         ViewBag.Bloqueado = "Usuario bloqueado, hable con un administrador";
                         _contexto.Update(usuario1);
                         _contexto.SaveChanges();
                         return View();
                     }
                     usuario1.IntentosPassword++;
                     _contexto.Update(usuario1);
                     _contexto.SaveChanges();
                     ViewBag.Error = "Nombre de usuario o clave incorrectos";
                     return View();
                 }
            }
            ViewBag.Error = "Nombre de usuario o clave incorrectos";
            return View();
        }
        [HttpPost]
        public IActionResult CrearCuenta(Usuario usuario)
        {
            if (_contexto.Usuarios.Any(e => e.NombreUsuario == usuario.NombreUsuario))
            {
                ViewBag.Error = "Ya existe un usuario con ese nombre";
                return View("Index");
            }
            usuario.Id = 0;
            usuario.FechaDeActualizacion = DateTime.Now;
            usuario.IntentosPassword = 0;
            usuario.Estado = 2;
            _contexto.Add(usuario);
            _contexto.SaveChanges();
            return RedirectToAction("Index", "Login");
        }
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}

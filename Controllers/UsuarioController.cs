using Microsoft.AspNetCore.Mvc;
using webapi.Model;

namespace webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        public static List<Usuario> Usuarios()
        {
            return new List<Usuario>{
                new Usuario{Nome = "ANGELO", Id = 1, Email = "angelonoda@gmail.com"}
            };
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(Usuarios());
        }
        [HttpPost]
        public IActionResult Post(Usuario usuario)
        {
            var usuarios = Usuarios();
            usuarios.Add(usuario);
            return Ok(usuarios);
        }
    }
}
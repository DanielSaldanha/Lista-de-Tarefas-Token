using Microsoft.AspNetCore.Mvc;
using CloneListaTarefas.TokenPaste;

namespace CloneListaTarefas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)// Recebe as credenciais do usuário no corpo da requisição.
        {
            // Validação simples de usuário e senha (substitua por validação real)
            if (model.Username == "admin" && model.Password == "password")// verificador
            {
                var token = TokenService.GenerateToken(model.Username); // Gera um token JWT usando o TokenService.
                return Ok(new { token });
            }
            return Unauthorized("Usuário ou senha inválidos.");
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}


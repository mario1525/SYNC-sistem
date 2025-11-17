using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;
using Entity;

namespace Controller.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UsersCredentialLogical _usersCredentialLogical;

        public AuthController(UsersCredentialLogical usersCredentialLogical)
        {
            _usersCredentialLogical = usersCredentialLogical;
        }

        // POST: /api/Auth
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Token>> Login([FromBody] Login login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Usuario) || string.IsNullOrWhiteSpace(login.Contrasenia))
            {
                return BadRequest("Usuario y contraseña son requeridos.");
            }

            var token = await _usersCredentialLogical.Login(login);
            if (token == null || string.IsNullOrWhiteSpace(token.token))
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}
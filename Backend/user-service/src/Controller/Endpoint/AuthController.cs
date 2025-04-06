using Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers.Endpoint
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UsersCredentialLogical _UsConLogical;

        public AuthController(UsersCredentialLogical usConLogical)
        {
            _UsConLogical = usConLogical;
        }
        // POST: api/auth        
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            Token token = await _UsConLogical.Login(model);


            if (token != null)
            {
                return Ok(token);
            }

            return Unauthorized("Credenciales inválidas");
        }
    }
}

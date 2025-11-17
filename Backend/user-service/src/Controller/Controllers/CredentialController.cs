using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Services;

namespace Controller.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CredentialController : ControllerBase
    {
        private readonly UsersCredentialLogical _usersCredentialLogical;

        public CredentialController(UsersCredentialLogical usersCredentialLogical)
        {
            _usersCredentialLogical = usersCredentialLogical;
        }

        // GET: /api/Credential/{id}
        // Valida si un usuario (id de Users) tiene credenciales creadas.
        [HttpGet("{id}")]
        public async Task<ActionResult<bool>> GetValidCredential(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("El id de usuario es requerido.");

            var valid = await _usersCredentialLogical.ValidCredential(id);
            return Ok(valid);
        }

        // POST: /api/Credential
        // Crea credenciales para un usuario
        [HttpPost]
        public async Task<ActionResult<Mensaje>> Create([FromBody] UsuarioCredential credential)
        {
            if (credential == null
                || string.IsNullOrWhiteSpace(credential.Usuario)
                || string.IsNullOrWhiteSpace(credential.Contrasenia)
                || string.IsNullOrWhiteSpace(credential.IdUser))
            {
                return BadRequest("Usuario, Contrasenia e IdUser son requeridos.");
            }

            var msg = await _usersCredentialLogical.CreateUsers(credential);
            return Ok(msg);
        }

        // PUT: /api/Credential/{id}
        // Actualiza credenciales por Id de credencial
        [HttpPut("{id}")]
        public ActionResult<Mensaje> Update(string id, [FromBody] UsuarioCredential credential)
        {
            if (credential == null)
                return BadRequest("Cuerpo inválido.");

            credential.Id = id;
            var msg = _usersCredentialLogical.UpdateUsers(credential);
            return Ok(msg);
        }

        // DELETE: /api/Credential/{id}
        // Elimina credenciales por Id de credencial
        [HttpDelete("{id}")]
        public ActionResult<Mensaje> Delete(string id)
        {
            var msg = _usersCredentialLogical.DeleteUsers(id);
            return Ok(msg);
        }
    }
}
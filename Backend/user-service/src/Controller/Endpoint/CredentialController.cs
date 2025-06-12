using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Controllers.Endpoint
{
    [Route("api/Credential")]
    [ApiController]
    public class CredentialController : ControllerBase
    {
        private readonly UsersCredentialLogical _UserLogical;

        public CredentialController(UsersCredentialLogical userLogical)
        {
            _UserLogical = userLogical;
        }

        // GET: api/Credential/id
        // validar si el Users ya tiene credenciales asociadas
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Root,Supervisor,Mecanico")]
        public async Task<bool> Get(string id)
        {
            return await _UserLogical.ValidCredential(id);
        }

        // POST api/Credential
        [HttpPost]
        [Authorize(Roles = "Admin,Root")]
        public Task<Mensaje> Post([FromBody] UsuarioCredential value)
        {
           return _UserLogical.CreateUsers(value);
        }

        // PUT api/Credential/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Root,Supervisor,Mecanico")]
        public Mensaje Put(string id, [FromBody] UsuarioCredential value)
        {
            value.Id = id;
            return _UserLogical.UpdateUsers(value);
        }

        // DELETE api/Credential/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Root")]
        public Mensaje Delete(string id)
        {
            return _UserLogical.DeleteUsers(id);
        }
    }
}

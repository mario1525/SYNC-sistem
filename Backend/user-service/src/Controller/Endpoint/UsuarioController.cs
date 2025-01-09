using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Controllers.Endpoint
{
    [Route("api/Usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioLogical _UserLogical;

        public UsuarioController(UsuarioLogical usuariological)
        {
            _UserLogical = usuariological;
        }
        // GET: api/Usuario
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<List<Usuario>> Get()
        {
            return await _UserLogical.GetUsuarios();
        }

        // GET api/Usuario/Compania/5
        [HttpGet("Compania/{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public async Task<List<Usuario>> GetComp(string id)
        {
            return await _UserLogical.GetUsuariosComp(id);
        }

        // GET api/Usuario/Compania/Supervisor/5
        [HttpGet("Compania/Supervisor/{id}")]
        [Authorize(Roles = "Admin,Admin-Compania,Cordinador")]
        public async Task<List<Usuario>> GetSupervisor(string id)
        {
            return await _UserLogical.GetUsuariosSuperv(id);
        }

        // GET api/Usuario/Compania/Operativo/5
        [HttpGet("Compania/Operativo/{id}")]
        [Authorize(Roles = "Admin,Admin-Compania,Cordinador")]
        public async Task<List<Usuario>> GetOperativo(string id)
        {
            return await _UserLogical.GetUsuariosOperativo(id);
        }

        // GET api/Usuario/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public async Task<List<Usuario>> Get(string id)
        {
            return await _UserLogical.GetUsuario(id) ;
        }

        // POST api/Usuario
        [HttpPost]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public Mensaje Post([FromBody] Usuario value)
        {
            return _UserLogical.CreateUsuario(value);
        }

        // PUT api/Usuario/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public  Mensaje Put(string id, [FromBody] Usuario value)
        {
            value.Id = id;
            return  _UserLogical.UpdateUsuario(value);
        }

        // DELETE api/Usuario/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public Mensaje Delete(string id)
        {
            return _UserLogical.DeleteUsuario(id);
        }
    }
}

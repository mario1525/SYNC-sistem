using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Controller.Endpoint
{
    [Route("api/Usuario/Proyecto")]
    [ApiController]
    public class UserProyectController : ControllerBase
    {
        private readonly UserProyectLogical _UserLogical;

        public UserProyectController(UserProyectLogical usuariological)
        {
            _UserLogical = usuariological;
        }

        // GET api/Usuario/Proyecto/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public async Task<List<UserProyect>> Gets(string id)
        {
            return await _UserLogical.Gets(id);
        }

        // POST api/Usuario/Proyecto
        [HttpPost]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public Mensaje Post([FromBody] UserProyect value)
        {
            return _UserLogical.Create(value);
        }

        // PUT api/Usuario/Proyecto/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public Mensaje Put(string id, [FromBody] UserProyect value)
        {
            value.Id = id;
            return _UserLogical.Update(value);
        }

        // DELETE api/Usuario/Proyecto/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public Mensaje Delete(string id)
        {
            return _UserLogical.Delete(id);
        }
    }
}

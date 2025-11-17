using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Services;

namespace Controller.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UsersLogical _usersLogical;

        public UsersController(UsersLogical usersLogical)
        {
            _usersLogical = usersLogical;
        }

        // GET: /api/Users
        // Permite filtrar por query string opcionalmente (ej: ?IdComp=123&Nombre=Juan)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers(
            [FromQuery] string? Id = null,
            [FromQuery] long? Identificacion = null,
            [FromQuery] string? Nombre = null,
            [FromQuery] string? Apellido = null,
            [FromQuery] string? Correo = null,
            [FromQuery] string? IdComp = null,
            [FromQuery] string? IdEsp = null,
            [FromQuery] string? IdCuad = null,
            [FromQuery] string? Cargo = null,
            [FromQuery] string? Rol = null
        )
        {
            var criteria = new Users
            {
                Id = Id,
                Identificacion = Identificacion,
                Nombre = Nombre,
                Apellido = Apellido,
                Correo = Correo,
                IdComp = IdComp,
                IdEsp = IdEsp,
                IdCuad = IdCuad,
                Cargo = Cargo,
                Rol = Rol
            };

            var result = await _usersLogical.GetUser(criteria);
            return Ok(result);
        }

        // POST: /api/Users
        [HttpPost]
        public ActionResult<Mensaje> Create([FromBody] Users user)
        {
            if (user == null)
                return BadRequest("Cuerpo inválido.");

            var msg = _usersLogical.CreateUsers(user);
            return CreatedAtAction(nameof(GetUsers), new { Id = msg.mensaje }, msg);
        }

        // PUT: /api/Users/{id}
        [HttpPut("{id}")]
        public ActionResult<Mensaje> Update(string id, [FromBody] Users user)
        {
            if (user == null)
                return BadRequest("Cuerpo inválido.");

            user.Id = id;
            var msg = _usersLogical.UpdateUsers(user);
            return Ok(msg);
        }

        // DELETE: /api/Users/{id}
        [HttpDelete("{id}")]
        public ActionResult<Mensaje> Delete(string id)
        {
            var msg = _usersLogical.DeleteUsers(id);
            return Ok(msg);
        }

        // PUT: /api/Users/{id}/state?estado=1
        [HttpPut("{id}/state")]
        public ActionResult<Mensaje> ChangeState(string id, [FromQuery] int estado)
        {
            var msg = _usersLogical.ActiveUsers(id, estado);
            return Ok(msg);
        }
    }
}
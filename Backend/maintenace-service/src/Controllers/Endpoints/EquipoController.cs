using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoints
{
    [Route("api/Equipo")]
    [ApiController]
    public class EquipoController : ControllerBase
    {
        private readonly EquipoLogical _equipoLogical;

        public EquipoController(EquipoLogical equipoLogical)
        {
            _equipoLogical = equipoLogical;
        }

        // GET: api/Equipo
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Equipo>>> Get([FromQuery] string Id, string Nombre, string IdComp, string Marca, string NSerie, bool Estado)
        {
            var result = await _equipoLogical.GetEquipos(Id, Nombre, IdComp, Marca, NSerie, Estado);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron equipos.");

            return Ok(result);
        }

        // POST api/Equipo
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] EquipoC equipo)
        {
            if (equipo == null)
                return BadRequest("Los datos del equipo no pueden estar vacíos.");

            var result = await _equipoLogical.CreateEquipo(equipo);
            return Ok(result);
        }

        // PUT api/Equipo/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] EquipoC equipo)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            equipo.Id = id;
            var result = await _equipoLogical.UpdateEquipo(equipo);
            return Ok(result);
        }

        // DELETE api/Equipo/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _equipoLogical.DeleteEquipo(id);
            return Ok(result);
        }

        // PATCH api/Equipo/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] int estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _equipoLogical.ActiveEquipo(id, estado);
            return Ok(result);
        }
    }
}

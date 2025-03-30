using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoints
{
    [Route("api/GuiaEquipo")]
    [ApiController]
    public class GuiaEquipoController : ControllerBase
    {
        private readonly GuiaEquipoLogical _guiaEquipoLogical;

        public GuiaEquipoController(GuiaEquipoLogical guiaEquipoLogical)
        {
            _guiaEquipoLogical = guiaEquipoLogical;
        }

        // GET: api/GuiaEquipo
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Guia_Equipo>>> Get([FromQuery] Guia_Equipo guiaEquipo)
        {
            var result = await _guiaEquipoLogical.GetGuiaEquipos(guiaEquipo);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron relaciones Guía-Equipo.");

            return Ok(result);
        }

        // POST api/GuiaEquipo
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] Guia_Equipo guiaEquipo)
        {
            if (guiaEquipo == null)
                return BadRequest("Los datos de la relación Guía-Equipo no pueden estar vacíos.");

            var result = await _guiaEquipoLogical.CreateGuiaEquipo(guiaEquipo);
            return Ok(result);
        }

        // PUT api/GuiaEquipo/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] Guia_Equipo guiaEquipo)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            guiaEquipo.Id = id;
            var result = await _guiaEquipoLogical.UpdateGuiaEquipo(guiaEquipo);
            return Ok(result);
        }

        // DELETE api/GuiaEquipo/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _guiaEquipoLogical.DeleteGuiaEquipo(id);
            return Ok(result);
        }

        // PATCH api/GuiaEquipo/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] int estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _guiaEquipoLogical.ActiveGuiaEquipo(id, estado);
            return Ok(result);
        }
    }
}

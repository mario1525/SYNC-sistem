using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoint
{
    [Route("api/ActividadEquipo")]
    [ApiController]
    public class ActividadEquipoController : ControllerBase
    {
        private readonly ActividadEquipoLogical _actividadEquipoLogical;

        public ActividadEquipoController(ActividadEquipoLogical actividadEquipoLogical)
        {
            _actividadEquipoLogical = actividadEquipoLogical;
        }

        // GET: api/ActividadEquipo
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ActividadEquipo>>> Get([FromQuery] string id)
        {
            var result = await _actividadEquipoLogical.GetActividadEquipo(id);
            if (result == null )
                return NotFound("No se encontraron relaciones Actividad-Equipo.");

            return Ok(result);
        }

        // POST api/ActividadEquipo
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] ActividadEquipo actividadEquipo)
        {
            if (actividadEquipo == null)
                return BadRequest("Los datos de la relación Actividad-Equipo no pueden estar vacíos.");

            var result = await _actividadEquipoLogical.CreateActividadEquipo(actividadEquipo);
            return Ok(result);
        }

        // PUT api/ActividadEquipo/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] ActividadEquipo actividadEquipo)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            actividadEquipo.Id = id;
            var result = await _actividadEquipoLogical.UpdateActividadEquipo(actividadEquipo);
            return Ok(result);
        }

        // DELETE api/ActividadEquipo/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _actividadEquipoLogical.DeleteActividadEquipo(id);
            return Ok(result);
        }

        // PATCH api/ActividadEquipo/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] int estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _actividadEquipoLogical.ActiveActividadEquipo(id, estado);
            return Ok(result);
        }
    }
}

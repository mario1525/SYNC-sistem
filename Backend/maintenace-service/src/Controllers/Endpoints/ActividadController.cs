using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoint
{
    [Route("api/Actividad")]
    [ApiController]
    public class ActividadController : ControllerBase
    {
        private readonly ActividadLogical _actividadLogical;

        public ActividadController(ActividadLogical actividadLogical)
        {
            _actividadLogical = actividadLogical;
        }

        // GET: api/Actividad
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Actividad>>> Get([FromQuery] Actividad actividad)
        {
            var result = await _actividadLogical.GetActividads(actividad);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron actividades.");

            return Ok(result);
        }

        // POST api/Actividad
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] Actividad actividad)
        {
            if (actividad == null)
                return BadRequest("Los datos de la actividad no pueden estar vacíos.");

            var result = await _actividadLogical.CreateActividad(actividad);
            return Ok(result);
        }

        // PUT api/Actividad/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] Actividad actividad)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            actividad.Id = id;
            var result = await _actividadLogical.UpdateActividad(actividad);
            return Ok(result);
        }

        // DELETE api/Actividad/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _actividadLogical.DeleteActividad(id);
            return Ok(result);
        }

        // PATCH api/Actividad/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] int estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _actividadLogical.ActiveActividad(id, estado);
            return Ok(result);
        }
    }
}
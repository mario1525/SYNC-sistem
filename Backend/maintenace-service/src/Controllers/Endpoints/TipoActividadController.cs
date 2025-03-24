using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoints
{
    [Route("api/TipoActividad")]
    [ApiController]
    public class TipoActividadController : ControllerBase
    {
        private readonly TipoActividadLogical _tipoActividadLogical;

        public TipoActividadController(TipoActividadLogical tipoActividadLogical)
        {
            _tipoActividadLogical = tipoActividadLogical;
        }

        // GET: api/TipoActividad
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<TipoAct>>> Get([FromQuery] TipoAct tipoActividad)
        {
            var result = await _tipoActividadLogical.GetTipoActividades(tipoActividad);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron tipos de actividad.");

            return Ok(result);
        }

        // POST api/TipoActividad
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] TipoAct tipoActividad)
        {
            if (tipoActividad == null)
                return BadRequest("Los datos del tipo de actividad no pueden estar vacíos.");

            var result = await _tipoActividadLogical.CreateTipoActividad(tipoActividad);
            return Ok(result);
        }

        // PUT api/TipoActividad/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] TipoAct tipoActividad)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            tipoActividad.Id = id;
            var result = await _tipoActividadLogical.UpdateTipoActividad(tipoActividad);
            return Ok(result);
        }

        // DELETE api/TipoActividad/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _tipoActividadLogical.DeleteTipoActividad(id);
            return Ok(result);
        }

        // PATCH api/TipoActividad/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] int estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _tipoActividadLogical.ActiveTipoActividad(id, estado);
            return Ok(result);
        }
    }
}

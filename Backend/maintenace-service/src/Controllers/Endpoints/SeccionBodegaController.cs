using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoint
{
    [Route("api/SeccionBodega")]
    [ApiController]
    public class SeccionBodegaController : ControllerBase
    {
        private readonly SeccionBodegaLogical _seccionBodegaLogical;

        public SeccionBodegaController(SeccionBodegaLogical seccionBodegaLogical)
        {
            _seccionBodegaLogical = seccionBodegaLogical;
        }

        // GET: api/SeccionBodega
        [HttpGet]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<List<SeccionBodega>>> Get([FromQuery] SeccionBodega seccionBodega)
        {
            var result = await _seccionBodegaLogical.GetSeccionBodegas(seccionBodega);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron secciones de bodega.");

            return Ok(result);
        }

        // POST api/SeccionBodega
        [HttpPost]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] SeccionBodega seccionBodega)
        {
            if (seccionBodega == null)
                return BadRequest("Los datos de la sección de bodega no pueden estar vacíos.");

            var result = await _seccionBodegaLogical.CreateSeccionBodega(seccionBodega);
            return Ok(result);
        }

        // PUT api/SeccionBodega/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] SeccionBodega seccionBodega)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            seccionBodega.Id = id;
            var result = await _seccionBodegaLogical.UpdateSeccionBodega(seccionBodega);
            return Ok(result);
        }

        // DELETE api/SeccionBodega/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _seccionBodegaLogical.DeleteSeccionBodega(id);
            return Ok(result);
        }

        // PATCH api/SeccionBodega/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] bool estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _seccionBodegaLogical.ActiveSeccionBodega(id, estado);
            return Ok(result);
        }
    }
}
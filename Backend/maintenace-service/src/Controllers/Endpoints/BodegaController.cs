using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoint
{
    [Route("api/Bodega")]
    [ApiController]
    public class BodegaController : ControllerBase
    {
        private readonly BodegaLogical _bodegaLogical;

        public BodegaController(BodegaLogical bodegaLogical)
        {
            _bodegaLogical = bodegaLogical;
        }

        // GET: api/Bodega
        [HttpGet]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<List<Bodega>>> Get([FromQuery] Bodega bodega)
        {
            var result = await _bodegaLogical.GetBodegas(bodega);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron bodegas.");

            return Ok(result);
        }

        // POST api/Bodega
        [HttpPost]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] Bodega bodega)
        {
            if (bodega == null)
                return BadRequest("Los datos de la bodega no pueden estar vacíos.");

            var result = await _bodegaLogical.CreateBodega(bodega);
            return Ok(result);
        }

        // PUT api/Bodega/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] Bodega bodega)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            bodega.Id = id;
            var result = await _bodegaLogical.UpdateBodega(bodega);
            return Ok(result);
        }

        // DELETE api/Bodega/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _bodegaLogical.DeleteBodega(id);
            return Ok(result);
        }

        // PATCH api/Bodega/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] bool estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _bodegaLogical.ActiveBodega(id, estado);
            return Ok(result);
        }
    }
}
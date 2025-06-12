using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoints
{
    [Route("api/Proced")]
    [ApiController]
    public class ProcedController : ControllerBase
    {
        private readonly ProcedLogical _procedLogical;

        public ProcedController(ProcedLogical procedLogical)
        {
            _procedLogical = procedLogical;
        }

        // GET: api/Proced
        [HttpGet]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<List<Proced>>> Get([FromQuery] Proced proced)
        {
            var result = await _procedLogical.GetProceds(proced);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron procedimientos.");

            return Ok(result);
        }

        // POST api/Proced
        [HttpPost]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] Proced proced)
        {
            if (proced == null)
                return BadRequest("Los datos del procedimiento no pueden estar vacíos.");

            var result = await _procedLogical.CreateProced(proced);
            return Ok(result);
        }

        // PUT api/Proced/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] Proced proced)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            proced.Id = id;
            var result = await _procedLogical.UpdateProced(proced);
            return Ok(result);
        }

        // DELETE api/Proced/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _procedLogical.DeleteProced(id);
            return Ok(result);
        }

        // PATCH api/Proced/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] int estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _procedLogical.ActiveProced(id, estado);
            return Ok(result);
        }
    }
}

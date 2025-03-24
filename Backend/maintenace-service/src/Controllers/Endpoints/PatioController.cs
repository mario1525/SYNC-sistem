using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoint
{
    [Route("api/Patio")]
    [ApiController]
    public class PatioController : ControllerBase
    {
        private readonly PatioLogical _patioLogical;

        public PatioController(PatioLogical patioLogical)
        {
            _patioLogical = patioLogical;
        }

        // GET: api/Patio
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Patio>>> Get([FromQuery] Patio patio)
        {
            var result = await _patioLogical.GetPatios(patio);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron patios.");

            return Ok(result);
        }

        // POST api/Patio
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] Patio patio)
        {
            if (patio == null)
                return BadRequest("Los datos del patio no pueden estar vacíos.");

            var result = await _patioLogical.CreatePatio(patio);
            return Ok(result);
        }

        // PUT api/Patio/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] Patio patio)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            patio.Id = id;
            var result = await _patioLogical.UpdatePatio(patio);
            return Ok(result);
        }

        // DELETE api/Patio/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _patioLogical.DeletePatio(id);
            return Ok(result);
        }

        // PATCH api/Patio/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] bool estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _patioLogical.ActivePatio(id, estado);
            return Ok(result);
        }
    }
}
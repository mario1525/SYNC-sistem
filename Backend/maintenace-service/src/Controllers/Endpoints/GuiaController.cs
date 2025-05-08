using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoints
{
    [Route("api/Guia")]
    [ApiController]
    public class GuiaController : ControllerBase
    {
        private readonly GuiaLogical _guiaLogical;

        public GuiaController(GuiaLogical guiaLogical)
        {
            _guiaLogical = guiaLogical;
        }

        // GET: api/Guia
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Guia>>> Gets([FromQuery] Guia guia)
        {
            var result = await _guiaLogical.GetGuias(guia);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron guías.");

            return Ok(result);
        }

        // GET: api/Guia/:5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Guia>> Get(string id)
        {
            var result = await _guiaLogical.GetGuiaById(id);
            if (result == null)
                return NotFound("No se encontraron guías.");

            return Ok(result);
        }

        // POST api/Guia
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] Guia guia)
        {
            if (guia == null)
                return BadRequest("Los datos de la guía no pueden estar vacíos.");

            var result = await _guiaLogical.CreateGuia(guia);
            return Ok(result);
        }

        // PUT api/Guia/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] Guia guia)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            guia.Id = id;
            var result = await _guiaLogical.UpdateGuia(guia);
            return Ok(result);
        }

        // DELETE api/Guia/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _guiaLogical.DeleteGuia(id);
            return Ok(result);
        }

        // PATCH api/Guia/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] int estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _guiaLogical.ActiveGuia(id, estado);
            return Ok(result);
        }
    }
}

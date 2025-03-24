using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoints
{
    [Route("api/Esp")]
    [ApiController]
    public class EspController : ControllerBase
    {
        private readonly EspLogical _espLogical;

        public EspController(EspLogical espLogical)
        {
            _espLogical = espLogical;
        }

        // GET: api/Esp
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Esp>>> Get([FromQuery] Esp esp)
        {
            var result = await _espLogical.GetEsps(esp);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron especialidades.");

            return Ok(result);
        }

        // POST api/Esp
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] Esp esp)
        {
            if (esp == null)
                return BadRequest("Los datos de la especialidad no pueden estar vacíos.");

            var result = await _espLogical.CreateEsp(esp);
            return Ok(result);
        }

        // PUT api/Esp/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] Esp esp)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            esp.Id = id;
            var result = await _espLogical.UpdateEsp(esp);
            return Ok(result);
        }

        // DELETE api/Esp/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _espLogical.DeleteEsp(id);
            return Ok(result);
        }

        // PATCH api/Esp/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] int estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _espLogical.ActiveEsp(id, estado);
            return Ok(result);
        }
    }
}

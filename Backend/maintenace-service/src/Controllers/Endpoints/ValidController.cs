using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoints
{
    [Route("api/Valid")]
    [ApiController]
    public class ValidController : ControllerBase
    {
        private readonly ValidLogical _validLogical;

        public ValidController(ValidLogical validLogical)
        {
            _validLogical = validLogical;
        }

        // GET: api/Valid
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Valid>>> Get([FromQuery] Valid valid)
        {
            var result = await _validLogical.GetValids(valid);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron validaciones.");

            return Ok(result);
        }

        // POST api/Valid
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] Valid valid)
        {
            if (valid == null)
                return BadRequest("Los datos de la validación no pueden estar vacíos.");

            var result = await _validLogical.CreateValid(valid);
            return Ok(result);
        }

        // PUT api/Valid/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] Valid valid)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            valid.Id = id;
            var result = await _validLogical.UpdateValid(valid);
            return Ok(result);
        }

        // DELETE api/Valid/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _validLogical.DeleteValid(id);
            return Ok(result);
        }

        // PATCH api/Valid/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] int estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _validLogical.ActiveValid(id, estado);
            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoint
{
    [Route("api/AreaFuncional")]
    [ApiController]
    public class AreaFuncionalController : ControllerBase
    {
        private readonly AreaFuncionalLogical _areaFuncionalLogical;

        public AreaFuncionalController(AreaFuncionalLogical areaFuncionalLogical)
        {
            _areaFuncionalLogical = areaFuncionalLogical;
        }

        // GET: api/AreaFuncional
        [HttpGet]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<List<AreaFuncional>>> Get([FromQuery] AreaFuncional areaFuncional)
        {
            var result = await _areaFuncionalLogical.GetAreaFuncionales(areaFuncional);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron áreas funcionales.");

            return Ok(result);
        }

        // POST api/AreaFuncional
        [HttpPost]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] AreaFuncional areaFuncional)
        {
            if (areaFuncional == null)
                return BadRequest("Los datos del área funcional no pueden estar vacíos.");

            var result = await _areaFuncionalLogical.CreateAreaFuncional(areaFuncional);
            return Ok(result);
        }

        // PUT api/AreaFuncional/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] AreaFuncional areaFuncional)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            areaFuncional.Id = id;
            var result = await _areaFuncionalLogical.UpdateAreaFuncional(areaFuncional);
            return Ok(result);
        }

        // DELETE api/AreaFuncional/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _areaFuncionalLogical.DeleteAreaFuncional(id);
            return Ok(result);
        }

        // PATCH api/AreaFuncional/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Root,Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] bool estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _areaFuncionalLogical.ActiveAreaFuncional(id, estado);
            return Ok(result);
        }
    }
}
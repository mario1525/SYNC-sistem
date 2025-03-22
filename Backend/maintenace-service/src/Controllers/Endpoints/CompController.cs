using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Controllers.Endpoint
{
    [Route("api/Comp")]
    [ApiController]
    public class CompController : ControllerBase
    {
        private readonly CompLogical _CompLogical;

        public CompController(CompLogical CompLogical)
        {
            _CompLogical = CompLogical;
        }

        // GET: api/Comp
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Comp>>> Get()
        {
            var result = await _CompLogical.GetComps();
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron compañías.");

            return Ok(result);
        }

        // GET: api/Comp/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Admin-Comp")]
        public async Task<ActionResult<List<Comp>>> Get(string id)
        {
            var result = await _CompLogical.GetComp(id);
            if (result == null || result.Count == 0)
                return NotFound($"No se encontró ninguna compañía con ID: {id}");

            return Ok(result);
        }

        // POST api/Comp
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] Comp comp)
        {
            if (comp == null)
                return BadRequest("Los datos de la compañía no pueden estar vacíos.");

            var result = await _CompLogical.CreateComp(comp);
            return Ok(result);
        }

        // PUT api/Comp/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] Comp comp)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            comp.Id = id;
            var result = await _CompLogical.UpdateComp(comp);
            return Ok(result);
        }

        // DELETE api/Comp/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _CompLogical.DeleteComp(id);
            return Ok(result);
        }
    }
}


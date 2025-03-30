using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Entity;

namespace Controllers.Endpoint
{
    [Route("api/Planta")]
    [ApiController]
    public class PlantaController : ControllerBase
    {
        private readonly PlantaLogical _plantaLogical;

        public PlantaController(PlantaLogical plantaLogical)
        {
            _plantaLogical = plantaLogical;
        }

        // GET: api/Planta
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Planta>>> Get([FromQuery] Planta planta)
        {
            var result = await _plantaLogical.GetPlantas(planta);
            if (result == null || result.Count == 0)
                return NotFound("No se encontraron plantas.");

            return Ok(result);
        }

        // POST api/Planta
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Post([FromBody] Planta planta)
        {
            if (planta == null)
                return BadRequest("Los datos de la planta no pueden estar vacíos.");

            var result = await _plantaLogical.CreatePlanta(planta);
            return Ok(result);
        }

        // PUT api/Planta/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Put(string id, [FromBody] Planta planta)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            planta.Id = id;
            var result = await _plantaLogical.UpdatePlanta(planta);
            return Ok(result);
        }

        // DELETE api/Planta/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _plantaLogical.DeletePlanta(id);
            return Ok(result);
        }

        // PATCH api/Planta/5/estado
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] bool estado)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("El ID no puede estar vacío.");

            var result = await _plantaLogical.ActivePlanta(id, estado);
            return Ok(result);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Services;

namespace Controller.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CuadController : ControllerBase
    {
        private readonly CuadLogical _cuadLogical;

        public CuadController(CuadLogical cuadLogical)
        {
            _cuadLogical = cuadLogical;
        }

        // GET: /api/Cuad
        // Permite filtros opcionales por querystring
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cuad>>> GetCuads(
            [FromQuery] string? Id = null,
            [FromQuery] string? Nombre = null,
            [FromQuery] string? IdComp = null,
            [FromQuery] bool? Estado = null
        )
        {
            var criteria = new Cuad
            {
                Id = Id,
                Nombre = Nombre,
                IdComp = IdComp,
                Estado = Estado ?? true
            };

            var result = await _cuadLogical.Getcuad(criteria);
            return Ok(result);
        }

        // GET: /api/Cuad/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Cuad>> GetById(string id)
        {
            var list = await _cuadLogical.Getcuad(new Cuad { Id = id });
            if (list == null || list.Count == 0) return NotFound();
            return Ok(list[0]);
        }

        // POST: /api/Cuad
        [HttpPost]
        public ActionResult<Mensaje> Create([FromBody] Cuad cuad)
        {
            if (cuad == null || string.IsNullOrWhiteSpace(cuad.Nombre) || string.IsNullOrWhiteSpace(cuad.IdComp))
                return BadRequest("Nombre e IdComp son requeridos.");

            var msg = _cuadLogical.CreateCuad(cuad);
            // Devuelve el Id generado en el mensaje.mensaje
            return CreatedAtAction(nameof(GetById), new { id = msg.mensaje }, msg);
        }

        // PUT: /api/Cuad/{id}
        [HttpPut("{id}")]
        public ActionResult<Mensaje> Update(string id, [FromBody] Cuad cuad)
        {
            if (cuad == null) return BadRequest("Cuerpo inválido.");
            cuad.Id = id;
            var msg = _cuadLogical.UpdateCuad(cuad);
            return Ok(msg);
        }

        // DELETE: /api/Cuad/{id}
        [HttpDelete("{id}")]
        public ActionResult<Mensaje> Delete(string id)
        {
            var msg = _cuadLogical.DeleteCuad(id);
            return Ok(msg);
        }

        // PUT: /api/Cuad/{id}/state?estado=1
        [HttpPut("{id}/state")]
        public ActionResult<Mensaje> ChangeState(string id, [FromQuery] int estado)
        {
            var msg = _cuadLogical.ActiveCuad(id, estado);
            return Ok(msg);
        }
    }
}
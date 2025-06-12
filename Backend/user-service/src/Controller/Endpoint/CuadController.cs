using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controller.Endpoint
{
    [Route("api/Cuad")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class CuadController : ControllerBase
    {
        private readonly CuadLogical _CuadLogical;

        public CuadController(CuadLogical Cuadlogical)
        {
            _CuadLogical = Cuadlogical;
        }

        // GET api/Cuad
        [HttpGet]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<List<Cuad>> Get([FromQuery] Cuad Cuad)
        {
            return await _CuadLogical.Getcuad(Cuad);
        }

        // POST api/Cuad
        [HttpPost]
        [Authorize(Roles = "Admin,Supervisor")]
        public Mensaje Post([FromBody] Cuad value)
        {
            return _CuadLogical.CreateCuad(value);
        }

        // PUT api/Cuad/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Supervisor")]
        public Mensaje Put(string id, [FromBody] Cuad value)
        {
            value.Id = id;
            return _CuadLogical.UpdateCuad(value);
        }

        // DELETE api/Cuad/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Supervisor")]
        public Mensaje Delete(string id)
        {
            return _CuadLogical.DeleteCuad(id);
        }
    }
}

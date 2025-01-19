using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Controllers.Endpoint
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersLogical _UserLogical;

        public UsersController(UsersLogical Userslogical)
        {
            _UserLogical = Userslogical;
        }

        // GET api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public async Task<List<Users>> Get(string id)
        {
            return await _UserLogical.GetUser(id);
        }

        // GET api/Users/Comp/5
        [HttpGet("Comp/{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public async Task<List<Users>> Gets(string id)
        {
            return await _UserLogical.GetUsers(id) ;
        }

        // POST api/Users
        [HttpPost]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public Mensaje Post([FromBody] Users value)
        {
            return _UserLogical.CreateUsers(value);
        }

        // PUT api/Users/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public  Mensaje Put(string id, [FromBody] Users value)
        {
            value.Id = id;
            return  _UserLogical.UpdateUsers(value);
        }

        // DELETE api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Admin-Compania")]
        public Mensaje Delete(string id)
        {
            return _UserLogical.DeleteUsers(id);
        }
    }
}

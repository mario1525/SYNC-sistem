using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Controllers.Endpoint
{
    [Route("api/Users")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersLogical _UserLogical;

        public UsersController(UsersLogical Userslogical)
        {
            _UserLogical = Userslogical;
        }

        // GET api/Users
        [HttpGet]
        [Authorize(Roles = "Admin,Root")]
        public async Task<List<Users>> Get([FromQuery] Users user )
        {
            return await _UserLogical.GetUser(user);
        }

        // POST api/Users
        [HttpPost]
        [Authorize(Roles = "Admin,Root")]
        public Mensaje Post([FromBody] Users value)
        {
            return _UserLogical.CreateUsers(value);
        }

        // PUT api/Users/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Root")]
        public  Mensaje Put(string id, [FromBody] Users value)
        {
            value.Id = id;
            return  _UserLogical.UpdateUsers(value);
        }

        // DELETE api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Root")]
        public Mensaje Delete(string id)
        {
            return _UserLogical.DeleteUsers(id);
        }
    }
}

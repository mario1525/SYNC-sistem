using Data;
using Entity;
using Middlewares;
namespace Services
{
    public class UsersLogical
    {
        private readonly DaoUsers _daoUsers;
        
        public UsersLogical(DaoUsers daoUsers)
        {
            _daoUsers = daoUsers;
            
        }

        public async Task<List<Users>> GetUser(Users Users)
        {            
            return await _daoUsers.GetUser(Users);
        } 
        


        public Mensaje CreateUsers(Users Users)
        {
            Guid uid = Guid.NewGuid();
            Users.Id = uid.ToString();           
            _daoUsers.SetUsers("I", Users);
            Mensaje mensaje = new Mensaje();    
            mensaje.mensaje = uid.ToString();
            return mensaje;

        }

        public Mensaje UpdateUsers(Users Users)
        {
            _daoUsers.SetUsers("A", Users);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "Users actualizado";
            return mensaje;

        }

        public Mensaje DeleteUsers(string Id)
        {
            _daoUsers.DeleteUser(Id);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "Users eliminado";
            return mensaje;

        }

        public Mensaje ActiveUsers(string Id, int estado)
        {
            _daoUsers.ActiveUser(Id, estado);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "se cambio el estado del Users";
            return mensaje;

        }        
    }
}


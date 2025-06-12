using Data;
using Entity;
using Middlewares;

namespace Services
{
    public class UsersCredentialLogical
    {
        private readonly DaoUsuarioCredential _daoCredential;
        private readonly DaoUsers _DaoUsers;
        private readonly HashPassword _password;
        private readonly GenerateToken _Token;
        public UsersCredentialLogical(DaoUsuarioCredential daoCredential, HashPassword password, GenerateToken token, DaoUsers DaoUsers)
        {
            _daoCredential = daoCredential;
            _DaoUsers = DaoUsers;
            _password = password;
            _Token = token;

        }

        public async Task<Token> Login(Login login)
        {
            List<UsuarioCredential> credential = await _daoCredential.GetUserName(login.Usuario);            
            if (credential != null)
            {
                
                bool acep = _password.VerifyPassword(login.Contrasenia, credential[0].Contrasenia);
                if (acep)
                {
                    List<UsuarioCredential> credetial = await _daoCredential.GetUserName(login.Usuario);
                    List<Users> Users = await _DaoUsers.GetUserValid(credetial[0].IdUser);
                    Token token = _Token.GenerateJwtToken(Users[0]);
                    return token;
                }   
                return null;
            }

            return null;
        }

        public async Task<Mensaje> CreateUsers(UsuarioCredential Users)
        {
            bool credential = await _daoCredential.ValidCredential(Users.IdUser);
            if( credential )
            {
                Mensaje mensaje = new Mensaje();
                mensaje.mensaje = "El Users tiene credenciales asignadas";
                return mensaje;
            }
            else
            {
                Guid uid = Guid.NewGuid();
                Users.Id = uid.ToString();
                string PassHash = _password.Hashpassword(Users.Contrasenia);
                Users.Contrasenia = PassHash;
                _daoCredential.SetUsers("I", Users);
                Mensaje mensaje = new Mensaje();
                mensaje.mensaje = "Credenciales guardadas correctamente";
                return mensaje;
            }
        }

        public async Task<bool> ValidCredential(string idUser)
        {
            bool credential = await _daoCredential.ValidCredential(idUser);
            return credential ;
        }

        public Mensaje UpdateUsers(UsuarioCredential Users)
        {
            string PassHash = _password.Hashpassword(Users.Contrasenia);
            Users.Contrasenia = PassHash;
            _daoCredential.SetUsers("A", Users);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "credenciales actualizadas";
            return mensaje;

        }

        public Mensaje DeleteUsers(string Id)
        {
            _daoCredential.DeleteUser(Id);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "credenciales eliminadas";
            return mensaje;

        }

        public Mensaje ActiveUsers(string Id, int estado)
        {
            _daoCredential.ActiveUser(Id, estado);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "se cambio el estado de las credenciales";
            return mensaje;

        }
    }
}

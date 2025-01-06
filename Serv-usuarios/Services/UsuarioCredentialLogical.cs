using Data;
using Entity;
using Middlewares;

namespace Services
{
    public class UsuarioCredentialLogical
    {
        private readonly DaoUsuarioCredential _daoCredential;
        private readonly DaoUsuario _daoUsuario;
        private readonly HashPassword _password;
        private readonly GenerateToken _Token;
        public UsuarioCredentialLogical(DaoUsuarioCredential daoCredential, HashPassword password, GenerateToken token, DaoUsuario daoUsuario)
        {
            _daoCredential = daoCredential;
            _daoUsuario = daoUsuario;
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
                    List<Usuario> usuario = await _daoUsuario.GetUser(credetial[0].IdUsuario);
                    Token token = _Token.GenerateJwtToken(usuario[0]);
                    return token;
                }   
                return null;
            }

            return null;
        }

        public async Task<Mensaje> CreateUsuario(UsuarioCredential usuario)
        {
            bool credential = await _daoCredential.ValidCredential(usuario.IdUsuario);
            if( credential )
            {
                Mensaje mensaje = new Mensaje();
                mensaje.mensaje = "El usuario tiene credenciales asignadas";
                return mensaje;
            }
            else
            {
                Guid uid = Guid.NewGuid();
                usuario.Id = uid.ToString();
                string PassHash = _password.Hashpassword(usuario.Contrasenia);
                usuario.Contrasenia = PassHash;
                _daoCredential.SetUsers("I", usuario);
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

        public Mensaje UpdateUsuario(UsuarioCredential usuario)
        {
            string PassHash = _password.Hashpassword(usuario.Contrasenia);
            usuario.Contrasenia = PassHash;
            _daoCredential.SetUsers("A", usuario);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "credenciales actualizadas";
            return mensaje;

        }

        public Mensaje DeleteUsuario(string Id)
        {
            _daoCredential.DeleteUser(Id);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "credenciales eliminadas";
            return mensaje;

        }

        public Mensaje ActiveUsuario(string Id, int estado)
        {
            _daoCredential.ActiveUser(Id, estado);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "se cambio el estado de las credenciales";
            return mensaje;

        }
    }
}

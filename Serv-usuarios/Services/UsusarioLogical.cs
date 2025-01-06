using Data;
using Entity;
using Middlewares;
namespace Services
{
    public class UsuarioLogical
    {
        private readonly DaoUsuario _daoUsuario;
        
        public UsuarioLogical(DaoUsuario daoUsuario)
        {
            _daoUsuario = daoUsuario;
            
        }

        public async Task<List<Usuario>> GetUsuario(String Id)
        {            
            return await _daoUsuario.GetUser(Id);
        }

        public async Task<List<Usuario>> GetUsuariosComp(String IdCompania)
        {
            return await _daoUsuario.GetUsersComp(IdCompania);
        }
        public async Task<List<Usuario>> GetUsuariosSuperv(String IdCompania)
        {
            return await _daoUsuario.GetUsersSuperv(IdCompania);
        }

        public async Task<List<Usuario>> GetUsuariosOperativo(String IdCompania)
        {
            return await _daoUsuario.GetUsersOperativo(IdCompania);
        }

        public async Task<List<Usuario>> GetUsuarios()
        {
            return await _daoUsuario.GetUsers();
        }

        public Mensaje CreateUsuario(Usuario usuario)
        {
            Guid uid = Guid.NewGuid();
            usuario.Id = uid.ToString();           
            _daoUsuario.SetUsers("I", usuario);
            Mensaje mensaje = new Mensaje();    
            mensaje.mensaje = uid.ToString();
            return mensaje;

        }

        public Mensaje UpdateUsuario(Usuario usuario)
        {
            _daoUsuario.SetUsers("A", usuario);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "usuario actualizado";
            return mensaje;

        }

        public Mensaje DeleteUsuario(string Id)
        {
            _daoUsuario.DeleteUser(Id);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "usuario eliminado";
            return mensaje;

        }

        public Mensaje ActiveUsuario(string Id, int estado)
        {
            _daoUsuario.ActiveUser(Id, estado);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "se cambio el estado del usuario";
            return mensaje;

        }        
    }
}


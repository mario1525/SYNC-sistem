
using Data;
using Entity;

namespace Services
{
    public class UserProyectLogical
    {
        private readonly DaoUserProyect _daoUsuario;

        public UserProyectLogical(DaoUserProyect daoUsuario)
        {
            _daoUsuario = daoUsuario;

        }

        public async Task<List<UserProyect>> Gets(String Id)
        {
            return await _daoUsuario.Gets(Id);
        }

        public Mensaje Create(UserProyect usuario)
        {
            Guid uid = Guid.NewGuid();
            usuario.Id = uid.ToString();
            _daoUsuario.Set("I", usuario);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = uid.ToString();
            return mensaje;

        }

        public Mensaje Update(UserProyect usuario)
        {
            _daoUsuario.Set("A", usuario);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "actualizado";
            return mensaje;

        }

        public Mensaje Delete(string Id)
        {
            _daoUsuario.Delete(Id);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "usuario eliminado del proyecto";
            return mensaje;

        }

        public Mensaje ActiveUsuario(string Id, int estado)
        {
            _daoUsuario.Active(Id, estado);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "se cambio el permiso del usuario para este proyecto";
            return mensaje;

        }

    }
}

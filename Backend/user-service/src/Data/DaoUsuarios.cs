using System.Data;
using Entity;
using Microsoft.Data.SqlClient;
using Sync.Common.Data;

namespace Data
{
    public class DaoUsers(SqlClient db) : CrudDao<Users, UsersFiltro>(db)
    {
        protected override string Entidad => "Users";

        protected override IEnumerable<SqlParameter> FiltroParams(UsersFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@Identificacion", f.Identificacion),
            Sql.P("@Correo", Sql.NullIfEmpty(f.Correo)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@IdCuad", Sql.NullIfEmpty(f.IdCuad)),
            Sql.P("@IdEsp", Sql.NullIfEmpty(f.IdEsp)),
            Sql.P("@Rol", Sql.NullIfEmpty(f.Rol)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Users e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@Apellido", e.Apellido),
            Sql.P("@Identificacion", e.Identificacion),
            Sql.P("@Correo", e.Correo),
            Sql.P("@IdComp", e.IdComp),
            Sql.P("@IdCuad", Sql.NullIfEmpty(e.IdCuad)),
            Sql.P("@IdEsp", Sql.NullIfEmpty(e.IdEsp)),
            Sql.P("@Cargo", e.Cargo),
            Sql.P("@Rol", e.Rol),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Users Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            Nombre = r.Str("Nombre"),
            Apellido = r.Str("Apellido"),
            Identificacion = r.Long("Identificacion"),
            Correo = r.Str("Correo"),
            IdComp = r.Str("IdComp"),
            CompNombre = r.StrOrNull("CompNombre"),
            IdCuad = r.StrOrNull("IdCuad"),
            CuadNombre = r.StrOrNull("CuadNombre"),
            IdEsp = r.StrOrNull("IdEsp"),
            EspNombre = r.StrOrNull("EspNombre"),
            Cargo = r.Str("Cargo"),
            Rol = r.Str("Rol"),
            Usuario = r.StrOrNull("Usuario"),
            TieneCredencial = r.Bool("TieneCredencial"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoUsuarioCredential(SqlClient db) : CrudDao<UsuarioCredential, CredentialFiltro>(db)
    {
        protected override string Entidad => "User_Cred";

        protected override IEnumerable<SqlParameter> FiltroParams(CredentialFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdUser", Sql.NullIfEmpty(f.IdUser)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp))
        ];

        /// <summary>Contrasenia aquí ya es el hash BCrypt (o null para no cambiarla).</summary>
        protected override IEnumerable<SqlParameter> SetParams(UsuarioCredential e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@Usuario", e.Usuario),
            Sql.P("@Contrasenia", Sql.NullIfEmpty(e.Contrasenia)),
            Sql.P("@IdUser", e.IdUser),
            Sql.P("@Estado", e.Estado)
        ];

        protected override UsuarioCredential Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            Usuario = r.Str("Usuario"),
            IdUser = r.Str("IdUser"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };

        /// <summary>Busca credenciales activas por nombre de usuario EXACTO.</summary>
        public async Task<CredencialLogin?> GetLoginAsync(string usuario, CancellationToken ct = default)
        {
            var table = await Db.QueryAsync("dbo.db_Sp_User_Cred_Login", [Sql.P("@Usuario", usuario)], ct);
            return table.MapRows(r => new CredencialLogin
            {
                IdCred = r.Str("IdCred"),
                Hash = r.Str("Contrasenia"),
                IdUser = r.Str("IdUser"),
                Nombre = (r.Str("Nombre") + " " + r.Str("Apellido")).Trim(),
                Rol = r.Str("Rol"),
                IdComp = r.Str("IdComp")
            }).FirstOrDefault();
        }
    }
}

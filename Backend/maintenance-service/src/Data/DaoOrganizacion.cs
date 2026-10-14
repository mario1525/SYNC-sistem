using System.Data;
using Entity;
using Microsoft.Data.SqlClient;
using Sync.Common.Data;

namespace Data
{
    public class DaoComp(SqlClient db) : CrudDao<Comp, CompFiltro>(db)
    {
        protected override string Entidad => "Comp";

        protected override IEnumerable<SqlParameter> FiltroParams(CompFiltro f) =>
        [
            // Para compañías, el filtro de tenant se aplica sobre el propio Id
            Sql.P("@Id", f.Id ?? f.IdComp),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@NIT", Sql.NullIfEmpty(f.NIT)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Comp e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@NIT", e.NIT ?? string.Empty),
            Sql.P("@Direccion", e.Direccion ?? string.Empty),
            Sql.P("@Sector", e.Sector ?? string.Empty),
            Sql.P("@Ciudad", e.Ciudad ?? string.Empty),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Comp Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            Nombre = r.Str("Nombre"),
            Ciudad = r.Str("Ciudad"),
            NIT = r.Str("NIT"),
            Direccion = r.Str("Direccion"),
            Sector = r.Str("Sector"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoEsp(SqlClient db) : CrudDao<Esp, EspFiltro>(db)
    {
        protected override string Entidad => "Esp";

        protected override IEnumerable<SqlParameter> FiltroParams(EspFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Esp e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@Descripcion", e.Descripcion),
            Sql.P("@IdComp", Sql.NullIfEmpty(e.IdComp)),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Esp Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            Nombre = r.Str("Nombre"),
            Descripcion = r.Str("Descripcion"),
            IdComp = r.StrOrNull("IdComp"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }
}

using System.Data;
using Entity;
using Microsoft.Data.SqlClient;
using Sync.Common.Data;

namespace Data
{
    public class DaoPlanta(SqlClient db) : CrudDao<Planta, PlantaFiltro>(db)
    {
        protected override string Entidad => "Planta";

        protected override IEnumerable<SqlParameter> FiltroParams(PlantaFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@Region", Sql.NullIfEmpty(f.Region)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Planta e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@Region", e.Region),
            Sql.P("@IdComp", e.IdComp),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Planta Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            Nombre = r.Str("Nombre"),
            Region = r.Str("Region"),
            IdComp = r.Str("IdComp"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoTipoArea(SqlClient db) : CrudDao<TipoArea, TipoAreaFiltro>(db)
    {
        protected override string Entidad => "TipoArea";

        protected override IEnumerable<SqlParameter> FiltroParams(TipoAreaFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(TipoArea e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@Descripcion", e.Descripcion),
            Sql.P("@Estado", e.Estado)
        ];

        protected override TipoArea Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            Nombre = r.Str("Nombre"),
            Descripcion = r.StrOrNull("Descripcion"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoArea(SqlClient db) : CrudDao<Area, AreaFiltro>(db)
    {
        protected override string Entidad => "Area";

        protected override IEnumerable<SqlParameter> FiltroParams(AreaFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdPlanta", Sql.NullIfEmpty(f.IdPlanta)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@TipoArea", Sql.NullIfEmpty(f.TipoArea)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Area e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@IdPlanta", e.IdPlanta),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@TipoArea", e.TipoArea),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Area Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            IdPlanta = r.Str("IdPlanta"),
            Nombre = r.Str("Nombre"),
            TipoArea = r.Str("TipoArea"),
            TipoAreaNombre = r.StrOrNull("TipoAreaNombre"),
            PlantaNombre = r.StrOrNull("PlantaNombre"),
            IdComp = r.StrOrNull("IdComp"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoSeccionArea(SqlClient db) : CrudDao<SeccionArea, SeccionAreaFiltro>(db)
    {
        protected override string Entidad => "SeccionArea";

        protected override IEnumerable<SqlParameter> FiltroParams(SeccionAreaFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdArea", Sql.NullIfEmpty(f.IdArea)),
            Sql.P("@IdPlanta", Sql.NullIfEmpty(f.IdPlanta)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(SeccionArea e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@IdArea", e.IdArea),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@Estado", e.Estado)
        ];

        protected override SeccionArea Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            IdArea = r.Str("IdArea"),
            Nombre = r.Str("Nombre"),
            AreaNombre = r.StrOrNull("AreaNombre"),
            IdPlanta = r.StrOrNull("IdPlanta"),
            PlantaNombre = r.StrOrNull("PlantaNombre"),
            TipoAreaNombre = r.StrOrNull("TipoAreaNombre"),
            IdComp = r.StrOrNull("IdComp"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }
}

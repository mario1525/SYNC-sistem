using System.Data;
using System.Text.Json;
using Entity;
using Microsoft.Data.SqlClient;
using Sync.Common.Data;

namespace Data
{
    /// <summary>
    /// Guía. Si la entidad trae <c>proced</c>, el SP sincroniza el árbol
    /// Proced → Valid en la misma transacción (ver @ProcedJson).
    /// </summary>
    public class DaoGuia(SqlClient db) : CrudDao<Guia, GuiaFiltro>(db)
    {
        private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

        protected override string Entidad => "Guia";

        protected override IEnumerable<SqlParameter> FiltroParams(GuiaFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@IdEsp", Sql.NullIfEmpty(f.IdEsp)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Guia e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@Descripcion", e.Descripcion),
            Sql.P("@Proceso", e.Proceso),
            Sql.P("@Inspeccion", e.Inspeccion),
            Sql.P("@Herramientas", e.Herramientas),
            Sql.P("@IdComp", e.IdComp),
            Sql.P("@IdEsp", Sql.NullIfEmpty(e.IdEsp)),
            Sql.P("@SeguridadInd", e.SeguridadInd),
            Sql.P("@SeguridadAmb", e.SeguridadAmb),
            Sql.P("@Intervalo", e.Intervalo),
            Sql.P("@Importante", e.Importante),
            Sql.P("@Insumos", e.Insumos),
            Sql.P("@Personal", e.Personal),
            Sql.P("@Duracion", e.Duracion),
            Sql.P("@Logistica", e.Logistica),
            Sql.P("@Situacion", e.Situacion),
            Sql.P("@Notas", e.Notas),
            Sql.P("@UpdatedBy", e.UpdatedBy ?? string.Empty),
            Sql.P("@Estado", e.Estado),
            Sql.P("@ProcedJson", e.proced == null ? null : JsonSerializer.Serialize(
                e.proced.Select(p => new
                {
                    id = p.Id,
                    nombre = p.Nombre,
                    descripcion = p.Descripcion,
                    valid = (p.valid ?? []).Select(v => new { id = v.Id, nombre = v.Nombre, descripcion = v.Descripcion, requiereFoto = v.RequiereFoto })
                }), Json))
        ];

        protected override Guia Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            Nombre = r.Str("Nombre"),
            Descripcion = r.Str("Descripcion"),
            Proceso = r.Str("Proceso"),
            Inspeccion = r.Str("Inspeccion"),
            Herramientas = r.Str("Herramientas"),
            IdComp = r.Str("IdComp"),
            IdEsp = r.StrOrNull("IdEsp"),
            EspNombre = r.StrOrNull("EspNombre"),
            SeguridadInd = r.Str("SeguridadInd"),
            SeguridadAmb = r.Str("SeguridadAmb"),
            Intervalo = r.Int("Intervalo"),
            Importante = r.Str("Importante"),
            Insumos = r.Str("Insumos"),
            Personal = r.Int("Personal"),
            Duracion = r.Int("Duracion"),
            Logistica = r.Str("Logistica"),
            Situacion = r.Str("Situacion"),
            Notas = r.Str("Notas"),
            CreatedBy = r.Str("CreatedBy"),
            UpdatedBy = r.Str("UpdatedBy"),
            FechaUpdate = r.Date("FechaUpdate"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoProced(SqlClient db) : CrudDao<Proced, ProcedFiltro>(db)
    {
        protected override string Entidad => "Proced";

        protected override IEnumerable<SqlParameter> FiltroParams(ProcedFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdGuia", Sql.NullIfEmpty(f.IdGuia)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Proced e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@IdGuia", e.IdGuia),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@Descripcion", e.Descripcion),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Proced Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            IdGuia = r.Str("IdGuia"),
            Nombre = r.Str("Nombre"),
            Descripcion = r.Str("Descripcion"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoValid(SqlClient db) : CrudDao<Valid, ValidFiltro>(db)
    {
        protected override string Entidad => "Valid";

        protected override IEnumerable<SqlParameter> FiltroParams(ValidFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdProced", Sql.NullIfEmpty(f.IdProced)),
            Sql.P("@IdGuia", Sql.NullIfEmpty(f.IdGuia)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Valid e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@IdProced", e.IdProced),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@Descripcion", e.Descripcion),
            Sql.P("@RequiereFoto", e.RequiereFoto),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Valid Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            IdProced = r.Str("IdProced"),
            IdGuia = r.StrOrNull("IdGuia"),
            Nombre = r.Str("Nombre"),
            Descripcion = r.Str("Descripcion"),
            RequiereFoto = r.Bool("RequiereFoto"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoGuiaEquipo(SqlClient db) : CrudDao<GuiaEquipo, GuiaEquipoFiltro>(db)
    {
        protected override string Entidad => "GuiaEquipo";

        protected override IEnumerable<SqlParameter> FiltroParams(GuiaEquipoFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdGuia", Sql.NullIfEmpty(f.IdGuia)),
            Sql.P("@IdEquipo", Sql.NullIfEmpty(f.IdEquipo)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(GuiaEquipo e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@IdGuia", e.IdGuia),
            Sql.P("@IdEquipo", e.IdEquipo),
            Sql.P("@Estado", e.Estado)
        ];

        protected override GuiaEquipo Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            IdGuia = r.Str("IdGuia"),
            IdEquipo = r.Str("IdEquipo"),
            GuiaNombre = r.StrOrNull("GuiaNombre"),
            EquipoNombre = r.StrOrNull("EquipoNombre"),
            Intervalo = r.Int("Intervalo"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }
}

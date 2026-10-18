using System.Data;
using Entity;
using Microsoft.Data.SqlClient;
using Sync.Common.Data;

namespace Data
{
    public class DaoFalla(SqlClient db) : CrudDao<Falla, FallaFiltro>(db)
    {
        protected override string Entidad => "Falla";

        protected override IEnumerable<SqlParameter> FiltroParams(FallaFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@IdEquipo", Sql.NullIfEmpty(f.IdEquipo)),
            Sql.P("@EstadoFalla", Sql.NullIfEmpty(f.EstadoFalla)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Falla e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@IdComp", e.IdComp),
            Sql.P("@IdEquipo", e.IdEquipo),
            Sql.P("@Tipo", e.Tipo),
            Sql.P("@Prioridad", e.Prioridad),
            Sql.P("@EquipoDetenido", e.EquipoDetenido),
            Sql.P("@Descripcion", Sql.NullIfEmpty(e.Descripcion)),
            Sql.P("@EstadoFalla", e.EstadoFalla),
            Sql.P("@IdUserReporta", e.IdUserReporta),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Falla Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            IdComp = r.Str("IdComp"),
            IdEquipo = r.Str("IdEquipo"),
            EquipoNombre = r.StrOrNull("EquipoNombre"),
            EquipoSerie = r.StrOrNull("EquipoSerie"),
            Tipo = r.Str("Tipo"),
            Prioridad = r.Str("Prioridad"),
            EquipoDetenido = r.Bool("EquipoDetenido"),
            Descripcion = r.StrOrNull("Descripcion"),
            EstadoFalla = r.Str("EstadoFalla"),
            IdUserReporta = r.StrOrNull("IdUserReporta"),
            ReportaNombre = r.StrOrNull("ReportaNombre"),
            FechaReporte = r.Date("FechaReporte"),
            FechaCierre = r.Date("FechaCierre"),
            NumFotos = r.Int("NumFotos"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    /// <summary>Adjuntos: metadatos por un lado y bytes por otro (para no cargar imágenes en los listados).</summary>
    public class DaoAdjunto(SqlClient db)
    {
        public virtual async Task<List<Adjunto>> GetAsync(string? id, string? idComp, string? entidad, string? idEntidad, CancellationToken ct = default)
        {
            var table = await db.QueryAsync("dbo.db_Sp_Adjunto_Get",
            [
                Sql.P("@Id", Sql.NullIfEmpty(id)),
                Sql.P("@IdComp", Sql.NullIfEmpty(idComp)),
                Sql.P("@Entidad", Sql.NullIfEmpty(entidad)),
                Sql.P("@IdEntidad", Sql.NullIfEmpty(idEntidad))
            ], ct);
            return table.MapRows(r => new Adjunto
            {
                Id = r.Str("Id"),
                IdComp = r.Str("IdComp"),
                Entidad = r.Str("Entidad"),
                IdEntidad = r.Str("IdEntidad"),
                Ref = r.StrOrNull("Ref"),
                Nombre = r.Str("Nombre"),
                ContentType = r.Str("ContentType"),
                Tamano = r.Int("Tamano"),
                IdUser = r.StrOrNull("IdUser"),
                Fecha = r.Date("Fecha")
            });
        }

        public virtual async Task<ArchivoAdjunto?> GetDatosAsync(string id, string? idComp, CancellationToken ct = default)
        {
            var table = await db.QueryAsync("dbo.db_Sp_Adjunto_Datos", [Sql.P("@Id", id), Sql.P("@IdComp", Sql.NullIfEmpty(idComp))], ct);
            if (table.Rows.Count == 0) return null;
            var r = table.Rows[0];
            return new ArchivoAdjunto(r.Str("Nombre"), r.Str("ContentType"), (byte[])r["Datos"]);
        }

        public virtual Task SetAsync(Adjunto a, byte[] datos, CancellationToken ct = default) =>
            db.ExecuteAsync("dbo.db_Sp_Adjunto_Set",
            [
                Sql.P("@Id", a.Id),
                Sql.P("@IdComp", a.IdComp),
                Sql.P("@Entidad", a.Entidad),
                Sql.P("@IdEntidad", a.IdEntidad),
                Sql.P("@Ref", Sql.NullIfEmpty(a.Ref)),
                Sql.P("@Nombre", a.Nombre),
                Sql.P("@ContentType", a.ContentType),
                new SqlParameter("@Datos", SqlDbType.VarBinary, -1) { Value = datos },
                Sql.P("@IdUser", a.IdUser)
            ], ct);

        public virtual Task DeleteAsync(string id, CancellationToken ct = default) =>
            db.ExecuteAsync("dbo.db_Sp_Adjunto_Del", [Sql.P("@Id", id)], ct);
    }
}

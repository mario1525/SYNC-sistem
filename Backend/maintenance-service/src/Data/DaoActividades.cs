using System.Data;
using Entity;
using Microsoft.Data.SqlClient;
using Sync.Common.Data;

namespace Data
{
    public class DaoTipoActividad(SqlClient db) : CrudDao<TipoActividad, TipoActividadFiltro>(db)
    {
        protected override string Entidad => "TipoActividad";

        protected override IEnumerable<SqlParameter> FiltroParams(TipoActividadFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(TipoActividad e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@IdComp", e.IdComp),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@Descripcion", e.Descripcion),
            Sql.P("@Estado", e.Estado)
        ];

        protected override TipoActividad Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            IdComp = r.Str("IdComp"),
            Nombre = r.Str("Nombre"),
            Descripcion = r.Str("Descripcion"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoActividad(SqlClient db) : CrudDao<Actividad, ActividadFiltro>(db)
    {
        protected override string Entidad => "Actividad";

        protected override IEnumerable<SqlParameter> FiltroParams(ActividadFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@IdTipoActividad", Sql.NullIfEmpty(f.IdTipoActividad)),
            Sql.P("@IdCuad", Sql.NullIfEmpty(f.IdCuad)),
            Sql.P("@FechaDesde", f.FechaDesde),
            Sql.P("@FechaHasta", f.FechaHasta),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Actividad e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@IdComp", e.IdComp),
            Sql.P("@Descripcion", e.Descripcion),
            Sql.P("@IdTipoActividad", e.IdTipoActividad),
            Sql.P("@Ubicacion", e.Ubicacion),
            Sql.P("@FechaEjecucion", e.FechaEjecucion),
            Sql.P("@IdCuad", Sql.NullIfEmpty(e.IdCuad)),
            Sql.P("@Detalle", e.Detalle),
            Sql.P("@Intervalo", e.Intervalo),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Actividad Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            IdComp = r.Str("IdComp"),
            Descripcion = r.Str("Descripcion"),
            IdTipoActividad = r.Str("IdTipoActividad"),
            TipoActividadNombre = r.StrOrNull("TipoActividadNombre"),
            Ubicacion = r.Str("Ubicacion"),
            FechaEjecucion = r.Date("FechaEjecucion"),
            IdCuad = r.StrOrNull("IdCuad"),
            CuadNombre = r.StrOrNull("CuadNombre"),
            Detalle = r.Str("Detalle"),
            Intervalo = r.Int("Intervalo"),
            NumEquipos = r.Int("NumEquipos"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoActividadEquipo(SqlClient db) : CrudDao<ActividadEquipo, ActividadEquipoFiltro>(db)
    {
        protected override string Entidad => "ActividadEquipo";

        protected override IEnumerable<SqlParameter> FiltroParams(ActividadEquipoFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdActividad", Sql.NullIfEmpty(f.IdActividad)),
            Sql.P("@IdEquipo", Sql.NullIfEmpty(f.IdEquipo)),
            Sql.P("@IdGuia", Sql.NullIfEmpty(f.IdGuia)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@IdCuad", Sql.NullIfEmpty(f.IdCuad)),
            Sql.P("@FechaDesde", f.FechaDesde),
            Sql.P("@FechaHasta", f.FechaHasta),
            Sql.P("@EstadoEjec", Sql.NullIfEmpty(f.EstadoEjec)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(ActividadEquipo e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@IdActividad", e.IdActividad),
            Sql.P("@IdEquipo", e.IdEquipo),
            Sql.P("@IdGuia", Sql.NullIfEmpty(e.IdGuia)),
            Sql.P("@Estado", e.Estado)
        ];

        protected override ActividadEquipo Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            IdActividad = r.Str("IdActividad"),
            IdEquipo = r.Str("IdEquipo"),
            IdGuia = r.StrOrNull("IdGuia"),
            EquipoNombre = r.StrOrNull("EquipoNombre"),
            EquipoSerie = r.StrOrNull("EquipoSerie"),
            GuiaNombre = r.StrOrNull("GuiaNombre"),
            GuiaDuracion = r.Int("GuiaDuracion"),
            FechaEjecucion = r.Date("FechaEjecucion"),
            ActividadDescripcion = r.StrOrNull("ActividadDescripcion"),
            TipoActividadNombre = r.StrOrNull("TipoActividadNombre"),
            IdCuad = r.StrOrNull("IdCuad"),
            CuadNombre = r.StrOrNull("CuadNombre"),
            UbicacionNombre = r.StrOrNull("UbicacionNombre"),
            EstadoEjec = r.Has("EstadoEjec") ? r.Str("EstadoEjec") : ActividadEquipo.Pendiente,
            FechaInicio = r.Date("FechaInicio"),
            FechaFin = r.Date("FechaFin"),
            IdUserEjec = r.StrOrNull("IdUserEjec"),
            Observacion = r.StrOrNull("Observacion"),
            TotalValid = r.Int("TotalValid"),
            ValidCumplidas = r.Int("ValidCumplidas"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };

        // ------------------------------------------------------------ Ejecución en campo

        public virtual async Task<List<EjecucionValid>> GetResultadosAsync(string idActividadEquipo, CancellationToken ct = default)
        {
            var table = await Db.QueryAsync("dbo.db_Sp_EjecucionValid_Get", [Sql.P("@IdActividadEquipo", idActividadEquipo)], ct);
            return table.MapRows(r => new EjecucionValid
            {
                Id = r.Str("Id"),
                IdActividadEquipo = r.Str("IdActividadEquipo"),
                IdValid = r.Str("IdValid"),
                Cumple = r.Bool("Cumple"),
                Observacion = r.StrOrNull("Observacion"),
                IdUser = r.StrOrNull("IdUser"),
                Fecha = r.Date("Fecha")
            });
        }

        /// <summary>Registra o corrige el resultado de una verificación (inicia la ejecución si estaba pendiente).</summary>
        public virtual Task SetResultadoAsync(EjecucionValid e, CancellationToken ct = default) =>
            Db.ExecuteAsync("dbo.db_Sp_EjecucionValid_Set",
            [
                Sql.P("@Id", e.Id),
                Sql.P("@IdActividadEquipo", e.IdActividadEquipo),
                Sql.P("@IdValid", e.IdValid),
                Sql.P("@Cumple", e.Cumple),
                Sql.P("@Observacion", Sql.NullIfEmpty(e.Observacion)),
                Sql.P("@IdUser", e.IdUser)
            ], ct);

        public virtual Task SetEjecucionAsync(string id, string estadoEjec, string? idUser, string? observacion, CancellationToken ct = default) =>
            Db.ExecuteAsync("dbo.db_Sp_ActividadEquipo_Ejecucion",
            [
                Sql.P("@Id", id),
                Sql.P("@EstadoEjec", estadoEjec),
                Sql.P("@IdUser", idUser),
                Sql.P("@Observacion", Sql.NullIfEmpty(observacion))
            ], ct);
    }
}

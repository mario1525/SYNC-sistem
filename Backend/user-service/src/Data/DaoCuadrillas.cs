using System.Data;
using Entity;
using Microsoft.Data.SqlClient;
using Sync.Common.Data;

namespace Data
{
    public class DaoCuad(SqlClient db) : CrudDao<Cuad, CuadFiltro>(db)
    {
        protected override string Entidad => "Cuad";

        protected override IEnumerable<SqlParameter> FiltroParams(CuadFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Cuad e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@IdComp", e.IdComp),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Cuad Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            Nombre = r.Str("Nombre"),
            IdComp = r.Str("IdComp"),
            NumIntegrantes = r.Int("NumIntegrantes"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoTurno(SqlClient db) : CrudDao<Turno, TurnoFiltro>(db)
    {
        protected override string Entidad => "Turno";

        protected override IEnumerable<SqlParameter> FiltroParams(TurnoFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Turno e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@IdComp", e.IdComp),
            Sql.P("@HoraInicio", TimeSpan.Parse(e.HoraInicio)),
            Sql.P("@HoraFin", TimeSpan.Parse(e.HoraFin)),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Turno Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            Nombre = r.Str("Nombre"),
            IdComp = r.Str("IdComp"),
            HoraInicio = r.Time("HoraInicio").ToString(@"hh\:mm"),
            HoraFin = r.Time("HoraFin").ToString(@"hh\:mm"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }

    public class DaoHorario(SqlClient db) : CrudDao<Horario, HorarioFiltro>(db)
    {
        protected override string Entidad => "Horario";

        protected override IEnumerable<SqlParameter> FiltroParams(HorarioFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@IdTurno", Sql.NullIfEmpty(f.IdTurno)),
            Sql.P("@IdCuad", Sql.NullIfEmpty(f.IdCuad)),
            Sql.P("@DiaSemana", f.DiaSemana),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Horario e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@IdTurno", e.IdTurno),
            Sql.P("@IdCuad", e.IdCuad),
            Sql.P("@DiaSemana", e.DiaSemana),
            Sql.P("@EsFestivo", e.EsFestivo),
            Sql.P("@Estado", e.Estado)
        ];

        protected override Horario Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            IdTurno = r.Str("IdTurno"),
            TurnoNombre = r.StrOrNull("TurnoNombre"),
            HoraInicio = r.Time("HoraInicio").ToString(@"hh\:mm"),
            HoraFin = r.Time("HoraFin").ToString(@"hh\:mm"),
            IdCuad = r.Str("IdCuad"),
            CuadNombre = r.StrOrNull("CuadNombre"),
            DiaSemana = r.Int("DiaSemana"),
            EsFestivo = r.Bool("EsFestivo"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log")
        };
    }
}

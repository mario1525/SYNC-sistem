using Data;
using Entity;
using Sync.Common.Services;
using Sync.Common.Web;

namespace Services
{
    public class CuadLogical(DaoCuad dao) : CrudLogical<Cuad, CuadFiltro>(dao)
    {
        protected override string Nombre => "Cuadrilla";

        protected override async Task PrepararAsync(Cuad e, Cuad? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            await base.PrepararAsync(e, existente, tenant, ct);
        }
    }

    public class TurnoLogical(DaoTurno dao) : CrudLogical<Turno, TurnoFiltro>(dao)
    {
        protected override string Nombre => "Turno";

        protected override async Task PrepararAsync(Turno e, Turno? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            e.HoraInicio = Hora(e.HoraInicio, "HoraInicio");
            e.HoraFin = Hora(e.HoraFin, "HoraFin");
            if (e.HoraInicio == e.HoraFin)
                throw new BusinessException("HoraInicio y HoraFin no pueden ser iguales.");
            await base.PrepararAsync(e, existente, tenant, ct);
        }

        private static string Hora(string? valor, string campo) =>
            TimeSpan.TryParse(valor, out var t) && t >= TimeSpan.Zero && t < TimeSpan.FromDays(1)
                ? t.ToString(@"hh\:mm")
                : throw new BusinessException($"{campo} debe tener formato HH:mm.");
    }

    public class HorarioLogical(DaoHorario dao, TurnoLogical turnos, CuadLogical cuadrillas) : CrudLogical<Horario, HorarioFiltro>(dao)
    {
        protected override string Nombre => "Horario";

        protected override async Task PrepararAsync(Horario e, Horario? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.IdTurno, "IdTurno");
            Requerido(e.IdCuad, "IdCuad");
            if (e.DiaSemana is < 1 or > 7)
                throw new BusinessException("DiaSemana debe estar entre 1 (lunes) y 7 (domingo).");
            var turno = await turnos.ObtenerAsync(e.IdTurno, tenant, ct);
            var cuad = await cuadrillas.ObtenerAsync(e.IdCuad, tenant, ct);
            if (turno.IdComp != cuad.IdComp)
                throw new BusinessException("El turno y la cuadrilla deben pertenecer a la misma compañía.");
        }
    }
}

using Data;
using Entity;
using Sync.Common.Services;
using Sync.Common.Web;

namespace Services
{
    public class TipoActividadLogical(DaoTipoActividad dao) : CrudLogical<TipoActividad, TipoActividadFiltro>(dao)
    {
        protected override string Nombre => "Tipo de actividad";

        protected override async Task PrepararAsync(TipoActividad e, TipoActividad? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            await base.PrepararAsync(e, existente, tenant, ct);
        }
    }

    public class ActividadLogical(DaoActividad dao, TipoActividadLogical tipos) : CrudLogical<Actividad, ActividadFiltro>(dao)
    {
        protected override string Nombre => "Actividad";

        protected override async Task PrepararAsync(Actividad e, Actividad? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.IdTipoActividad, "IdTipoActividad");
            if (e.Intervalo < 0)
                throw new BusinessException("Intervalo no puede ser negativo.");
            await base.PrepararAsync(e, existente, tenant, ct);

            var tipo = await tipos.ObtenerAsync(e.IdTipoActividad, tenant, ct);
            if (tipo.IdComp != e.IdComp)
                throw new BusinessException("El tipo de actividad no pertenece a la compañía.");
            if (string.IsNullOrWhiteSpace(e.IdCuad))
                e.IdCuad = null;
        }
    }

    public class ActividadEquipoLogical(DaoActividadEquipo dao, ActividadLogical actividades, EquipoLogical equipos, GuiaLogical guias)
        : CrudLogical<ActividadEquipo, ActividadEquipoFiltro>(dao)
    {
        protected override string Nombre => "Equipo de la actividad";

        protected override async Task PrepararAsync(ActividadEquipo e, ActividadEquipo? existente, Tenant tenant, CancellationToken ct)
        {
            if (existente != null)
            {
                e.IdActividad = existente.IdActividad;
                e.IdEquipo = existente.IdEquipo;
            }
            Requerido(e.IdActividad, "IdActividad");
            Requerido(e.IdEquipo, "IdEquipo");

            var actividad = await actividades.ObtenerAsync(e.IdActividad, tenant, ct);
            var equipo = await equipos.ObtenerAsync(e.IdEquipo, tenant, ct);
            if (actividad.IdComp != equipo.IdComp)
                throw new BusinessException("La actividad y el equipo deben pertenecer a la misma compañía.");

            if (!string.IsNullOrWhiteSpace(e.IdGuia))
                await guias.ObtenerAsync(e.IdGuia, tenant, ct);
            else
                e.IdGuia = null;
        }
    }
}

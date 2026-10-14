using Data;
using Entity;
using Sync.Common.Services;
using Sync.Common.Web;

namespace Services
{
    public class PlantaLogical(DaoPlanta dao) : CrudLogical<Planta, PlantaFiltro>(dao)
    {
        protected override string Nombre => "Planta";

        protected override async Task PrepararAsync(Planta e, Planta? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            await base.PrepararAsync(e, existente, tenant, ct);
        }
    }

    /// <summary>Catálogo global de tipos de área. Solo Root escribe (ver controlador).</summary>
    public class TipoAreaLogical(DaoTipoArea dao) : CrudLogical<TipoArea, TipoAreaFiltro>(dao)
    {
        protected override string Nombre => "Tipo de área";

        protected override Task PrepararAsync(TipoArea e, TipoArea? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            return Task.CompletedTask;
        }
    }

    public class AreaLogical(DaoArea dao, PlantaLogical plantas, TipoAreaLogical tipos) : CrudLogical<Area, AreaFiltro>(dao)
    {
        protected override string Nombre => "Área";

        protected override async Task PrepararAsync(Area e, Area? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            Requerido(e.IdPlanta, "IdPlanta");
            Requerido(e.TipoArea, "TipoArea");
            // La planta debe pertenecer a la compañía del usuario
            await plantas.ObtenerAsync(e.IdPlanta, tenant, ct);
            await tipos.ObtenerAsync(e.TipoArea, tenant, ct);
        }
    }

    public class SeccionAreaLogical(DaoSeccionArea dao, AreaLogical areas) : CrudLogical<SeccionArea, SeccionAreaFiltro>(dao)
    {
        protected override string Nombre => "Sección";

        protected override async Task PrepararAsync(SeccionArea e, SeccionArea? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            Requerido(e.IdArea, "IdArea");
            await areas.ObtenerAsync(e.IdArea, tenant, ct);
        }
    }
}

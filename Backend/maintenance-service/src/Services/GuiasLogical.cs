using Data;
using Entity;
using Sync.Common.Services;
using Sync.Common.Web;

namespace Services
{
    /// <summary>
    /// Guía de mantenimiento con su árbol de pasos (Proced) y verificaciones (Valid).
    /// El detalle (GET /{id}) devuelve el árbol completo; el guardado lo sincroniza en una transacción.
    /// </summary>
    public class GuiaLogical(DaoGuia dao, DaoProced daoProced, DaoValid daoValid, EspLogical especialidades)
        : CrudLogical<Guia, GuiaFiltro>(dao)
    {
        protected override string Nombre => "Guía";

        /// <summary>Detalle con pasos y verificaciones.</summary>
        public async Task<Guia> DetalleAsync(string id, Tenant tenant, CancellationToken ct = default)
        {
            var guia = await ObtenerAsync(id, tenant, ct);
            var pasos = await daoProced.GetAsync(new ProcedFiltro { IdGuia = id }, ct);
            var verificaciones = await daoValid.GetAsync(new ValidFiltro { IdGuia = id }, ct);
            foreach (var paso in pasos)
                paso.valid = verificaciones.Where(v => v.IdProced == paso.Id).ToList();
            guia.proced = pasos;
            return guia;
        }

        protected override async Task PrepararAsync(Guia e, Guia? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            if (e.Intervalo < 0 || e.Personal < 0 || e.Duracion < 0)
                throw new BusinessException("Intervalo, Personal y Duracion no pueden ser negativos.");
            await base.PrepararAsync(e, existente, tenant, ct);

            if (!string.IsNullOrWhiteSpace(e.IdEsp))
                await especialidades.ObtenerAsync(e.IdEsp, tenant, ct);
            else
                e.IdEsp = null;

            e.UpdatedBy = tenant.UserId;

            // Ids para los pasos/verificaciones nuevos
            foreach (var paso in e.proced ?? [])
            {
                Requerido(paso.Nombre, "Nombre del procedimiento");
                if (string.IsNullOrWhiteSpace(paso.Id)) paso.Id = Guid.NewGuid().ToString();
                foreach (var v in paso.valid ?? [])
                {
                    Requerido(v.Nombre, "Nombre de la validación");
                    if (string.IsNullOrWhiteSpace(v.Id)) v.Id = Guid.NewGuid().ToString();
                }
            }
        }
    }

    public class ProcedLogical(DaoProced dao, GuiaLogical guias) : CrudLogical<Proced, ProcedFiltro>(dao)
    {
        protected override string Nombre => "Procedimiento";

        protected override async Task PrepararAsync(Proced e, Proced? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            if (existente != null) e.IdGuia = existente.IdGuia;
            Requerido(e.IdGuia, "IdGuia");
            await guias.ObtenerAsync(e.IdGuia, tenant, ct);
        }
    }

    public class ValidLogical(DaoValid dao, ProcedLogical pasos) : CrudLogical<Valid, ValidFiltro>(dao)
    {
        protected override string Nombre => "Validación";

        protected override async Task PrepararAsync(Valid e, Valid? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            if (existente != null) e.IdProced = existente.IdProced;
            Requerido(e.IdProced, "IdProced");
            await pasos.ObtenerAsync(e.IdProced, tenant, ct);
        }
    }

    public class GuiaEquipoLogical(DaoGuiaEquipo dao, GuiaLogical guias, EquipoLogical equipos)
        : CrudLogical<GuiaEquipo, GuiaEquipoFiltro>(dao)
    {
        protected override string Nombre => "Asignación de guía";

        protected override async Task PrepararAsync(GuiaEquipo e, GuiaEquipo? existente, Tenant tenant, CancellationToken ct)
        {
            if (existente != null)
            {
                e.IdGuia = existente.IdGuia;
                e.IdEquipo = existente.IdEquipo;
                return;
            }
            Requerido(e.IdGuia, "IdGuia");
            Requerido(e.IdEquipo, "IdEquipo");
            var guia = await guias.ObtenerAsync(e.IdGuia, tenant, ct);
            var equipo = await equipos.ObtenerAsync(e.IdEquipo, tenant, ct);
            if (guia.IdComp != equipo.IdComp)
                throw new BusinessException("La guía y el equipo deben pertenecer a la misma compañía.");
        }
    }
}

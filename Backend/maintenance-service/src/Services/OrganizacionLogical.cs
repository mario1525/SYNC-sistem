using Data;
using Entity;
using Sync.Common.Services;
using Sync.Common.Web;

namespace Services
{
    public class CompLogical(DaoComp dao) : CrudLogical<Comp, CompFiltro>(dao)
    {
        protected override string Nombre => "Compañía";

        public override Task<List<Comp>> ListarAsync(CompFiltro filtro, Tenant tenant, CancellationToken ct = default)
        {
            // Un usuario no Root solo ve su propia compañía
            if (!tenant.IsRoot)
                filtro.Id = tenant.IdComp;
            filtro.IdComp = null;
            return Dao.GetAsync(filtro, ct);
        }

        public override async Task<Comp> ObtenerAsync(string id, Tenant tenant, CancellationToken ct = default)
        {
            if (!tenant.IsRoot && id != tenant.IdComp)
                throw new NotFoundException("Compañía no encontrada.");
            var lista = await Dao.GetAsync(new CompFiltro { Id = id }, ct);
            return lista.FirstOrDefault() ?? throw new NotFoundException("Compañía no encontrada.");
        }

        protected override Task PrepararAsync(Comp e, Comp? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            return Task.CompletedTask;
        }
    }

    public class EspLogical(DaoEsp dao) : CrudLogical<Esp, EspFiltro>(dao)
    {
        protected override string Nombre => "Especialidad";

        protected override Task PrepararAsync(Esp e, Esp? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            if (existente != null)
            {
                // Las especialidades globales (IdComp null) solo las modifica Root
                if (existente.IdComp == null && !tenant.IsRoot)
                    throw new ForbiddenException("Las especialidades globales solo las modifica el administrador del sistema.");
                e.IdComp = existente.IdComp;
            }
            else
            {
                // Root puede crear especialidades globales (sin IdComp)
                e.IdComp = tenant.IsRoot ? string.IsNullOrWhiteSpace(e.IdComp) ? null : e.IdComp : tenant.IdComp;
            }
            return Task.CompletedTask;
        }

        protected override Task ValidarCambioAsync(Esp existente, Tenant tenant, CancellationToken ct) =>
            existente.IdComp == null && !tenant.IsRoot
                ? throw new ForbiddenException("Las especialidades globales solo las modifica el administrador del sistema.")
                : Task.CompletedTask;
    }
}

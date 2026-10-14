using Data;
using Entity;
using Sync.Common.Services;
using Sync.Common.Web;

namespace Services
{
    public class EquipoLogical(DaoEquipo dao, SeccionAreaLogical secciones) : CrudLogical<Equipo, EquipoFiltro>(dao)
    {
        private readonly DaoEquipo _dao = dao;

        protected override string Nombre => "Equipo";

        protected override async Task PrepararAsync(Equipo e, Equipo? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            await base.PrepararAsync(e, existente, tenant, ct);

            if (e.AnioFabricacion != 0 && (e.AnioFabricacion < 1900 || e.AnioFabricacion > DateTime.UtcNow.Year + 1))
                throw new BusinessException("AnioFabricacion no es válido.");

            // Ubicación: null = sin cambios. Si viene, se valida tipo y sección del mismo tenant.
            if (e.Ubicacion is { } u)
            {
                if (!UbicacionEquipo.Tipos.Contains(u.TipoUbicacion))
                    throw new BusinessException($"TipoUbicacion debe ser uno de: {string.Join(", ", UbicacionEquipo.Tipos)}.");
                if (!string.IsNullOrWhiteSpace(u.IdSeccionArea))
                    await secciones.ObtenerAsync(u.IdSeccionArea, tenant, ct);
                else
                    u.IdSeccionArea = null;
            }
        }

        public async Task<List<UbicacionEquipo>> UbicacionesAsync(string idEquipo, Tenant tenant, CancellationToken ct = default)
        {
            await ObtenerAsync(idEquipo, tenant, ct);
            return await _dao.GetUbicacionesAsync(idEquipo, tenant.Filtro(null), ct);
        }
    }
}

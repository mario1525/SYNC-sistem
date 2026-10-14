using Sync.Common.Data;
using Sync.Common.Web;

namespace Sync.Common.Services
{
    /// <summary>
    /// Lógica CRUD con aislamiento por compañía.
    /// Toda lectura y escritura pasa por el <see cref="Tenant"/> del usuario.
    /// </summary>
    public abstract class CrudLogical<T, TFiltro>
        where T : IEntidad
        where TFiltro : IFiltro, new()
    {
        protected readonly CrudDao<T, TFiltro> Dao;

        protected CrudLogical(CrudDao<T, TFiltro> dao) => Dao = dao;

        /// <summary>Nombre legible para mensajes ("Equipo", "Planta"...).</summary>
        protected abstract string Nombre { get; }

        public virtual Task<List<T>> ListarAsync(TFiltro filtro, Tenant tenant, CancellationToken ct = default)
        {
            filtro.IdComp = tenant.Filtro(filtro.IdComp);
            return Dao.GetAsync(filtro, ct);
        }

        /// <summary>Obtiene un registro visible para el tenant o lanza 404.</summary>
        public virtual async Task<T> ObtenerAsync(string id, Tenant tenant, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new BusinessException("El Id es obligatorio.");
            var filtro = new TFiltro { Id = id, IdComp = tenant.Filtro(null) };
            var lista = await Dao.GetAsync(filtro, ct);
            return lista.FirstOrDefault() ?? throw new NotFoundException($"{Nombre}: registro no encontrado.");
        }

        public virtual async Task<Mensaje> CrearAsync(T entidad, Tenant tenant, CancellationToken ct = default)
        {
            entidad.Id = Guid.NewGuid().ToString();
            await PrepararAsync(entidad, default, tenant, ct);
            await Dao.SetAsync('I', entidad, ct);
            return new Mensaje($"{Nombre}: registro creado.", entidad.Id);
        }

        public virtual async Task<Mensaje> ActualizarAsync(string id, T entidad, Tenant tenant, CancellationToken ct = default)
        {
            var existente = await ObtenerAsync(id, tenant, ct);
            entidad.Id = id;
            await PrepararAsync(entidad, existente, tenant, ct);
            await Dao.SetAsync('A', entidad, ct);
            return new Mensaje($"{Nombre}: registro actualizado.", id);
        }

        public virtual async Task<Mensaje> EliminarAsync(string id, Tenant tenant, CancellationToken ct = default)
        {
            var existente = await ObtenerAsync(id, tenant, ct);
            await ValidarCambioAsync(existente, tenant, ct);
            await Dao.DeleteAsync(id, ct);
            return new Mensaje($"{Nombre}: registro eliminado.", id);
        }

        public virtual async Task<Mensaje> CambiarEstadoAsync(string id, bool estado, Tenant tenant, CancellationToken ct = default)
        {
            var existente = await ObtenerAsync(id, tenant, ct);
            await ValidarCambioAsync(existente, tenant, ct);
            await Dao.ActiveAsync(id, estado, ct);
            return new Mensaje(estado ? $"{Nombre}: registro activado." : $"{Nombre}: registro desactivado.", id);
        }

        /// <summary>
        /// Valida y completa la entidad antes de guardar.
        /// Por defecto: si la entidad pertenece a una compañía, se fuerza el IdComp del tenant
        /// (o el del registro existente al actualizar). Las entidades hijas deben sobrescribirlo
        /// para verificar que el padre pertenece al tenant.
        /// </summary>
        protected virtual Task PrepararAsync(T entidad, T? existente, Tenant tenant, CancellationToken ct)
        {
            if (entidad is IEntidadCompania conCompania)
            {
                conCompania.IdComp = existente is IEntidadCompania previo
                    ? previo.IdComp
                    : tenant.ParaEscritura(conCompania.IdComp);
            }
            return Task.CompletedTask;
        }

        /// <summary>Validación adicional antes de eliminar o cambiar el estado de un registro.</summary>
        protected virtual Task ValidarCambioAsync(T existente, Tenant tenant, CancellationToken ct) => Task.CompletedTask;

        protected static void Requerido(string? valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new BusinessException($"El campo {campo} es obligatorio.");
        }
    }
}

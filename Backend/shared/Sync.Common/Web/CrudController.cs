using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sync.Common.Data;
using Sync.Common.Services;

namespace Sync.Common.Web
{
    /// <summary>
    /// Controlador REST estándar:
    ///   GET    /            lista (filtros por query string)
    ///   GET    /{id}        detalle
    ///   POST   /            crear              (RolesEscritura)
    ///   PUT    /{id}        actualizar         (RolesEscritura)
    ///   DELETE /{id}        eliminar (lógico)  (RolesEscritura)
    ///   PATCH  /{id}/estado activar/desactivar (RolesEscritura)
    /// </summary>
    [ApiController]
    [Authorize]
    public abstract class CrudController<T, TFiltro> : ControllerBase
        where T : IEntidad
        where TFiltro : IFiltro, new()
    {
        protected abstract CrudLogical<T, TFiltro> Logic { get; }

        /// <summary>Roles que pueden leer. Por defecto todos los autenticados.</summary>
        protected virtual string[] RolesLectura => Roles.Todos;
        /// <summary>Roles que pueden escribir. Por defecto Root y Admin.</summary>
        protected virtual string[] RolesEscritura => Roles.Gestion;

        protected Tenant Tenant => User.Tenant();

        [HttpGet]
        public virtual async Task<ActionResult<List<T>>> Get([FromQuery] TFiltro filtro, CancellationToken ct)
        {
            Exigir(RolesLectura);
            return Ok(await Logic.ListarAsync(filtro, Tenant, ct));
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<T>> GetById(string id, CancellationToken ct)
        {
            Exigir(RolesLectura);
            return Ok(await Logic.ObtenerAsync(id, Tenant, ct));
        }

        [HttpPost]
        public virtual async Task<ActionResult<Mensaje>> Post([FromBody] T entidad, CancellationToken ct)
        {
            Exigir(RolesEscritura);
            var result = await Logic.CrearAsync(entidad, Tenant, ct);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<Mensaje>> Put(string id, [FromBody] T entidad, CancellationToken ct)
        {
            Exigir(RolesEscritura);
            return Ok(await Logic.ActualizarAsync(id, entidad, Tenant, ct));
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<Mensaje>> Delete(string id, CancellationToken ct)
        {
            Exigir(RolesEscritura);
            return Ok(await Logic.EliminarAsync(id, Tenant, ct));
        }

        [HttpPatch("{id}/estado")]
        public virtual async Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] bool estado, CancellationToken ct)
        {
            Exigir(RolesEscritura);
            return Ok(await Logic.CambiarEstadoAsync(id, estado, Tenant, ct));
        }

        protected void Exigir(string[] roles)
        {
            if (!Tenant.TieneRol(roles))
                throw new ForbiddenException("No tiene permisos para esta operación.");
        }
    }
}

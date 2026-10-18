using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Sync.Common.Services;
using Sync.Common.Web;

namespace Controllers.Endpoints
{
    // Todos los controladores heredan el CRUD estándar de CrudController:
    //   GET / · GET /{id} · POST / · PUT /{id} · DELETE /{id} · PATCH /{id}/estado
    // con aislamiento por compañía (tenant del JWT) y control de roles.

    [Route("api/Comp")]
    public class CompController(CompLogical logic) : CrudController<Comp, CompFiltro>
    {
        protected override CrudLogical<Comp, CompFiltro> Logic => logic;

        // Crear, eliminar o desactivar compañías es exclusivo de Root; Admin puede editar la suya.
        public override Task<ActionResult<Mensaje>> Post([FromBody] Comp entidad, CancellationToken ct)
        {
            Exigir(Roles.SoloRoot);
            return base.Post(entidad, ct);
        }

        public override Task<ActionResult<Mensaje>> Delete(string id, CancellationToken ct)
        {
            Exigir(Roles.SoloRoot);
            return base.Delete(id, ct);
        }

        public override Task<ActionResult<Mensaje>> PatchEstado(string id, [FromBody] bool estado, CancellationToken ct)
        {
            Exigir(Roles.SoloRoot);
            return base.PatchEstado(id, estado, ct);
        }
    }

    [Route("api/Esp")]
    public class EspController(EspLogical logic) : CrudController<Esp, EspFiltro>
    {
        protected override CrudLogical<Esp, EspFiltro> Logic => logic;
    }

    [Route("api/Planta")]
    public class PlantaController(PlantaLogical logic) : CrudController<Planta, PlantaFiltro>
    {
        protected override CrudLogical<Planta, PlantaFiltro> Logic => logic;
    }

    [Route("api/TipoArea")]
    public class TipoAreaController(TipoAreaLogical logic) : CrudController<TipoArea, TipoAreaFiltro>
    {
        protected override CrudLogical<TipoArea, TipoAreaFiltro> Logic => logic;
        protected override string[] RolesEscritura => Roles.SoloRoot;
    }

    [Route("api/Area")]
    public class AreaController(AreaLogical logic) : CrudController<Area, AreaFiltro>
    {
        protected override CrudLogical<Area, AreaFiltro> Logic => logic;
    }

    [Route("api/SeccionArea")]
    public class SeccionAreaController(SeccionAreaLogical logic) : CrudController<SeccionArea, SeccionAreaFiltro>
    {
        protected override CrudLogical<SeccionArea, SeccionAreaFiltro> Logic => logic;
    }

    [Route("api/Equipo")]
    public class EquipoController(EquipoLogical logic) : CrudController<Equipo, EquipoFiltro>
    {
        protected override CrudLogical<Equipo, EquipoFiltro> Logic => logic;

        /// <summary>Historial de ubicaciones del equipo.</summary>
        [HttpGet("{id}/ubicaciones")]
        public async Task<ActionResult<List<UbicacionEquipo>>> Ubicaciones(string id, CancellationToken ct)
        {
            Exigir(RolesLectura);
            return Ok(await logic.UbicacionesAsync(id, Tenant, ct));
        }
    }

    [Route("api/Guia")]
    public class GuiaController(GuiaLogical logic) : CrudController<Guia, GuiaFiltro>
    {
        protected override CrudLogical<Guia, GuiaFiltro> Logic => logic;

        /// <summary>Detalle con el árbol completo de procedimientos y validaciones.</summary>
        public override async Task<ActionResult<Guia>> GetById(string id, CancellationToken ct)
        {
            Exigir(RolesLectura);
            return Ok(await logic.DetalleAsync(id, Tenant, ct));
        }
    }

    [Route("api/Proced")]
    public class ProcedController(ProcedLogical logic) : CrudController<Proced, ProcedFiltro>
    {
        protected override CrudLogical<Proced, ProcedFiltro> Logic => logic;
    }

    [Route("api/Valid")]
    public class ValidController(ValidLogical logic) : CrudController<Valid, ValidFiltro>
    {
        protected override CrudLogical<Valid, ValidFiltro> Logic => logic;
    }

    [Route("api/GuiaEquipo")]
    public class GuiaEquipoController(GuiaEquipoLogical logic) : CrudController<GuiaEquipo, GuiaEquipoFiltro>
    {
        protected override CrudLogical<GuiaEquipo, GuiaEquipoFiltro> Logic => logic;
    }

    [Route("api/TipoActividad")]
    public class TipoActividadController(TipoActividadLogical logic) : CrudController<TipoActividad, TipoActividadFiltro>
    {
        protected override CrudLogical<TipoActividad, TipoActividadFiltro> Logic => logic;
    }

    [Route("api/Actividad")]
    public class ActividadController(ActividadLogical logic) : CrudController<Actividad, ActividadFiltro>
    {
        protected override CrudLogical<Actividad, ActividadFiltro> Logic => logic;
        // La programación de actividades la hacen también los supervisores
        protected override string[] RolesEscritura => Roles.Operacion;
    }

    [Route("api/ActividadEquipo")]
    public class ActividadEquipoController(ActividadEquipoLogical logic, EjecucionLogical ejecucion)
        : CrudController<ActividadEquipo, ActividadEquipoFiltro>
    {
        protected override CrudLogical<ActividadEquipo, ActividadEquipoFiltro> Logic => logic;
        protected override string[] RolesEscritura => Roles.Operacion;

        // Ejecución en campo: la registran todos los roles (técnicos incluidos)

        /// <summary>Resultados registrados del checklist.</summary>
        [HttpGet("{id}/resultados")]
        public async Task<ActionResult<List<EjecucionValid>>> Resultados(string id, CancellationToken ct)
        {
            Exigir(Roles.Todos);
            return Ok(await ejecucion.ResultadosAsync(id, Tenant, ct));
        }

        /// <summary>Marca (o desmarca) una verificación de la guía.</summary>
        [HttpPut("{id}/resultados/{idValid}")]
        public async Task<ActionResult<Mensaje>> Marcar(string id, string idValid, [FromBody] ResultadoValid resultado, CancellationToken ct)
        {
            Exigir(Roles.Todos);
            return Ok(await ejecucion.MarcarAsync(id, idValid, resultado, Tenant, ct));
        }

        /// <summary>Finaliza la ejecución: exige todas las verificaciones y las fotos que pide la guía.</summary>
        [HttpPost("{id}/finalizar")]
        public async Task<ActionResult<Mensaje>> Finalizar(string id, [FromBody] CierreEjecucion cierre, CancellationToken ct)
        {
            Exigir(Roles.Todos);
            return Ok(await ejecucion.FinalizarAsync(id, cierre, Tenant, ct));
        }

        [HttpPost("{id}/reabrir")]
        public async Task<ActionResult<Mensaje>> Reabrir(string id, CancellationToken ct)
        {
            Exigir(Roles.Operacion);
            return Ok(await ejecucion.ReabrirAsync(id, Tenant, ct));
        }
    }

    [Route("api/Falla")]
    public class FallaController(FallaLogical logic) : CrudController<Falla, FallaFiltro>
    {
        protected override CrudLogical<Falla, FallaFiltro> Logic => logic;
        // Atender, cerrar o eliminar reportes es tarea de supervisión
        protected override string[] RolesEscritura => Roles.Operacion;

        /// <summary>Cualquier usuario de la compañía puede reportar una falla.</summary>
        public override async Task<ActionResult<Mensaje>> Post([FromBody] Falla entidad, CancellationToken ct)
        {
            Exigir(Roles.Todos);
            var result = await logic.CrearAsync(entidad, Tenant, ct);
            return StatusCode(StatusCodes.Status201Created, result);
        }
    }

    /// <summary>
    /// Fotos de equipos, evidencias de ejecución y fotos de fallas:
    ///   GET    /?entidad=&amp;idEntidad=   metadatos
    ///   GET    /{id}/archivo              bytes de la imagen
    ///   POST   /                          multipart: entidad, idEntidad, ref?, archivo
    ///   DELETE /{id}
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/Adjunto")]
    public class AdjuntoController(AdjuntoLogical logic) : ControllerBase
    {
        private Tenant Tenant => User.Tenant();

        [HttpGet]
        public async Task<ActionResult<List<Adjunto>>> Get([FromQuery] string entidad, [FromQuery] string idEntidad, CancellationToken ct) =>
            Ok(await logic.ListarAsync(entidad, idEntidad, Tenant, ct));

        [HttpGet("{id}/archivo")]
        public async Task<IActionResult> Archivo(string id, CancellationToken ct)
        {
            var archivo = await logic.ArchivoAsync(id, Tenant, ct);
            // Los adjuntos no cambian: se pueden cachear en el navegador del usuario
            Response.Headers.CacheControl = "private, max-age=86400";
            return File(archivo.Datos, archivo.ContentType);
        }

        [HttpPost]
        [RequestSizeLimit(AdjuntoLogical.TamanoMaximo + 64 * 1024)]
        public async Task<ActionResult<Mensaje>> Post([FromForm] string entidad, [FromForm] string idEntidad, [FromForm] string? @ref,
            IFormFile archivo, CancellationToken ct)
        {
            if (archivo is null)
                throw new BusinessException("Adjunte el archivo de la foto.");
            if (archivo.Length > AdjuntoLogical.TamanoMaximo)
                throw new BusinessException("La imagen supera el tamaño máximo de 8 MB.");
            using var ms = new MemoryStream();
            await archivo.CopyToAsync(ms, ct);
            var adjunto = new Adjunto
            {
                Entidad = entidad,
                IdEntidad = idEntidad,
                Ref = @ref,
                Nombre = archivo.FileName,
                ContentType = archivo.ContentType
            };
            var result = await logic.SubirAsync(adjunto, ms.ToArray(), Tenant, ct);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Mensaje>> Delete(string id, CancellationToken ct) =>
            Ok(await logic.EliminarAsync(id, Tenant, ct));
    }
}

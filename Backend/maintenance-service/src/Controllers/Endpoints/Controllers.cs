using Entity;
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
    public class ActividadEquipoController(ActividadEquipoLogical logic) : CrudController<ActividadEquipo, ActividadEquipoFiltro>
    {
        protected override CrudLogical<ActividadEquipo, ActividadEquipoFiltro> Logic => logic;
        protected override string[] RolesEscritura => Roles.Operacion;
    }
}

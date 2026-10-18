using Data;
using Entity;
using Sync.Common.Services;
using Sync.Common.Web;

namespace Services
{
    /// <summary>
    /// Ejecución en campo de un equipo de una actividad: el técnico marca cada verificación
    /// de la guía y adjunta las fotos que la guía exige. Solo se puede finalizar con el checklist completo.
    /// </summary>
    public class EjecucionLogical(DaoActividadEquipo dao, ActividadEquipoLogical equipos, GuiaLogical guias, DaoAdjunto adjuntos)
    {
        public async Task<List<EjecucionValid>> ResultadosAsync(string id, Tenant tenant, CancellationToken ct = default)
        {
            await equipos.ObtenerAsync(id, tenant, ct);
            return await dao.GetResultadosAsync(id, ct);
        }

        public async Task<Mensaje> MarcarAsync(string id, string idValid, ResultadoValid resultado, Tenant tenant, CancellationToken ct = default)
        {
            var ae = await equipos.ObtenerAsync(id, tenant, ct);
            NoFinalizada(ae);
            var guia = await GuiaDeAsync(ae, tenant, ct);
            if (!Verificaciones(guia).Any(v => v.Id == idValid))
                throw new NotFoundException("La verificación no pertenece a la guía de esta actividad.");

            await dao.SetResultadoAsync(new EjecucionValid
            {
                Id = Guid.NewGuid().ToString(),
                IdActividadEquipo = id,
                IdValid = idValid,
                Cumple = resultado.Cumple,
                Observacion = resultado.Observacion,
                IdUser = tenant.UserId
            }, ct);
            return new Mensaje("Verificación registrada.", id);
        }

        public async Task<Mensaje> FinalizarAsync(string id, CierreEjecucion cierre, Tenant tenant, CancellationToken ct = default)
        {
            var ae = await equipos.ObtenerAsync(id, tenant, ct);
            NoFinalizada(ae);

            if (!string.IsNullOrWhiteSpace(ae.IdGuia))
            {
                var verificaciones = Verificaciones(await GuiaDeAsync(ae, tenant, ct));
                var cumplidas = (await dao.GetResultadosAsync(id, ct)).Where(r => r.Cumple).Select(r => r.IdValid).ToHashSet();
                var pendientes = verificaciones.Count(v => !cumplidas.Contains(v.Id!));
                if (pendientes > 0)
                    throw new BusinessException($"Faltan {pendientes} verificaciones por completar.");

                var conFoto = (await adjuntos.GetAsync(null, null, Adjunto.Ejecucion, id, ct)).Select(a => a.Ref).ToHashSet();
                var sinFoto = verificaciones.Count(v => v.RequiereFoto && !conFoto.Contains(v.Id));
                if (sinFoto > 0)
                    throw new BusinessException($"Faltan {sinFoto} fotos de evidencia que exige la guía.");
            }

            await dao.SetEjecucionAsync(id, ActividadEquipo.Completada, tenant.UserId, cierre.Observacion, ct);
            return new Mensaje("Actividad finalizada.", id);
        }

        /// <summary>Vuelve a abrir una ejecución finalizada (corrección por parte de un supervisor).</summary>
        public async Task<Mensaje> ReabrirAsync(string id, Tenant tenant, CancellationToken ct = default)
        {
            var ae = await equipos.ObtenerAsync(id, tenant, ct);
            if (ae.EstadoEjec != ActividadEquipo.Completada)
                throw new BusinessException("La ejecución no está finalizada.");
            await dao.SetEjecucionAsync(id, ActividadEquipo.EnCurso, null, null, ct);
            return new Mensaje("Ejecución reabierta.", id);
        }

        private async Task<Guia> GuiaDeAsync(ActividadEquipo ae, Tenant tenant, CancellationToken ct) =>
            string.IsNullOrWhiteSpace(ae.IdGuia)
                ? throw new BusinessException("Este equipo no tiene una guía asignada en la actividad.")
                : await guias.DetalleAsync(ae.IdGuia, tenant, ct);

        private static List<Valid> Verificaciones(Guia guia) =>
            (guia.proced ?? []).SelectMany(p => p.valid ?? []).ToList();

        private static void NoFinalizada(ActividadEquipo ae)
        {
            if (ae.EstadoEjec == ActividadEquipo.Completada)
                throw new BusinessException("La ejecución ya fue finalizada.");
        }
    }

    /// <summary>Reportes de falla. Cualquier usuario de la compañía puede reportar; la gestión es de supervisión.</summary>
    public class FallaLogical(DaoFalla dao, EquipoLogical equipos) : CrudLogical<Falla, FallaFiltro>(dao)
    {
        protected override string Nombre => "Reporte de falla";

        protected override async Task PrepararAsync(Falla e, Falla? existente, Tenant tenant, CancellationToken ct)
        {
            if (existente != null)
            {
                e.IdEquipo = existente.IdEquipo;
                e.IdUserReporta = existente.IdUserReporta;
                if (!Falla.Estados.Contains(e.EstadoFalla))
                    throw new BusinessException($"EstadoFalla debe ser uno de: {string.Join(", ", Falla.Estados)}.");
            }
            else
            {
                e.EstadoFalla = "Abierta";
                e.IdUserReporta = tenant.UserId;
            }
            Requerido(e.IdEquipo, "IdEquipo");
            if (!Falla.Tipos.Contains(e.Tipo))
                throw new BusinessException($"Tipo debe ser uno de: {string.Join(", ", Falla.Tipos)}.");
            if (!Falla.Prioridades.Contains(e.Prioridad))
                throw new BusinessException($"Prioridad debe ser una de: {string.Join(", ", Falla.Prioridades)}.");

            var equipo = await equipos.ObtenerAsync(e.IdEquipo, tenant, ct);
            if (existente == null) e.IdComp = equipo.IdComp;
            await base.PrepararAsync(e, existente, tenant, ct);
            if (e.IdComp != equipo.IdComp)
                throw new BusinessException("El equipo no pertenece a la compañía.");
        }
    }

    /// <summary>Fotos de equipos, evidencias de ejecución y fotos de fallas, siempre dentro de la compañía del usuario.</summary>
    public class AdjuntoLogical(
        DaoAdjunto dao,
        EquipoLogical equipos,
        ActividadEquipoLogical actividadEquipos,
        ActividadLogical actividades,
        FallaLogical fallas)
    {
        public const int TamanoMaximo = 8 * 1024 * 1024;
        public static readonly string[] TiposPermitidos = ["image/jpeg", "image/png", "image/webp"];

        public async Task<List<Adjunto>> ListarAsync(string entidad, string idEntidad, Tenant tenant, CancellationToken ct = default)
        {
            await CompaniaDeAsync(entidad, idEntidad, tenant, ct);
            return await dao.GetAsync(null, tenant.Filtro(null), entidad, idEntidad, ct);
        }

        public async Task<ArchivoAdjunto> ArchivoAsync(string id, Tenant tenant, CancellationToken ct = default) =>
            await dao.GetDatosAsync(id, tenant.Filtro(null), ct) ?? throw new NotFoundException("Adjunto: registro no encontrado.");

        public async Task<Mensaje> SubirAsync(Adjunto a, byte[] datos, Tenant tenant, CancellationToken ct = default)
        {
            if (datos.Length == 0)
                throw new BusinessException("El archivo está vacío.");
            if (datos.Length > TamanoMaximo)
                throw new BusinessException("La imagen supera el tamaño máximo de 8 MB.");
            if (!TiposPermitidos.Contains(a.ContentType))
                throw new BusinessException("Solo se permiten imágenes JPG, PNG o WebP.");

            if (a.Entidad == Adjunto.Ejecucion)
            {
                if (string.IsNullOrWhiteSpace(a.Ref))
                    throw new BusinessException("Indique la verificación (Ref) que evidencia la foto.");
                await NoFinalizadaAsync(a.IdEntidad, tenant, ct);
            }
            else a.Ref = null;

            a.IdComp = await CompaniaDeAsync(a.Entidad, a.IdEntidad, tenant, ct);
            a.Id = Guid.NewGuid().ToString();
            a.IdUser = tenant.UserId;
            a.Nombre = NombreSeguro(a.Nombre);
            await dao.SetAsync(a, datos, ct);
            return new Mensaje("Foto guardada.", a.Id);
        }

        public async Task<Mensaje> EliminarAsync(string id, Tenant tenant, CancellationToken ct = default)
        {
            var a = (await dao.GetAsync(id, tenant.Filtro(null), null, null, ct)).FirstOrDefault()
                ?? throw new NotFoundException("Adjunto: registro no encontrado.");
            if (!tenant.TieneRol(Roles.Operacion) && a.IdUser != tenant.UserId)
                throw new ForbiddenException("Solo quien subió la foto o un supervisor puede eliminarla.");
            if (a.Entidad == Adjunto.Ejecucion)
                await NoFinalizadaAsync(a.IdEntidad, tenant, ct);
            await dao.DeleteAsync(id, ct);
            return new Mensaje("Foto eliminada.", id);
        }

        /// <summary>Verifica que la entidad dueña es visible para el usuario y devuelve su compañía.</summary>
        private async Task<string> CompaniaDeAsync(string entidad, string idEntidad, Tenant tenant, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(idEntidad))
                throw new BusinessException("El campo IdEntidad es obligatorio.");
            return entidad switch
            {
                Adjunto.Equipo => (await equipos.ObtenerAsync(idEntidad, tenant, ct)).IdComp!,
                Adjunto.Falla => (await fallas.ObtenerAsync(idEntidad, tenant, ct)).IdComp!,
                Adjunto.Ejecucion => (await actividades.ObtenerAsync(
                    (await actividadEquipos.ObtenerAsync(idEntidad, tenant, ct)).IdActividad, tenant, ct)).IdComp!,
                _ => throw new BusinessException($"Entidad debe ser una de: {string.Join(", ", Adjunto.Entidades)}.")
            };
        }

        private async Task NoFinalizadaAsync(string idActividadEquipo, Tenant tenant, CancellationToken ct)
        {
            var ae = await actividadEquipos.ObtenerAsync(idActividadEquipo, tenant, ct);
            if (ae.EstadoEjec == ActividadEquipo.Completada)
                throw new BusinessException("La ejecución ya fue finalizada: no se pueden cambiar sus fotos.");
        }

        private static string NombreSeguro(string? nombre)
        {
            var limpio = Path.GetFileName(nombre ?? string.Empty).Trim();
            if (limpio.Length == 0) limpio = "foto";
            return limpio.Length > 255 ? limpio[..255] : limpio;
        }
    }
}

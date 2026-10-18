using Sync.Common.Data;

namespace Entity
{
    /// <summary>Tipo de actividad (preventiva, correctiva, inspección...).</summary>
    public class TipoActividad : IEntidadCompania
    {
        public string? Id { get; set; }
        public string? IdComp { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class TipoActividadFiltro : FiltroBase
    {
        public string? Nombre { get; set; }
    }

    /// <summary>Actividad de mantenimiento ejecutada o programada por una cuadrilla.</summary>
    public class Actividad : IEntidadCompania
    {
        public string? Id { get; set; }
        public string? IdComp { get; set; }
        public string? Descripcion { get; set; }
        public string IdTipoActividad { get; set; } = string.Empty;
        public string? TipoActividadNombre { get; set; }
        public string? Ubicacion { get; set; }
        public DateTime? FechaEjecucion { get; set; }
        public string? IdCuad { get; set; }
        public string? CuadNombre { get; set; }
        public string? Detalle { get; set; }
        public int Intervalo { get; set; }
        public int NumEquipos { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class ActividadFiltro : FiltroBase
    {
        public string? IdTipoActividad { get; set; }
        public string? IdCuad { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }

    /// <summary>Equipo intervenido en una actividad (opcionalmente con la guía usada) y su ejecución en campo.</summary>
    public class ActividadEquipo : IEntidad
    {
        public const string Pendiente = "Pendiente";
        public const string EnCurso = "EnCurso";
        public const string Completada = "Completada";

        public string? Id { get; set; }
        public string IdActividad { get; set; } = string.Empty;
        public string IdEquipo { get; set; } = string.Empty;
        public string? IdGuia { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }

        // Solo lectura (desnormalizado para mostrar)
        public string? EquipoNombre { get; set; }
        public string? EquipoSerie { get; set; }
        public string? GuiaNombre { get; set; }
        public int GuiaDuracion { get; set; }
        public DateTime? FechaEjecucion { get; set; }
        public string? ActividadDescripcion { get; set; }
        public string? TipoActividadNombre { get; set; }
        public string? IdCuad { get; set; }
        public string? CuadNombre { get; set; }
        public string? UbicacionNombre { get; set; }

        // Ejecución: se modifica con los endpoints de ejecución, no con PUT
        public string EstadoEjec { get; set; } = Pendiente;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? IdUserEjec { get; set; }
        public string? Observacion { get; set; }
        public int TotalValid { get; set; }
        public int ValidCumplidas { get; set; }
    }

    public class ActividadEquipoFiltro : FiltroBase
    {
        public string? IdActividad { get; set; }
        public string? IdEquipo { get; set; }
        public string? IdGuia { get; set; }
        public string? IdCuad { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? EstadoEjec { get; set; }
    }

    /// <summary>Resultado de una verificación del checklist durante la ejecución.</summary>
    public class EjecucionValid
    {
        public string? Id { get; set; }
        public string IdActividadEquipo { get; set; } = string.Empty;
        public string IdValid { get; set; } = string.Empty;
        public bool Cumple { get; set; }
        public string? Observacion { get; set; }
        public string? IdUser { get; set; }
        public DateTime? Fecha { get; set; }
    }

    /// <summary>Cuerpo de PUT /ActividadEquipo/{id}/resultados/{idValid}.</summary>
    public class ResultadoValid
    {
        public bool Cumple { get; set; }
        public string? Observacion { get; set; }
    }

    /// <summary>Cuerpo de POST /ActividadEquipo/{id}/finalizar.</summary>
    public class CierreEjecucion
    {
        public string? Observacion { get; set; }
    }
}

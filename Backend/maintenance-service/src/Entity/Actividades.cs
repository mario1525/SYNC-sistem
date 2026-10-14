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

    /// <summary>Equipo intervenido en una actividad (opcionalmente con la guía usada).</summary>
    public class ActividadEquipo : IEntidad
    {
        public string? Id { get; set; }
        public string IdActividad { get; set; } = string.Empty;
        public string IdEquipo { get; set; } = string.Empty;
        public string? IdGuia { get; set; }
        public string? EquipoNombre { get; set; }
        public string? GuiaNombre { get; set; }
        public DateTime? FechaEjecucion { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class ActividadEquipoFiltro : FiltroBase
    {
        public string? IdActividad { get; set; }
        public string? IdEquipo { get; set; }
        public string? IdGuia { get; set; }
    }
}

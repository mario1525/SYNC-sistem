using Sync.Common.Data;

namespace Entity
{
    /// <summary>Guía de mantenimiento: cómo se interviene un tipo de equipo.</summary>
    public class Guia : IEntidadCompania
    {
        public string? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Proceso { get; set; }
        public string? Inspeccion { get; set; }
        public string? Herramientas { get; set; }
        public string? IdComp { get; set; }
        public string? IdEsp { get; set; }
        public string? EspNombre { get; set; }
        public string? SeguridadInd { get; set; }
        public string? SeguridadAmb { get; set; }
        /// <summary>Frecuencia sugerida en días.</summary>
        public int Intervalo { get; set; }
        public string? Importante { get; set; }
        public string? Insumos { get; set; }
        /// <summary>Número de personas necesarias.</summary>
        public int Personal { get; set; }
        /// <summary>Duración estimada en horas.</summary>
        public int Duracion { get; set; }
        public string? Logistica { get; set; }
        public string? Situacion { get; set; }
        public string? Notas { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? FechaUpdate { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }

        /// <summary>Pasos de la guía. En escritura: null = no modificar los pasos.</summary>
        public List<Proced>? proced { get; set; }
    }

    public class GuiaFiltro : FiltroBase
    {
        public string? IdEsp { get; set; }
        public string? Nombre { get; set; }
    }

    /// <summary>Paso (procedimiento) de una guía.</summary>
    public class Proced : IEntidad
    {
        public string? Id { get; set; }
        public string IdGuia { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
        public List<Valid>? valid { get; set; }
    }

    public class ProcedFiltro : FiltroBase
    {
        public string? IdGuia { get; set; }
        public string? Nombre { get; set; }
    }

    /// <summary>Punto de verificación de un paso (ítem de checklist).</summary>
    public class Valid : IEntidad
    {
        public string? Id { get; set; }
        public string IdProced { get; set; } = string.Empty;
        public string? IdGuia { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class ValidFiltro : FiltroBase
    {
        public string? IdProced { get; set; }
        public string? IdGuia { get; set; }
        public string? Nombre { get; set; }
    }

    /// <summary>Asignación de una guía a un equipo.</summary>
    public class GuiaEquipo : IEntidad
    {
        public string? Id { get; set; }
        public string IdGuia { get; set; } = string.Empty;
        public string IdEquipo { get; set; } = string.Empty;
        public string? GuiaNombre { get; set; }
        public string? EquipoNombre { get; set; }
        public int Intervalo { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class GuiaEquipoFiltro : FiltroBase
    {
        public string? IdGuia { get; set; }
        public string? IdEquipo { get; set; }
    }
}

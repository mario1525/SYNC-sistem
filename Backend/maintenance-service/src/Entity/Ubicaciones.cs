using Sync.Common.Data;

namespace Entity
{
    /// <summary>Planta física de una compañía.</summary>
    public class Planta : IEntidadCompania
    {
        public string? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Region { get; set; }
        public string? IdComp { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class PlantaFiltro : FiltroBase
    {
        public string? Nombre { get; set; }
        public string? Region { get; set; }
    }

    /// <summary>Tipo de área (catálogo global): Produccion, Bodega, Patio...</summary>
    public class TipoArea : IEntidad
    {
        public const string Produccion = "00000000-0000-0000-0000-0000000000a1";
        public const string Bodega = "00000000-0000-0000-0000-0000000000a2";
        public const string Patio = "00000000-0000-0000-0000-0000000000a3";

        public string? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class TipoAreaFiltro : FiltroBase
    {
        public string? Nombre { get; set; }
    }

    /// <summary>Área dentro de una planta (de producción, bodega o patio).</summary>
    public class Area : IEntidad
    {
        public string? Id { get; set; }
        public string IdPlanta { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string TipoArea { get; set; } = string.Empty;
        public string? TipoAreaNombre { get; set; }
        public string? PlantaNombre { get; set; }
        public string? IdComp { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class AreaFiltro : FiltroBase
    {
        public string? IdPlanta { get; set; }
        public string? Nombre { get; set; }
        public string? TipoArea { get; set; }
    }

    /// <summary>Sección dentro de un área: el nivel donde se ubica físicamente un equipo.</summary>
    public class SeccionArea : IEntidad
    {
        public string? Id { get; set; }
        public string IdArea { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? AreaNombre { get; set; }
        public string? IdPlanta { get; set; }
        public string? PlantaNombre { get; set; }
        public string? TipoAreaNombre { get; set; }
        public string? IdComp { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class SeccionAreaFiltro : FiltroBase
    {
        public string? IdArea { get; set; }
        public string? IdPlanta { get; set; }
        public string? Nombre { get; set; }
    }
}

using Sync.Common.Data;

namespace Entity
{
    /// <summary>Compañía (tenant).</summary>
    public class Comp : IEntidad
    {
        public string? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Ciudad { get; set; }
        public string? NIT { get; set; }
        public string? Direccion { get; set; }
        public string? Sector { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class CompFiltro : FiltroBase
    {
        public string? Nombre { get; set; }
        public string? NIT { get; set; }
    }

    /// <summary>Especialidad (mecánica, eléctrica...). IdComp null = catálogo global.</summary>
    public class Esp : IEntidad
    {
        public string? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? IdComp { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class EspFiltro : FiltroBase
    {
        public string? Nombre { get; set; }
    }
}

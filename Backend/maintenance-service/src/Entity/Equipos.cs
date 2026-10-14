using Sync.Common.Data;

namespace Entity
{
    /// <summary>Equipo / activo industrial.</summary>
    public class Equipo : IEntidadCompania
    {
        public string? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? IdComp { get; set; }
        public string? Modelo { get; set; }
        public string? NSerie { get; set; }
        public string? Fabricante { get; set; }
        public string? Marca { get; set; }
        public string? Funcion { get; set; }
        public int Peso { get; set; }
        public int Cilindraje { get; set; }
        public int Potencia { get; set; }
        public int Ancho { get; set; }
        public int Alto { get; set; }
        public int Largo { get; set; }
        public int Capacidad { get; set; }
        public int AnioFabricacion { get; set; }
        public string? Caracteristicas { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }

        /// <summary>Ubicación vigente. En escritura: null = no cambiar.</summary>
        public UbicacionEquipo? Ubicacion { get; set; }
    }

    /// <summary>Ubicación de un equipo: en producción o almacenado, en una sección de área.</summary>
    public class UbicacionEquipo
    {
        public const string Produccion = "Produccion";
        public const string Almacen = "Almacen";
        public static readonly string[] Tipos = [Produccion, Almacen];

        public string? Id { get; set; }
        public string TipoUbicacion { get; set; } = Produccion;
        public string? IdSeccionArea { get; set; }

        // Solo lectura (desnormalizado para mostrar)
        public string? SeccionNombre { get; set; }
        public string? IdArea { get; set; }
        public string? AreaNombre { get; set; }
        public string? IdPlanta { get; set; }
        public string? PlantaNombre { get; set; }
        public string? TipoAreaNombre { get; set; }
        public bool Vigente { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class EquipoFiltro : FiltroBase
    {
        public string? Nombre { get; set; }
        public string? Marca { get; set; }
        public string? NSerie { get; set; }
        public string? IdPlanta { get; set; }
        public string? IdArea { get; set; }
        public string? IdSeccionArea { get; set; }
    }
}

namespace Entity
{
    public class UbicacionEquipo
    {
        public string Id { get; set; }              // Identificador único de la ubicación del equipo
        public string IdEquipo { get; set; }        // FK del equipo asociado
        public string TipoUbicacion { get; set; }   // Tipo de ubicación (Planta, Bodega, Patio, etc.)
        public string IdPlanta { get; set; }        // FK de la planta asociada
        public string? IdAreaFuncional { get; set; } // FK del área funcional asociada (opcional)
        public string? IdBodega { get; set; }        // FK de la bodega asociada (opcional)
        public string? IdSeccionBodega { get; set; } // FK de la sección de la bodega asociada (opcional)
        public string? IdPatio { get; set; }         // FK del patio asociado (opcional)
        public bool? Estado { get; set; }            // Estado (activo/inactivo)
        public DateTime? Fecha_log { get; set; }     // Fecha de creación o última modificación
    }
}
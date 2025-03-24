namespace Entity
{
    public class UbicacionEquipo
    {
        public string Id { get; set; }              // Id interno del registro
        public string IdEquipo { get; set; }        // FK del equipo
        public string TipoUbicacion { get; set; }   // Tipo de ubicación (Planta, Bodega, Patio, etc.)
        public string IdPlanta { get; set; }        // FK de la planta
        public string IdAreaFuncional { get; set; } // FK del área funcional (opcional)
        public string IdBodega { get; set; }        // FK de la bodega (opcional)
        public string IdSeccionBodega { get; set; } // FK de la sección de la bodega (opcional)
        public string IdPatio { get; set; }         // FK del patio (opcional)
        public bool Estado { get; set; }            // Estado (activo/inactivo)
        public DateTime Fecha_log { get; set; }     // Fecha de registro o última modificación
    }
}
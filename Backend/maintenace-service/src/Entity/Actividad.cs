namespace Entity
{
    public class Actividad
    {
        public string Id { get; set; }              // Id interno del registro
        public string Descripcion { get; set; }     // Descripción de la actividad
        public string IdTipoActividad { get; set; } // FK de la tabla TipoActividad
        public string Ubicacion { get; set; }       // Ubicación de la actividad
        public DateTime FechaEjecucion { get; set; }// Fecha de ejecución de la actividad
        public string IdCuad { get; set; }          // FK de la tabla Cuadrilla
        public string Detalle { get; set; }         // Detalle de la actividad
        public int Intervalo { get; set; }          // Intervalo de la actividad
        public bool Estado { get; set; }            // Estado
        public DateTime Fecha_log { get; set; }     // Log fecha
    }
}
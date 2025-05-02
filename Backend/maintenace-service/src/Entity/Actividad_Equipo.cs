namespace Entity
{
    public class ActividadEquipo
    {
        public string? Id { get; set; }              // Id interno del registro
        public string IdActividad { get; set; }     // FK de la tabla Actividad
        public string IdEquipo { get; set; }        // FK de la tabla Equipo
        public string IdGuia { get; set; }          // FK de la tabla Guia
        public bool Estado { get; set; }            // Estado
        public DateTime? Fecha_log { get; set; }     // Log fecha
    }
}
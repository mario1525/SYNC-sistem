namespace Entity
{
    public class Guia_Equipo
    {
        public string? Id { get; set; }              // Id interno del registro
        public string? IdGuia { get; set; }          // FK de la tabla Guia
        public string? IdEquipo { get; set; }        // FK de la tabla Equipo
        public bool Estado { get; set; }            // Estad
        public DateTime? Fecha_log { get; set; }     // Log fecha
    }
}
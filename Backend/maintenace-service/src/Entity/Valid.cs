namespace Entity
{
    public class Valid
    {
        public string Id { get; set; }              // Id interno del registro
        public string IdProced { get; set; }        // FK de la tabla Procedimiento
        public string Nombre { get; set; }          // Nombre de la validación
        public string Descripcion { get; set; }     // Descripción de la validación
        public bool Estado { get; set; }            // Estado
        
        public DateTime Fecha_log { get; set; }     // Log fecha
    }
}
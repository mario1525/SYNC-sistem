namespace Entity
{
    public class Esp
    {
        public string Id { get; set; }              // Id interno del registro
        public string Nombre { get; set; }          // Nombre 
        public string IdComp { get; set; }          // FK de la tabla Compania
        public bool Estado { get; set; }            // Estado
        public DateTime Fecha_log { get; set; }      // Log fecha
    }
}
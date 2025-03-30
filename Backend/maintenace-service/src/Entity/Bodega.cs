namespace Entity
{
    public class Bodega
    {
        public string? Id { get; set; }              // Identificador único de la bodega
        public string? IdPlanta { get; set; }        // FK de la planta asociada
        public string? Nombre { get; set; }          // Nombre de la bodega
        public bool? Estado { get; set; }            // Estado (activo/inactivo)
        public DateTime? Fecha_log { get; set; }     // Fecha de creación o última modificación
    }
}
namespace Entity
{
    public class Patio
    {
        public string Id { get; set; }              // Identificador único del patio
        public string IdBodega { get; set; }        // FK de la bodega asociada
        public string Nombre { get; set; }          // Nombre del patio
        public bool Estado { get; set; }            // Estado (activo/inactivo)
        public DateTime Fecha_log { get; set; }     // Fecha de creación o última modificación
    }
}
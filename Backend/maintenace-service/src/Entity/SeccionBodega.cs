namespace Entity
{
    public class SeccionBodega
    {
        public string Id { get; set; }              // Identificador único de la sección de bodega
        public string IdBodega { get; set; }        // FK de la bodega asociada
        public string Nombre { get; set; }          // Nombre de la sección de bodega
        public bool Estado { get; set; }            // Estado (activo/inactivo)
        public DateTime Fecha_log { get; set; }     // Fecha de creación o última modificación
    }
}
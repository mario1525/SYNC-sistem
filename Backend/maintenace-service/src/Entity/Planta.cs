namespace Entity
{
    public class Planta
    {
        public string? Id { get; set; }              // Identificador único de la planta
        public string? Nombre { get; set; }          // Nombre de la planta
        public string? Region { get; set; }          // Región donde se encuentra la planta
        public string IdComp { get; set; }          // FK de la compañía asociada
        public bool Estado { get; set; }            // Estado (activo/inactivo)
        public DateTime? Fecha_log { get; set; }     // Fecha de creación o última modificación
    }
}
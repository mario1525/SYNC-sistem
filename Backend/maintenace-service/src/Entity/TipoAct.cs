namespace Entity
{
    public class TipoAct
    {
        public string Id { get; set; }              // Id interno del registro
        public string IdComp { get; set; }          // FK de la tabla Compania
        public string Nombre { get; set; }          // Nombre del tipo de actividad
        public string Descripcion { get; set; }     // Descripción del tipo de actividad
        public bool Estado { get; set; }            // Estado
       
        public DateTime Fecha_log { get; set; }     // Log fecha
    }
}
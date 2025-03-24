namespace Entity
{
    public class AreaFuncional
    {
        public string Id { get; set; }              // Identificador único del área funcional
        public string IdPlanta { get; set; }        // FK de la planta asociada
        public string Nombre { get; set; }          // Nombre del área funcional
        public bool Estado { get; set; }            // Estado (activo/inactivo)
        public DateTime Fecha_log { get; set; }     // Fecha de creación o última modificación
    }
}
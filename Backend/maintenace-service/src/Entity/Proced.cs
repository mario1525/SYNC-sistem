namespace Entity
{
    public class Proced
    {
        public string? Id { get; set; }              // Id interno del registro
        public string IdGuia { get; set; }          // FK de la tabla Guia
        public string? Nombre { get; set; }          // Nombre del procedimiento
        public string? Descripcion { get; set; }     // Descripcion del procedimiento
        public bool Estado { get; set; }            // Estado        
        public DateTime? Fecha_log { get; set; }     // Log fecha
    }

     public class ProcedC
    {
        public string Id { get; set; }              // Id interno del registro
        public string IdGuia { get; set; }          // FK de la tabla Guia
        public string Nombre { get; set; }          // Nombre del procedimiento
        public string Descripcion { get; set; }     // Descripcion del procedimiento
        public bool Estado { get; set; }            // Estado         
        public DateTime Fecha_log { get; set; }     // Log fecha
        public Valid[] valid { get; set; }
    }

}


namespace Entity
{
    public class Guia
    {
        public string? Id { get; set; }              // Id interno del registro
        public string? Nombre { get; set; }          // Nombre de la guia
        public string? Descripcion { get; set; }     // Descripcion de la guia
        public string? Proceso { get; set; }         // Proceso de la guia
        public string? Inspeccion { get; set; }      // Inspeccion de la guia
        public string? Herramientas { get; set; }    // Herramientas de la guia
        public string? IdComp { get; set; }          // FK de la tabla Compania
        public string? IdEsp { get; set; }           // FK de la tabla Especialidad
        public string? SeguridadInd { get; set; }    // Seguridad industrial de la guia
        public string? SeguridadAmb { get; set; }    // Seguridad ambiental de la guia
        public int? Intervalo { get; set; }          // Intervalo de la guia
        public string? Importante { get; set; }      // Informacion importante de la guia
        public string? Insumos { get; set; }         // Insumos de la guia
        public int? Personal { get; set; }           // N personas necesarias para ejecutar la guia
        public int? Duracion { get; set; }           // Duracion de la guia en horas
        public string? Logistica { get; set; }       // Logistica de la guia
        public string? Situacion { get; set; }       // Situacion
        public string? Notas { get; set; }           // Notas de la guia
        public string? CreatedBy { get; set; }       // Usuario que creo la guia
        public string? UpdatedBy { get; set; }       // Usuario que actualizo la guia
        public DateTime?  FechaUpdate { get; set; }   // Fecha de actualizacion de la guia
        public bool? Estado { get; set; }            // Estado
        public DateTime? Fecha_log { get; set; }     // Log fecha
    }
}

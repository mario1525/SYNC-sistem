namespace Entity
{
    public class Usuario
    {
        public string Id { get; set; }
        public long Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string IdCompania { get; set; }
        public string Cargo { get; set; }
        public string Rol { get; set; }
        public string Estado { get; set; }       
        public string Fecha_log { get; set; }

        // Constructor vacío
        public Usuario() { }
    }
}
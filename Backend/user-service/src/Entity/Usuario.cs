namespace Entity
{
    public class Users
    {
        public string Id { get; set; }
        public long Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string IdComp { get; set; }
        public string IdEsp { get; set; }
        public string IdCuad {  get; set; }
        public string Cargo { get; set; }
        public string Rol { get; set; }
        public bool Estado { get; set; }       
        public string Fecha_log { get; set; }

        // Constructor vacío
        public Users() { }
    }
}
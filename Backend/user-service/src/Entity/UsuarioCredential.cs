namespace Entity
{
    public class UsuarioCredential
    {
        public string Id { get; set; }
        public string Usuario { get; set; }
        public string Contrasenia { get; set; }
        public string IdUser { get; set; }
        public bool Estado { get; set; }     
        public string Fecha_log { get; set; }

        // Constructor vacío
        public UsuarioCredential() { }
    }
}

namespace Entity
{
    public class Proced
    {
        public string? Id { get; set; }
        public string IdGuia { get; set; }
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public DateTime? Fecha_log { get; set; }

        // Opcional: solo se usa si se necesita
        public List<Valid>? valid { get; set; }
    }

}


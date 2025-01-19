namespace Entity
{
    public class Turno
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string IdComp { get; set; }
        public string HoraInicio { get; set; } 
        public string HoraFin { get; set; }     
        public string Fecha_log { get; set; }

        // Constructor vacío
        public Turno() { }
    }   
}
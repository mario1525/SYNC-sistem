namespace Entity
{
    public class Horario
    {
        public string Id { get; set; }
        public string IdTurno { get; set; }
        public string IdCuad { get; set; }
        public int DiaSemana { get; set; }
        public bool EsFestivo { get; set; }
        public bool Estado { get; set; }
        public string Fecha_log { get; set; }

        // Constructor vacío
        public Horario() { }
    }   

}
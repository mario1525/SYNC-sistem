namespace Sync.Common.Web
{
    /// <summary>Respuesta estándar de operaciones de escritura.</summary>
    public class Mensaje
    {
        public string mensaje { get; set; } = string.Empty;
        /// <summary>Id del registro creado/afectado (si aplica).</summary>
        public string? id { get; set; }

        public Mensaje() { }
        public Mensaje(string mensaje, string? id = null)
        {
            this.mensaje = mensaje;
            this.id = id;
        }
    }
}

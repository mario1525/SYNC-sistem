using Sync.Common.Data;

namespace Entity
{
    /// <summary>Reporte de falla hecho desde campo sobre un equipo.</summary>
    public class Falla : IEntidadCompania
    {
        public static readonly string[] Tipos = ["Fuga", "RuidoVibracion", "Temperatura", "Electrica", "NoArranca", "Otro"];
        public static readonly string[] Prioridades = ["Baja", "Media", "Alta", "Critica"];
        public static readonly string[] Estados = ["Abierta", "EnAtencion", "Cerrada"];

        public string? Id { get; set; }
        public string? IdComp { get; set; }
        public string IdEquipo { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Prioridad { get; set; } = "Media";
        public bool EquipoDetenido { get; set; }
        public string? Descripcion { get; set; }
        public string EstadoFalla { get; set; } = "Abierta";
        public string? IdUserReporta { get; set; }
        public DateTime? FechaReporte { get; set; }
        public DateTime? FechaCierre { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }

        // Solo lectura
        public string? EquipoNombre { get; set; }
        public string? EquipoSerie { get; set; }
        public string? ReportaNombre { get; set; }
        public int NumFotos { get; set; }
    }

    public class FallaFiltro : FiltroBase
    {
        public string? IdEquipo { get; set; }
        public string? EstadoFalla { get; set; }
    }

    /// <summary>Archivo adjunto (foto). Los bytes se sirven aparte en GET /Adjunto/{id}/archivo.</summary>
    public class Adjunto
    {
        public const string Equipo = "Equipo";
        public const string Ejecucion = "Ejecucion";
        public const string Falla = "Falla";
        public static readonly string[] Entidades = [Equipo, Ejecucion, Falla];

        public string? Id { get; set; }
        public string? IdComp { get; set; }
        /// <summary>Equipo, Ejecucion (IdEntidad = Actividad_Equipo) o Falla.</summary>
        public string Entidad { get; set; } = string.Empty;
        public string IdEntidad { get; set; } = string.Empty;
        /// <summary>En Ejecucion: Id de la verificación (Valid) que evidencia.</summary>
        public string? Ref { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public int Tamano { get; set; }
        public string? IdUser { get; set; }
        public DateTime? Fecha { get; set; }
    }

    /// <summary>Bytes de un adjunto.</summary>
    public sealed record ArchivoAdjunto(string Nombre, string ContentType, byte[] Datos);
}

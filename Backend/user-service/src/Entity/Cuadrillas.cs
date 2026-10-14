using Sync.Common.Data;

namespace Entity
{
    /// <summary>Cuadrilla de trabajo.</summary>
    public class Cuad : IEntidadCompania
    {
        public string? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? IdComp { get; set; }
        public int NumIntegrantes { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class CuadFiltro : FiltroBase
    {
        public string? Nombre { get; set; }
    }

    /// <summary>Turno de trabajo (formato de hora "HH:mm").</summary>
    public class Turno : IEntidadCompania
    {
        public string? Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? IdComp { get; set; }
        public string HoraInicio { get; set; } = "06:00";
        public string HoraFin { get; set; } = "14:00";
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class TurnoFiltro : FiltroBase
    {
        public string? Nombre { get; set; }
    }

    /// <summary>Asignación de un turno a una cuadrilla en un día de la semana (1 = lunes ... 7 = domingo).</summary>
    public class Horario : IEntidad
    {
        public string? Id { get; set; }
        public string IdTurno { get; set; } = string.Empty;
        public string? TurnoNombre { get; set; }
        public string? HoraInicio { get; set; }
        public string? HoraFin { get; set; }
        public string IdCuad { get; set; } = string.Empty;
        public string? CuadNombre { get; set; }
        public int DiaSemana { get; set; }
        public bool EsFestivo { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class HorarioFiltro : FiltroBase
    {
        public string? IdTurno { get; set; }
        public string? IdCuad { get; set; }
        public int? DiaSemana { get; set; }
    }
}

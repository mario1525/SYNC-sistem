namespace Sync.Common.Data
{
    /// <summary>Entidad con identificador y estado activo/inactivo.</summary>
    public interface IEntidad
    {
        string? Id { get; set; }
        bool Estado { get; set; }
    }

    /// <summary>Entidad que pertenece directamente a una compañía (columna IdComp).</summary>
    public interface IEntidadCompania : IEntidad
    {
        string? IdComp { get; set; }
    }

    /// <summary>Filtro base de consultas. IdComp lo sobrescribe siempre el tenant del token.</summary>
    public interface IFiltro
    {
        string? Id { get; set; }
        string? IdComp { get; set; }
        bool? Estado { get; set; }
    }

    /// <summary>Filtro mínimo para entidades sin filtros adicionales.</summary>
    public class FiltroBase : IFiltro
    {
        public string? Id { get; set; }
        public string? IdComp { get; set; }
        public bool? Estado { get; set; }
    }
}

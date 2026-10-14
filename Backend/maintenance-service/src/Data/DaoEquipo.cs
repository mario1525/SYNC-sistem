using System.Data;
using Entity;
using Microsoft.Data.SqlClient;
using Sync.Common.Data;

namespace Data
{
    /// <summary>
    /// Equipo + ubicación vigente. db_Sp_Equipo_Set guarda ambos en una transacción.
    /// </summary>
    public class DaoEquipo(SqlClient db) : CrudDao<Equipo, EquipoFiltro>(db)
    {
        protected override string Entidad => "Equipo";

        protected override IEnumerable<SqlParameter> FiltroParams(EquipoFiltro f) =>
        [
            Sql.P("@Id", Sql.NullIfEmpty(f.Id)),
            Sql.P("@Nombre", Sql.NullIfEmpty(f.Nombre)),
            Sql.P("@IdComp", Sql.NullIfEmpty(f.IdComp)),
            Sql.P("@Marca", Sql.NullIfEmpty(f.Marca)),
            Sql.P("@NSerie", Sql.NullIfEmpty(f.NSerie)),
            Sql.P("@IdPlanta", Sql.NullIfEmpty(f.IdPlanta)),
            Sql.P("@IdArea", Sql.NullIfEmpty(f.IdArea)),
            Sql.P("@IdSeccionArea", Sql.NullIfEmpty(f.IdSeccionArea)),
            Sql.P("@Estado", f.Estado)
        ];

        protected override IEnumerable<SqlParameter> SetParams(Equipo e) =>
        [
            Sql.P("@Id", e.Id),
            Sql.P("@Nombre", e.Nombre),
            Sql.P("@Descripcion", e.Descripcion),
            Sql.P("@IdComp", e.IdComp),
            Sql.P("@Modelo", e.Modelo),
            Sql.P("@NSerie", e.NSerie),
            Sql.P("@Fabricante", e.Fabricante),
            Sql.P("@Marca", e.Marca),
            Sql.P("@Funcion", e.Funcion),
            Sql.P("@Peso", e.Peso),
            Sql.P("@Cilindraje", e.Cilindraje),
            Sql.P("@Potencia", e.Potencia),
            Sql.P("@Ancho", e.Ancho),
            Sql.P("@Alto", e.Alto),
            Sql.P("@Largo", e.Largo),
            Sql.P("@Capacidad", e.Capacidad),
            Sql.P("@AnioFabricacion", e.AnioFabricacion),
            Sql.P("@Caracteristicas", e.Caracteristicas),
            Sql.P("@Estado", e.Estado),
            Sql.P("@TipoUbicacion", e.Ubicacion?.TipoUbicacion),
            Sql.P("@IdSeccionArea", Sql.NullIfEmpty(e.Ubicacion?.IdSeccionArea))
        ];

        protected override Equipo Map(DataRow r) => new()
        {
            Id = r.Str("Id"),
            Nombre = r.Str("Nombre"),
            Descripcion = r.Str("Descripcion"),
            IdComp = r.Str("IdComp"),
            Modelo = r.Str("Modelo"),
            NSerie = r.Str("NSerie"),
            Fabricante = r.Str("Fabricante"),
            Marca = r.Str("Marca"),
            Funcion = r.Str("Funcion"),
            Peso = r.Int("Peso"),
            Cilindraje = r.Int("Cilindraje"),
            Potencia = r.Int("Potencia"),
            Ancho = r.Int("Ancho"),
            Alto = r.Int("Alto"),
            Largo = r.Int("Largo"),
            Capacidad = r.Int("Capacidad"),
            AnioFabricacion = r.Int("AnioFabricacion"),
            Caracteristicas = r.Str("Caracteristicas"),
            Estado = r.Bool("Estado"),
            Fecha_log = r.Date("Fecha_log"),
            Ubicacion = r.Has("IdUbicacion")
                ? new UbicacionEquipo
                {
                    Id = r.Str("IdUbicacion"),
                    TipoUbicacion = r.Str("TipoUbicacion"),
                    IdSeccionArea = r.StrOrNull("IdSeccionArea"),
                    SeccionNombre = r.StrOrNull("SeccionNombre"),
                    IdArea = r.StrOrNull("IdArea"),
                    AreaNombre = r.StrOrNull("AreaNombre"),
                    IdPlanta = r.StrOrNull("IdPlanta"),
                    PlantaNombre = r.StrOrNull("PlantaNombre"),
                    TipoAreaNombre = r.StrOrNull("TipoAreaNombre"),
                    Vigente = true
                }
                : null
        };

        /// <summary>Historial de ubicaciones del equipo (vigente primero).</summary>
        public async Task<List<UbicacionEquipo>> GetUbicacionesAsync(string idEquipo, string? idComp, CancellationToken ct = default)
        {
            var table = await Db.QueryAsync("dbo.db_Sp_UbicacionEquipo_Get",
                [Sql.P("@IdEquipo", idEquipo), Sql.P("@IdComp", Sql.NullIfEmpty(idComp))], ct);
            return table.MapRows(r => new UbicacionEquipo
            {
                Id = r.Str("Id"),
                TipoUbicacion = r.Str("TipoUbicacion"),
                IdSeccionArea = r.StrOrNull("IdSeccionArea"),
                SeccionNombre = r.StrOrNull("SeccionNombre"),
                AreaNombre = r.StrOrNull("AreaNombre"),
                PlantaNombre = r.StrOrNull("PlantaNombre"),
                Vigente = r.Bool("Vigente"),
                Fecha_log = r.Date("Fecha_log")
            });
        }
    }
}

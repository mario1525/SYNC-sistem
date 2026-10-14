using System.Data;
using Microsoft.Data.SqlClient;

namespace Sync.Common.Data
{
    /// <summary>
    /// DAO base sobre la convención de SP: dbo.db_Sp_{Entidad}_{Get|Set|Del|Active}.
    /// Cada DAO concreto solo define cómo mapear filtros, parámetros y filas.
    /// </summary>
    public abstract class CrudDao<T, TFiltro> where TFiltro : IFiltro
    {
        protected readonly SqlClient Db;

        protected CrudDao(SqlClient db) => Db = db;

        /// <summary>Nombre de la entidad en los SP (p. ej. "Equipo").</summary>
        protected abstract string Entidad { get; }
        protected abstract IEnumerable<SqlParameter> FiltroParams(TFiltro filtro);
        protected abstract IEnumerable<SqlParameter> SetParams(T entidad);
        protected abstract T Map(DataRow row);

        public virtual async Task<List<T>> GetAsync(TFiltro filtro, CancellationToken ct = default)
        {
            var table = await Db.QueryAsync($"dbo.db_Sp_{Entidad}_Get", FiltroParams(filtro).ToArray(), ct);
            return table.MapRows(Map);
        }

        /// <summary>Inserta ('I') o actualiza ('A').</summary>
        public virtual Task SetAsync(char operacion, T entidad, CancellationToken ct = default)
        {
            var parameters = SetParams(entidad).Append(Sql.P("@Operacion", operacion.ToString())).ToArray();
            return Db.ExecuteAsync($"dbo.db_Sp_{Entidad}_Set", parameters, ct);
        }

        public virtual Task DeleteAsync(string id, CancellationToken ct = default) =>
            Db.ExecuteAsync($"dbo.db_Sp_{Entidad}_Del", [Sql.P("@Id", id)], ct);

        public virtual Task ActiveAsync(string id, bool estado, CancellationToken ct = default) =>
            Db.ExecuteAsync($"dbo.db_Sp_{Entidad}_Active", [Sql.P("@Id", id), Sql.P("@Estado", estado)], ct);
    }
}

using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoUbicacionEquipo
    {
        private readonly SqlClient _sqlClient;

        public DaoUbicacionEquipo(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla UbicacionEquipo
        public async Task<List<UbicacionEquipo>> GetUbicacionEquipo(string id, string idEquipo, string tipoUbicacion, string idPlanta, string idAreaFuncional, string idBodega, string idSeccionBodega, string idPatio, bool? estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_UbicacionEquipo_Get";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id ?? (object)DBNull.Value),
                    new SqlParameter("@IdEquipo", idEquipo ?? (object)DBNull.Value),
                    new SqlParameter("@TipoUbicacion", tipoUbicacion ?? (object)DBNull.Value),
                    new SqlParameter("@IdPlanta", idPlanta ?? (object)DBNull.Value),
                    new SqlParameter("@IdAreaFuncional", idAreaFuncional ?? (object)DBNull.Value),
                    new SqlParameter("@IdBodega", idBodega ?? (object)DBNull.Value),
                    new SqlParameter("@IdSeccionBodega", idSeccionBodega ?? (object)DBNull.Value),
                    new SqlParameter("@IdPatio", idPatio ?? (object)DBNull.Value),
                    new SqlParameter("@Estado", estado.HasValue ? (object)estado.Value : DBNull.Value)
                };

                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                return MapDataTableToList(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener UbicacionEquipo: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla UbicacionEquipo
        public async void SetUbicacionEquipo(string operacion, UbicacionEquipo ubicacionEquipo)
        {
            try
            {
                if (ubicacionEquipo == null)
                {
                    throw new ArgumentNullException(nameof(ubicacionEquipo));
                }

                const string procedureName = "dbo.db_Sp_UbicacionEquipo_Set";

                var parameters = new[]
                {
                    new SqlParameter("@Id", ubicacionEquipo.Id),
                    new SqlParameter("@IdEquipo", ubicacionEquipo.IdEquipo),
                    new SqlParameter("@TipoUbicacion", ubicacionEquipo.TipoUbicacion),
                    new SqlParameter("@IdPlanta", ubicacionEquipo.IdPlanta),
                    new SqlParameter("@IdAreaFuncional", ubicacionEquipo.IdAreaFuncional ?? (object)DBNull.Value),
                    new SqlParameter("@IdBodega", ubicacionEquipo.IdBodega ?? (object)DBNull.Value),
                    new SqlParameter("@IdSeccionBodega", ubicacionEquipo.IdSeccionBodega ?? (object)DBNull.Value),
                    new SqlParameter("@IdPatio", ubicacionEquipo.IdPatio ?? (object)DBNull.Value),
                    new SqlParameter("@Estado", ubicacionEquipo.Estado),
                    new SqlParameter("@Operacion", operacion)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear/modificar una UbicacionEquipo: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla UbicacionEquipo
        public async void DeleteUbicacionEquipo(string id)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_UbicacionEquipo_Del";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar una UbicacionEquipo: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla UbicacionEquipo
        public async void ActiveUbicacionEquipo(string id, bool estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_UbicacionEquipo_Active";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Estado", estado)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar el estado de una UbicacionEquipo: {ex.Message}");
                throw;
            }
        }

        // Método para mapear los resultados de la consulta a una lista de UbicacionEquipo
        private static List<UbicacionEquipo> MapDataTableToList(DataTable dataTable)
        {
            var ubicacionEquipoList = new List<UbicacionEquipo>();

            foreach (DataRow row in dataTable.Rows)
            {
                var ubicacionEquipo = new UbicacionEquipo
                {
                    Id = row["Id"].ToString(),
                    IdEquipo = row["IdEquipo"].ToString(),
                    TipoUbicacion = row["TipoUbicacion"].ToString(),
                    IdPlanta = row["IdPlanta"].ToString(),
                    IdAreaFuncional = row["IdAreaFuncional"]?.ToString(),
                    IdBodega = row["IdBodega"]?.ToString(),
                    IdSeccionBodega = row["IdSeccionBodega"]?.ToString(),
                    IdPatio = row["IdPatio"]?.ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };

                ubicacionEquipoList.Add(ubicacionEquipo);
            }

            return ubicacionEquipoList;
        }
    }
}
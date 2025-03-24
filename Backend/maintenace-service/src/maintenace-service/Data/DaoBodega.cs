using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoBodega
    {
        private readonly SqlClient _sqlClient;

        public DaoBodega(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Bodega
        public async Task<List<Bodega>> GetBodega(string id, string idPlanta, string nombre, bool? estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_Bodega_Get";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id ?? (object)DBNull.Value),
                    new SqlParameter("@IdPlanta", idPlanta ?? (object)DBNull.Value),
                    new SqlParameter("@Nombre", nombre ?? (object)DBNull.Value),
                    new SqlParameter("@Estado", estado.HasValue ? (object)estado.Value : DBNull.Value)
                };

                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                return MapDataTableToList(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Bodega: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Bodega
        public async void SetBodega(string operacion, Bodega bodega)
        {
            try
            {
                if (bodega == null)
                {
                    throw new ArgumentNullException(nameof(bodega));
                }

                const string procedureName = "dbo.db_Sp_Bodega_Set";

                var parameters = new[]
                {
                    new SqlParameter("@Id", bodega.Id),
                    new SqlParameter("@IdPlanta", bodega.IdPlanta),
                    new SqlParameter("@Nombre", bodega.Nombre),
                    new SqlParameter("@Estado", bodega.Estado),
                    new SqlParameter("@Operacion", operacion)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear/modificar una Bodega: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Bodega
        public async void DeleteBodega(string id)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_Bodega_Del";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar una Bodega: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Bodega
        public async void ActiveBodega(string id, bool estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_Bodega_Active";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Estado", estado)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar el estado de la Bodega: {ex.Message}");
                throw;
            }
        }

        // Método para mapear los resultados de la consulta a una lista de Bodega
        private static List<Bodega> MapDataTableToList(DataTable dataTable)
        {
            var bodegaList = new List<Bodega>();

            foreach (DataRow row in dataTable.Rows)
            {
                var bodega = new Bodega
                {
                    Id = row["Id"].ToString(),
                    IdPlanta = row["IdPlanta"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };

                bodegaList.Add(bodega);
            }

            return bodegaList;
        }
    }
}
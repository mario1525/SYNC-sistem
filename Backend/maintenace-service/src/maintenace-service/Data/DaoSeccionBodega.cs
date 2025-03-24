using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoSeccionBodega
    {
        private readonly SqlClient _sqlClient;

        public DaoSeccionBodega(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla SeccionBodega
        public async Task<List<SeccionBodega>> GetSeccionBodega(string id, string idBodega, string nombre, bool? estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_SeccionBodega_Get";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id ?? (object)DBNull.Value),
                    new SqlParameter("@IdBodega", idBodega ?? (object)DBNull.Value),
                    new SqlParameter("@Nombre", nombre ?? (object)DBNull.Value),
                    new SqlParameter("@Estado", estado.HasValue ? (object)estado.Value : DBNull.Value)
                };

                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                return MapDataTableToList(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener SeccionBodega: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla SeccionBodega
        public async void SetSeccionBodega(string operacion, SeccionBodega seccionBodega)
        {
            try
            {
                if (seccionBodega == null)
                {
                    throw new ArgumentNullException(nameof(seccionBodega));
                }

                const string procedureName = "dbo.db_Sp_SeccionBodega_Set";

                var parameters = new[]
                {
                    new SqlParameter("@Id", seccionBodega.Id),
                    new SqlParameter("@IdBodega", seccionBodega.IdBodega),
                    new SqlParameter("@Nombre", seccionBodega.Nombre),
                    new SqlParameter("@Estado", seccionBodega.Estado),
                    new SqlParameter("@Operacion", operacion)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear/modificar una SeccionBodega: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla SeccionBodega
        public async void DeleteSeccionBodega(string id)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_SeccionBodega_Del";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar una SeccionBodega: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla SeccionBodega
        public async void ActiveSeccionBodega(string id, bool estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_SeccionBodega_Active";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Estado", estado)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar el estado de una SeccionBodega: {ex.Message}");
                throw;
            }
        }

        // Método para mapear los resultados de la consulta a una lista de SeccionBodega
        private static List<SeccionBodega> MapDataTableToList(DataTable dataTable)
        {
            var seccionBodegaList = new List<SeccionBodega>();

            foreach (DataRow row in dataTable.Rows)
            {
                var seccionBodega = new SeccionBodega
                {
                    Id = row["Id"].ToString(),
                    IdBodega = row["IdBodega"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };

                seccionBodegaList.Add(seccionBodega);
            }

            return seccionBodegaList;
        }
    }
}
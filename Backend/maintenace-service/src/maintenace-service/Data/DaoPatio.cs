using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoPatio
    {
        private readonly SqlClient _sqlClient;

        public DaoPatio(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Patio
        public async Task<List<Patio>> GetPatio(string id, string idBodega, string nombre, bool? estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_Patio_Get";

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
                Console.WriteLine($"Error al obtener Patio: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Patio
        public async void SetPatio(string operacion, Patio patio)
        {
            try
            {
                if (patio == null)
                {
                    throw new ArgumentNullException(nameof(patio));
                }

                const string procedureName = "dbo.db_Sp_Patio_Set";

                var parameters = new[]
                {
                    new SqlParameter("@Id", patio.Id),
                    new SqlParameter("@IdBodega", patio.IdBodega),
                    new SqlParameter("@Nombre", patio.Nombre),
                    new SqlParameter("@Estado", patio.Estado),
                    new SqlParameter("@Operacion", operacion)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear/modificar un Patio: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Patio
        public async void DeletePatio(string id)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_Patio_Del";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar un Patio: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Patio
        public async void ActivePatio(string id, bool estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_Patio_Active";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Estado", estado)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar el estado de un Patio: {ex.Message}");
                throw;
            }
        }

        // Método para mapear los resultados de la consulta a una lista de Patio
        private static List<Patio> MapDataTableToList(DataTable dataTable)
        {
            var patioList = new List<Patio>();

            foreach (DataRow row in dataTable.Rows)
            {
                var patio = new Patio
                {
                    Id = row["Id"].ToString(),
                    IdBodega = row["IdBodega"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };

                patioList.Add(patio);
            }

            return patioList;
        }
    }
}
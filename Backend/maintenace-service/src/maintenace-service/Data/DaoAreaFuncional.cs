using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoAreaFuncional
    {
        private readonly SqlClient _sqlClient;

        public DaoAreaFuncional(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla AreaFuncional
        public async Task<List<AreaFuncional>> GetAreaFuncional(string id, string idPlanta, string nombre, bool? estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_AreaFuncional_Get";

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
                Console.WriteLine($"Error al obtener AreaFuncional: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla AreaFuncional
        public async void SetAreaFuncional(string operacion, AreaFuncional areaFuncional)
        {
            try
            {
                if (areaFuncional == null)
                {
                    throw new ArgumentNullException(nameof(areaFuncional));
                }

                const string procedureName = "dbo.db_Sp_AreaFuncional_Set";

                var parameters = new[]
                {
                    new SqlParameter("@Id", areaFuncional.Id),
                    new SqlParameter("@IdPlanta", areaFuncional.IdPlanta),
                    new SqlParameter("@Nombre", areaFuncional.Nombre),
                    new SqlParameter("@Estado", areaFuncional.Estado),
                    new SqlParameter("@Operacion", operacion)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear/modificar un AreaFuncional: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla AreaFuncional
        public async void DeleteAreaFuncional(string id)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_AreaFuncional_Del";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar un AreaFuncional: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla AreaFuncional
        public async void ActiveAreaFuncional(string id, bool estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_AreaFuncional_Active";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Estado", estado)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar el estado de un AreaFuncional: {ex.Message}");
                throw;
            }
        }

        // Método para mapear los resultados de la consulta a una lista de AreaFuncional
        private static List<AreaFuncional> MapDataTableToList(DataTable dataTable)
        {
            var areaFuncionalList = new List<AreaFuncional>();

            foreach (DataRow row in dataTable.Rows)
            {
                var areaFuncional = new AreaFuncional
                {
                    Id = row["Id"].ToString(),
                    IdPlanta = row["IdPlanta"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };

                areaFuncionalList.Add(areaFuncional);
            }

            return areaFuncionalList;
        }
    }
}
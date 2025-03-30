using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoPlanta
    {
        private readonly SqlClient _sqlClient;

        public DaoPlanta(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Planta
        public async Task<List<Planta>> GetPlanta(string id, string nombre, string region, bool? estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_Planta_Get";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id ?? (object)DBNull.Value),
                    new SqlParameter("@Nombre", nombre ?? (object)DBNull.Value),
                    new SqlParameter("@Region", region ?? (object)DBNull.Value),
                    new SqlParameter("@Estado", estado.HasValue ? (object)estado.Value : DBNull.Value)
                };

                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                return MapDataTableToList(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Planta: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Planta
        public async void SetPlanta(string operacion, Planta planta)
        {
            try
            {
                if (planta == null)
                {
                    throw new ArgumentNullException(nameof(planta));
                }

                const string procedureName = "dbo.db_Sp_Planta_Set";

                var parameters = new[]
                {
                    new SqlParameter("@Id", planta.Id),
                    new SqlParameter("@Nombre", planta.Nombre),
                    new SqlParameter("@Region", planta.Region),
                    new SqlParameter("@IdComp", planta.IdComp),
                    new SqlParameter("@Estado", planta.Estado),
                    new SqlParameter("@Operacion", operacion)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear/modificar una Planta: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Planta (marcar como inactiva)
        public async void DeletePlanta(string id)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_Planta_Del";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar una Planta: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Planta
        public async void ActivePlanta(string id, bool estado)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_Planta_Active";

                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Estado", estado)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar el estado de la Planta: {ex.Message}");
                throw;
            }
        }

        // Método para mapear los resultados de la consulta a una lista de Planta
        private static List<Planta> MapDataTableToList(DataTable dataTable)
        {
            var plantaList = new List<Planta>();

            foreach (DataRow row in dataTable.Rows)
            {
                var planta = new Planta
                {
                    Id = row["Id"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Region = row["Region"].ToString(),
                    IdComp = row["IdComp"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };

                plantaList.Add(planta);
            }

            return plantaList;
        }
    }
}
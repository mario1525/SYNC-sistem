using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoEsp
    {
        private readonly SqlClient _sqlClient;

        public DaoEsp(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Esp
        public async Task<List<Esp>> GetEsp(string id, string nombre, string idComp, int estado)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Esp_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Nombre", nombre),
                    new SqlParameter("@IdComp", idComp),
                    new SqlParameter("@Estado", estado)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Esp> listaEsp = MapDataTableToList(dataTable);
                return listaEsp;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Esp: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Esp
        public async void SetEsp(string operacion, Esp esp)
        {
            try
            {
                if (esp == null)
                {
                    throw new ArgumentNullException(nameof(esp));
                }

                string procedureName = "dbo.db_Sp_Esp_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", esp.Id),
                    new SqlParameter("@Nombre", esp.Nombre),
                    new SqlParameter("@IdComp", esp.IdComp),
                    new SqlParameter("@Estado", esp.Estado),
                    new SqlParameter("@Operacion", operacion)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un Esp: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Esp (marcar como eliminado)
        public async void DeleteEsp(string espId)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Esp_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", espId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un Esp: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Esp
        public async void ActiveEsp(string espId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Esp_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", espId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al cambiar el estado del Esp: {ex.Message}");
                throw;
            }
        }

        private static List<Esp> MapDataTableToList(DataTable dataTable)
        {
            List<Esp> espList = new List<Esp>();

            foreach (DataRow row in dataTable.Rows)
            {
                Esp esp = new Esp
                {
                    Id = row["Id"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    IdComp = row["IdComp"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };
                espList.Add(esp);
            }
            return espList;
        }
    }
}
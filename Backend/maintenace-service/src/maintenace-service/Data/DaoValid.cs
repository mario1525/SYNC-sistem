using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoValid
    {
        private readonly SqlClient _sqlClient;

        public DaoValid(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Valid
        public async Task<List<Valid>> GetValid(string id, string nombre, string idProced, int estado)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Valid_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Nombre", nombre),
                    new SqlParameter("@IdProced", idProced),
                    new SqlParameter("@Estado", estado)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Valid> listaValid = MapDataTableToList(dataTable);
                return listaValid;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Valid: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Valid
        public async void SetValid(string operacion, Valid valid)
        {
            try
            {
                if (valid == null)
                {
                    throw new ArgumentNullException(nameof(valid));
                }

                string procedureName = "dbo.db_Sp_Valid_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", valid.Id),
                    new SqlParameter("@IdProced", valid.IdProced),
                    new SqlParameter("@Nombre", valid.Nombre),
                    new SqlParameter("@Descripcion", valid.Descripcion),
                    new SqlParameter("@Estado", valid.Estado),
                    new SqlParameter("@Operacion", operacion)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un Valid: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Valid (marcar como eliminado)
        public async void DeleteValid(string validId)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Valid_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", validId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un Valid: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Valid
        public async void ActiveValid(string validId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Valid_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", validId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al cambiar el estado del Valid: {ex.Message}");
                throw;
            }
        }

        private static List<Valid> MapDataTableToList(DataTable dataTable)
        {
            List<Valid> validList = new List<Valid>();

            foreach (DataRow row in dataTable.Rows)
            {
                Valid valid = new Valid
                {
                    Id = row["Id"].ToString(),
                    IdProced = row["IdProced"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };
                validList.Add(valid);
            }
            return validList;
        }
    }
}
using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoProced
    {
        private readonly SqlClient _sqlClient;

        public DaoProced(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Proced
        public async Task<List<Proced>> GetProced(string id, string nombre, string idGuia, int estado)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Proced_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Nombre", nombre),
                    new SqlParameter("@IdGuia", idGuia),
                    new SqlParameter("@Estado", estado)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Proced> listaProced = MapDataTableToList(dataTable);
                return listaProced;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Proced: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Proced
        public async void SetProced(string operacion, Proced proced)
        {
            try
            {
                if (proced == null)
                {
                    throw new ArgumentNullException(nameof(proced));
                }

                string procedureName = "dbo.db_Sp_Proced_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", proced.Id),
                    new SqlParameter("@IdGuia", proced.IdGuia),
                    new SqlParameter("@Nombre", proced.Nombre),
                    new SqlParameter("@Descripcion", proced.Descripcion),
                    new SqlParameter("@Estado", proced.Estado),
                    new SqlParameter("@Operacion", operacion)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un Proced: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Proced (marcar como eliminado)
        public async void DeleteProced(string procedId)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Proced_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", procedId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un Proced: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Proced
        public async void ActiveProced(string procedId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Proced_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", procedId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al cambiar el estado del Proced: {ex.Message}");
                throw;
            }
        }

        private static List<Proced> MapDataTableToList(DataTable dataTable)
        {
            List<Proced> procedList = new List<Proced>();

            foreach (DataRow row in dataTable.Rows)
            {
                Proced proced = new Proced
                {
                    Id = row["Id"].ToString(),
                    IdGuia = row["IdGuia"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };
                procedList.Add(proced);
            }
            return procedList;
        }
    }
}
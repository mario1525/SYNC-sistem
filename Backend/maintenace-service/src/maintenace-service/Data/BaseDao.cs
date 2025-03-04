using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public abstract class BaseDao<T>
    {
        protected readonly SqlClient _sqlClient;

        public BaseDao(SqlClient dbContext)
        {
            _sqlClient = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        protected async Task<List<T>> GetList(string procedureName, SqlParameter[] parameters)
        {
            try
            {
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
                return MapDataTableToList(dataTable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos: {ex.Message}");
                throw;
            }
        }

        protected async Task ExecuteProcedure(string procedureName, SqlParameter[] parameters)
        {
            try
            {
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar procedimiento almacenado: {ex.Message}");
                throw;
            }
        }

        protected abstract List<T> MapDataTableToList(DataTable dataTable);
    }
}


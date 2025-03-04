using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoTipoActividad
    {
        private readonly SqlClient _sqlClient;

        public DaoTipoActividad(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla TipoAct
        public async Task<List<TipoAct>> GetTipoAct(string id, string nombre, string idComp, int estado)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_TipoActividad_Get";

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

                List<TipoAct> listaTipoAct = MapDataTableToList(dataTable);
                return listaTipoAct;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener TipoAct: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla TipoActividad
        public async void SetTipoActividad(string operacion, TipoAct tipoActividad)
        {
            try
            {
                if (tipoActividad == null)
                {
                    throw new ArgumentNullException(nameof(tipoActividad));
                }

                string procedureName = "dbo.db_Sp_TipoActividad_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", tipoActividad.Id),
                    new SqlParameter("@IdComp", tipoActividad.IdComp),
                    new SqlParameter("@Nombre", tipoActividad.Nombre),
                    new SqlParameter("@Descripcion", tipoActividad.Descripcion),
                    new SqlParameter("@Estado", tipoActividad.Estado),
                    new SqlParameter("@Operacion", operacion)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un TipoActividad: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla TipoActividad (marcar como eliminado)
        public async void DeleteTipoActividad(string tipoActividadId)
        {
            try
            {
                string procedureName = "dbo.db_Sp_TipoActividad_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", tipoActividadId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un TipoActividad: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla TipoActividad
        public async void ActiveTipoActividad(string tipoActividadId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_TipoActividad_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", tipoActividadId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al cambiar el estado del TipoActividad: {ex.Message}");
                throw;
            }
        }

        private static List<TipoAct> MapDataTableToList(DataTable dataTable)
        {
            List<TipoAct> TipoActList = new List<TipoAct>();

            foreach (DataRow row in dataTable.Rows)
            {
                TipoAct TipoAct = new TipoAct
                {
                    Id = row["Id"].ToString(),
                    IdComp = row["IdComp"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };
                TipoActList.Add(TipoAct);
            }
            return TipoActList;
        }
    }
}
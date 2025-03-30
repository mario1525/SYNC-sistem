using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoActividad
    {
        private readonly SqlClient _sqlClient;

        public DaoActividad(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Actividad
        public async Task<List<Actividad>> GetActividad(string id, string idTipoActividad, string idCuad, bool estado)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Actividad_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@IdTipoActividad", idTipoActividad),
                    new SqlParameter("@IdCuad", idCuad),
                    new SqlParameter("@Estado", estado)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Actividad> listaActividad = MapDataTableToList(dataTable);
                return listaActividad;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Actividad: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Actividad
        public async void SetActividad(string operacion, Actividad actividad)
        {
            try
            {
                if (actividad == null)
                {
                    throw new ArgumentNullException(nameof(actividad));
                }

                string procedureName = "dbo.db_Sp_Actividad_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", actividad.Id),
                    new SqlParameter("@Descripcion", actividad.Descripcion),
                    new SqlParameter("@IdTipoActividad", actividad.IdTipoActividad),
                    new SqlParameter("@Ubicacion", actividad.Ubicacion),
                    new SqlParameter("@FechaEjecucion", actividad.FechaEjecucion),
                    new SqlParameter("@IdCuad", actividad.IdCuad),
                    new SqlParameter("@Detalle", actividad.Detalle),
                    new SqlParameter("@Intervalo", actividad.Intervalo),
                    new SqlParameter("@Estado", actividad.Estado),
                    new SqlParameter("@Operacion", operacion)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar una Actividad: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Actividad (marcar como eliminado)
        public async void DeleteActividad(string actividadId)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Actividad_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", actividadId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar una Actividad: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Actividad
        public async void ActiveActividad(string actividadId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Actividad_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", actividadId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al cambiar el estado de la Actividad: {ex.Message}");
                throw;
            }
        }

        private static List<Actividad> MapDataTableToList(DataTable dataTable)
        {
            List<Actividad> actividadList = new List<Actividad>();

            foreach (DataRow row in dataTable.Rows)
            {
                Actividad actividad = new Actividad
                {
                    Id = row["Id"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    IdTipoActividad = row["IdTipoActividad"].ToString(),
                    Ubicacion = row["Ubicacion"].ToString(),
                    FechaEjecucion = Convert.ToDateTime(row["FechaEjecucion"]),
                    IdCuad = row["IdCuad"].ToString(),
                    Detalle = row["Detalle"].ToString(),
                    Intervalo = Convert.ToInt32(row["Intervalo"]),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };
                actividadList.Add(actividad);
            }
            return actividadList;
        }
    }
}
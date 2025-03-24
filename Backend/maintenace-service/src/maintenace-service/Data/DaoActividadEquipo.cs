using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoActividadEquipo
    {
        private readonly SqlClient _sqlClient;

        public DaoActividadEquipo(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Actividad_Equipo
        public async Task<List<ActividadEquipo>> GetActividadEquipo(string idActividad, int estado)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Act_Equipo_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                    new SqlParameter("@IdActividad", idActividad),
                    new SqlParameter("@Estado", estado)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<ActividadEquipo> listaActividadEquipo = MapDataTableToList(dataTable);
                return listaActividadEquipo;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Actividad_Equipo: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Actividad_Equipo
        public async void SetActividadEquipo(string operacion, ActividadEquipo actividadEquipo)
        {
            try
            {
                if (actividadEquipo == null)
                {
                    throw new ArgumentNullException(nameof(actividadEquipo));
                }

                string procedureName = "dbo.db_Sp_Act_Equipo_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", actividadEquipo.Id),
                    new SqlParameter("@IdActividad", actividadEquipo.IdActividad),
                    new SqlParameter("@IdEquipo", actividadEquipo.IdEquipo),
                    new SqlParameter("@IdGuia", actividadEquipo.IdGuia),
                    new SqlParameter("@Estado", actividadEquipo.Estado),
                    new SqlParameter("@Operacion", operacion)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un Actividad_Equipo: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Actividad_Equipo (marcar como eliminado)
        public async void DeleteActividadEquipo(string actividadEquipoId)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Act_Equipo_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", actividadEquipoId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un Actividad_Equipo: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Actividad_Equipo
        public async void ActiveActividadEquipo(string actividadEquipoId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Act_Equipo_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", actividadEquipoId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al cambiar el estado del Actividad_Equipo: {ex.Message}");
                throw;
            }
        }

        private static List<ActividadEquipo> MapDataTableToList(DataTable dataTable)
        {
            List<ActividadEquipo> actividadEquipoList = new List<ActividadEquipo>();

            foreach (DataRow row in dataTable.Rows)
            {
                ActividadEquipo actividadEquipo = new ActividadEquipo
                {
                    Id = row["Id"].ToString(),
                    IdActividad = row["IdActividad"].ToString(),
                    IdEquipo = row["IdEquipo"].ToString(),
                    IdGuia = row["IdGuia"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                   
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };
                actividadEquipoList.Add(actividadEquipo);
            }
            return actividadEquipoList;
        }
    }
}
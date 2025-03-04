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
        public async Task<List<Actividad_Equipo>> GetActividadEquipo(string idActividad, int estado)
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

                List<Actividad_Equipo> listaActividadEquipo = MapDataTableToList(dataTable);
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
        public async void SetActividadEquipo(string operacion, Actividad_Equipo actividadEquipo)
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

        private static List<Actividad_Equipo> MapDataTableToList(DataTable dataTable)
        {
            List<Actividad_Equipo> actividadEquipoList = new List<Actividad_Equipo>();

            foreach (DataRow row in dataTable.Rows)
            {
                Actividad_Equipo actividadEquipo = new Actividad_Equipo
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
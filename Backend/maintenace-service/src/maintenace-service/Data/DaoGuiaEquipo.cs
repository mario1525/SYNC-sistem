using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoGuiaEquipo
    {
        private readonly SqlClient _sqlClient;

        public DaoGuiaEquipo(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Guia_Equipo
        public async Task<List<Guia_Equipo>> GetGuiaEquipo(string id, string idGuia, string idEquipo, int estado)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Guia_Equipo_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@IdGuia", idGuia),
                    new SqlParameter("@IdEquipo", idEquipo),
                    new SqlParameter("@Estado", estado)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Guia_Equipo> listaGuiaEquipo = MapDataTableToList(dataTable);
                return listaGuiaEquipo;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Guia_Equipo: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Guia_Equipo
        public async void SetGuiaEquipo(string operacion, Guia_Equipo guiaEquipo)
        {
            try
            {
                if (guiaEquipo == null)
                {
                    throw new ArgumentNullException(nameof(guiaEquipo));
                }

                string procedureName = "dbo.db_Sp_Guia_Equipo_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", guiaEquipo.Id),
                    new SqlParameter("@IdGuia", guiaEquipo.IdGuia),
                    new SqlParameter("@IdEquipo", guiaEquipo.IdEquipo),
                    new SqlParameter("@Estado", guiaEquipo.Estado),
                    new SqlParameter("@Operacion", operacion)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un Guia_Equipo: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Guia_Equipo (marcar como eliminado)
        public async void DeleteGuiaEquipo(string guiaEquipoId)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Guia_Equipo_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", guiaEquipoId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un Guia_Equipo: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Guia_Equipo
        public async void ActiveGuiaEquipo(string guiaEquipoId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Guia_Equipo_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", guiaEquipoId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al cambiar el estado del Guia_Equipo: {ex.Message}");
                throw;
            }
        }

        private static List<Guia_Equipo> MapDataTableToList(DataTable dataTable)
        {
            List<Guia_Equipo> guiaEquipoList = new List<Guia_Equipo>();

            foreach (DataRow row in dataTable.Rows)
            {
                Guia_Equipo guiaEquipo = new Guia_Equipo
                {
                    Id = row["Id"].ToString(),
                    IdGuia = row["IdGuia"].ToString(),
                    IdEquipo = row["IdEquipo"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };
                guiaEquipoList.Add(guiaEquipo);
            }
            return guiaEquipoList;
        }
    }
}
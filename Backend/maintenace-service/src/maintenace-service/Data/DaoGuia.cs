using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoGuia
    {
        private readonly SqlClient _sqlClient;

        public DaoGuia(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Guia
        public async Task<List<Guia>> GetGuia(string id, string nombre, string idComp, string idEsp, int estado)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Guia_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Nombre", nombre),
                    new SqlParameter("@IdComp", idComp),
                    new SqlParameter("@IdEsp", idEsp),
                    new SqlParameter("@Estado", estado)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Guia> listaGuia = MapDataTableToList(dataTable);
                return listaGuia;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Guia: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Guia
        public async void SetGuia(string operacion, Guia guia)
        {
            try
            {
                if (guia == null)
                {
                    throw new ArgumentNullException(nameof(guia));
                }

                string procedureName = "dbo.db_Sp_Guia_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", guia.Id),
                    new SqlParameter("@Nombre", guia.Nombre),
                    new SqlParameter("@Descripcion", guia.Descripcion),
                    new SqlParameter("@Proceso", guia.Proceso),
                    new SqlParameter("@Inspeccion", guia.Inspeccion),
                    new SqlParameter("@Herramientas", guia.Herramientas),
                    new SqlParameter("@IdComp", guia.IdComp),
                    new SqlParameter("@IdEsp", guia.IdEsp),
                    new SqlParameter("@SeguridadInd", guia.SeguridadInd),
                    new SqlParameter("@SeguridadAmb", guia.SeguridadAmb),
                    new SqlParameter("@Intervalo", guia.Intervalo),
                    new SqlParameter("@Importante", guia.Importante),
                    new SqlParameter("@Insumos", guia.Insumos),
                    new SqlParameter("@Personal", guia.Personal),
                    new SqlParameter("@Duracion", guia.Duracion),
                    new SqlParameter("@Logistica", guia.Logistica),
                    new SqlParameter("@Situacion", guia.Situacion),
                    new SqlParameter("@Notas", guia.Notas),
                    new SqlParameter("@CreatedBy", guia.CreatedBy),
                    new SqlParameter("@UpdatedBy", guia.UpdatedBy),
                    new SqlParameter("@Estado", guia.Estado),
                    new SqlParameter("@Operacion", operacion)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar una Guia: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Guia (marcar como eliminado)
        public async void DeleteGuia(string guiaId)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Guia_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", guiaId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar una Guia: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Guia
        public async void ActiveGuia(string guiaId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Guia_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", guiaId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al cambiar el estado de la Guia: {ex.Message}");
                throw;
            }
        }

        private static List<Guia> MapDataTableToList(DataTable dataTable)
        {
            List<Guia> guiaList = new List<Guia>();

            foreach (DataRow row in dataTable.Rows)
            {
                Guia guia = new Guia
                {
                    Id = row["Id"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    Proceso = row["Proceso"].ToString(),
                    Inspeccion = row["Inspeccion"].ToString(),
                    Herramientas = row["Herramientas"].ToString(),
                    IdComp = row["IdComp"].ToString(),
                    IdEsp = row["IdEsp"].ToString(),
                    SeguridadInd = row["SeguridadInd"].ToString(),
                    SeguridadAmb = row["SeguridadAmb"].ToString(),
                    Intervalo = Convert.ToInt32(row["Intervalo"]),
                    Importante = row["Importante"].ToString(),
                    Insumos = row["Insumos"].ToString(),
                    Personal = Convert.ToInt32(row["Personal"]),
                    Duracion = Convert.ToInt32(row["Duracion"]),
                    Logistica = row["Logistica"].ToString(),
                    Situacion = row["Situacion"].ToString(),
                    Notas = row["Notas"].ToString(),
                    CreatedBy = row["CreatedBy"].ToString(),
                    UpdatedBy = row["UpdatedBy"].ToString(),
                    FechaUpdate = Convert.ToDateTime(row["FechaUpdate"]),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };
                guiaList.Add(guia);
            }
            return guiaList;
        }
    }
}
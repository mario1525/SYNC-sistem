using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Data
{
    public class DaoComp
    {
        #region Metodos 
 
        private readonly SqlClient _sqlClient;

        public DaoComp(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        public async Task<List<Comp>> GetComp(String ID)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Comp_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                    new SqlParameter("@Id", ID),
                    new SqlParameter("@Nombre", ""),
                    new SqlParameter("@NIT", ""),
                    new SqlParameter("@Sector", ""),
                    new SqlParameter("@Ciudad", ""),
                    new SqlParameter("@Direccion", ""),
                    new SqlParameter("@Estado", "")
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Comp> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Comps: {ex.Message}");
                throw;
            }
        }

        // Metodo Set 
        public async void SetComp(string operacion, Comp comp)
        {
            try
            {
                if (comp == null)
                {
                    throw new ArgumentNullException(nameof(comp));
                }

                string procedureName = "dbo.db_Sp_Comp_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", comp.Id),
                    new SqlParameter("@Nombre", comp.Nombre),
                    new SqlParameter("@NIT", comp.NIT),
                    new SqlParameter("@Direccion", comp.Direccion),
                    new SqlParameter("@Sector", comp.Sector),
                    new SqlParameter("@Ciudad", comp.Ciudad),
                    new SqlParameter("@Estado", comp.Estado),
                    new SqlParameter("@Operacion", operacion)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un Comp: {ex.Message}");
                throw;
            }
        }

        // Metodo Delete
        public async void DeleteComp(string compId)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Comp_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", compId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un Comp: {ex.Message}");
                throw;
            }
        }

        // Metodo Active        
        public async void ActiveComp(string compId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Comp_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", compId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al cambiar el estado del Comp: {ex.Message}");
                throw;
            }
        }

        private static List<Comp> MapDataTableToList(DataTable dataTable)
        {
            List<Comp> CompsList = new List<Comp>();

            foreach (DataRow row in dataTable.Rows)
            {
                Comp comp = new Comp
                {
                    Id = row["Id"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    NIT = row["NIT"].ToString(),
                    Direccion = row["Direccion"].ToString(),
                    Sector = row["Sector"].ToString(),
                    Ciudad = row["Ciudad"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]), 
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };
                CompsList.Add(comp);
            }
            return CompsList;
        }
        #endregion
    }
}
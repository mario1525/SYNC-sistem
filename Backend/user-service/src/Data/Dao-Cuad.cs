using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class Dao_Cuad
    {
        #region Metodos 

        private readonly SqlClient _sqlClient;

        public Dao_Cuad(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // metodo get 
        public async Task<List<Cuad>> GetCuadList(Cuad cuad)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Cuad_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", cuad.Id),
                new SqlParameter("@Nombre", cuad.Nombre),                
                new SqlParameter("@IdEsp", cuad.IdComp),                
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Cuad> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Cuads: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Cuad>> GetCuad(string Comp)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Cuad_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", ""),
                new SqlParameter("@Nombre", ""),
                new SqlParameter("@IdComp", Comp),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Cuad> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Cuads: {ex.Message}");
                throw;
            }
        }


        public async Task<List<Cuad>> Getcuad(string cuad)

        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Cuad_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", cuad),
                new SqlParameter("@Nombre", ""),
                new SqlParameter("@IdComp", ""),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Cuad> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Cuads: {ex.Message}");
                throw;
            }
        }

        // Metodo Set 
        public async void SetCuad(string operacion, Cuad cuad)
        {
            try
            {

                if (cuad == null)
                {
                    throw new ArgumentNullException(nameof(cuad));
                }

                string procedureName = "dbo.db_Sp_Cuad_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", cuad.Id),
                    new SqlParameter("@Nombre", cuad.Nombre),
                    new SqlParameter("@IdComp", cuad.IdComp),
                    new SqlParameter("@Estado", 1),
                    new SqlParameter("@Operacion", operacion),
               };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
                // _sqlClient.
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un Cuad: {ex.Message}");
                throw;
            }
        }

        // Metodo Delete
        public async void Deletecuad(string cuadId)
        {
            try
            {


                string procedureName = "dbo.db_Sp_Cuad_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", cuadId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un Cuad: {ex.Message}");
                throw;
            }

        }

        // Metodo Active        
        public async void Activecuad(string cuadId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Cuad_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", cuadId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al carbiar el estado del Cuad: {ex.Message}");
                throw;
            }
        }

        private static List<Cuad> MapDataTableToList(DataTable dataTable)
        {
            List<Cuad> CuadsList = new List<Cuad>();

            foreach (DataRow row in dataTable.Rows)
            {
                Cuad Cuad = new Cuad
                {
                    Id = row["Id"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    IdComp = row["IdComp"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    //Eliminado = Convert.ToBoolean(row["Eliminado"]),
                    Fecha_log = row["Fecha_log"].ToString()
                    // Asigna otras propiedades según tu DataTable
                };
                CuadsList.Add(Cuad);
            }
            return CuadsList;
        }
        #endregion
    }
}

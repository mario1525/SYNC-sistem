using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;


namespace Data
{
    public class DaoUserProyect
    {        

        #region Metodos 

        private readonly SqlClient _sqlClient;

        public DaoUserProyect(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }        

        // metodo get 
        public async Task<List<UserProyect>> Gets(string IdUsuario)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.dbSpUserProyecGet";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", ""),
                new SqlParameter("@IdUsuario", IdUsuario),
                new SqlParameter("@IdProyecto", ""),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<UserProyect> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                throw;
            }
        }      

        // Metodo Set 
        public async void Set(string operacion, UserProyect user)
        {   
            try
            {
            
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                string procedureName = "dbo.dbSpUserProyecSet";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", user.Id),
                    new SqlParameter("@IdUsuario", user.IdUsuario),
                    new SqlParameter("@IdProyecto", user.IdProyecto),
                    new SqlParameter("@Estado", 1),
                    new SqlParameter("@Operacion", operacion),
               };
               await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
                // _sqlClient.
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un usuario: {ex.Message}");
                throw;
            }
        }        

        // Metodo Delete
        public async void Delete(string Id)
        {
            try
            {

            
                string procedureName = "dbo.dbSpUserProyecDel";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", Id)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un usuario: {ex.Message}");
                throw;
            }

        }

        // Metodo Active        
        public async void Active(string Id, int estado)
        {
            try
            {            
                string procedureName = "dbo.dbSpUserProyecActive";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", Id),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al carbiar el estado del usuario: {ex.Message}");
                throw;
            }
        }

        private static List<UserProyect> MapDataTableToList(DataTable dataTable)
        {
            List<UserProyect> usuariosList = new List<UserProyect>();

            foreach (DataRow row in dataTable.Rows)
            {
                UserProyect usuario = new UserProyect
                {
                    Id = row["Id"].ToString(),
                    IdUsuario = row["Nombre"].ToString(),
                    IdProyecto = row["Apellido"].ToString(),
                    Estado = row["Estado"].ToString(),
                    //Eliminado = Convert.ToBoolean(row["Eliminado"]),
                    Fecha_log = row["Fecha_log"].ToString()
                    // Asigna otras propiedades según tu DataTable
                };
                usuariosList.Add(usuario);
            }
            return usuariosList;
        }
        #endregion
    }
}

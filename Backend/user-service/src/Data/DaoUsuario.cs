using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;


namespace Data
{
    public class DaoUsers
    {        

        #region Metodos 

        private readonly SqlClient _sqlClient;

        public DaoUsers(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }        

        // metodo get 
        public async Task<List<Users>> GetUser(Users user)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Users_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", user.Id ?? (object)DBNull.Value),
                new SqlParameter("@Nombre", user.Nombre ?? (object)DBNull.Value),
                new SqlParameter("@Apellido", user.Apellido ??(object) DBNull.Value),
                new SqlParameter("@Identificacion", user.Identificacion ?? (object)DBNull.Value),
                new SqlParameter("@Correo", user.Correo ?? (object)DBNull.Value),
                new SqlParameter("@IdComp", user.IdComp ?? (object)DBNull.Value),
                new SqlParameter("@IdCuad", user.IdCuad ?? (object)DBNull.Value),
                new SqlParameter("@IdEsp", user.IdComp ?? (object)DBNull.Value),
                new SqlParameter("@Cargo", user.Cargo ?? (object)DBNull.Value),
                new SqlParameter("@Rol", user.Rol ?? (object)DBNull.Value),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Users> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Userss: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Users>> GetUserValid(string id)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Users_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", id),
                new SqlParameter("@Nombre", ""),
                new SqlParameter("@Apellido", ""),
                new SqlParameter("@Identificacion", ""),
                new SqlParameter("@Correo", ""),
                new SqlParameter("@IdComp", ""),
                new SqlParameter("@IdCuad", ""),
                new SqlParameter("@IdEsp", ""),
                new SqlParameter("@Cargo", ""),
                new SqlParameter("@Rol", ""),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Users> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Userss: {ex.Message}");
                throw;
            }
        }

        // Metodo Set 
        public async void SetUsers(string operacion, Users user)
        {   
            try
            {
            
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                string procedureName = "dbo.db_Sp_Users_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", user.Id),
                    new SqlParameter("@Nombre", user.Nombre),
                    new SqlParameter("@Apellido", user.Apellido),
                    new SqlParameter("@Identificacion", user.Identificacion),
                    new SqlParameter("@Correo", user.Correo),
                    new SqlParameter("@IdComp", user.IdComp),
                    new SqlParameter("@IdCuad", user.IdCuad ),
                    new SqlParameter("@IdEsp", user.IdEsp),
                    new SqlParameter("@Cargo", user.Cargo),
                    new SqlParameter("@Rol", user.Rol),
                    new SqlParameter("@Estado", 1),
                    new SqlParameter("@Operacion", operacion),
               };
               await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
                // _sqlClient.
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un Users: {ex.Message}");
                throw;
            }
        }        

        // Metodo Delete
        public async void DeleteUser(string userId)
        {
            try
            {

            
                string procedureName = "dbo.db_Sp_Users_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", userId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un Users: {ex.Message}");
                throw;
            }

        }

        // Metodo Active        
        public async void ActiveUser(string userId, int estado)
        {
            try
            {            
                string procedureName = "dbo.db_Sp_Users_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", userId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al carbiar el estado del Users: {ex.Message}");
                throw;
            }
        }

        private static List<Users> MapDataTableToList(DataTable dataTable)
        {
            List<Users> UserssList = new List<Users>();

            foreach (DataRow row in dataTable.Rows)
            {
                Users Users = new Users
                {
                    Id = row["Id"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Apellido = row["Apellido"].ToString(),
                    Identificacion = Convert.ToInt64(row["Identificacion"]),
                    IdComp = row["IdComp"].ToString(),
                    IdEsp = row["IdEsp"].ToString(),
                    IdCuad = row["IdCuad"].ToString(),
                    Rol = row["Rol"].ToString(),
                    Cargo = row["Cargo"].ToString(),
                    Correo = row["Correo"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    //Eliminado = Convert.ToBoolean(row["Eliminado"]),
                    Fecha_log = row["Fecha_log"].ToString()
                    // Asigna otras propiedades según tu DataTable
                };
                UserssList.Add(Users);
            }
            return UserssList;
        }
        #endregion
    }
}

using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;


namespace Data
{
    public class DaoUsuario
    {        

        #region Metodos 

        private readonly SqlClient _sqlClient;

        public DaoUsuario(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }        

        // metodo get 
        public async Task<List<Usuario>> GetUsersList(Usuario user)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.dbSpUsuarioGet";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@Nombre", user.Nombre),
                new SqlParameter("@Apellido", user.Apellido),
                new SqlParameter("@Identificacion", user.Identificacion),
                new SqlParameter("@Correo", user.Correo),
                new SqlParameter("@IdCompania", user.IdCompania),
                new SqlParameter("@Cargo", user.Cargo),
                new SqlParameter("@Rol", user.Rol),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Usuario> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Usuario>> GetUsers()
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.dbSpUsuarioGet";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", ""),
                new SqlParameter("@Nombre", ""),
                new SqlParameter("@Apellido", ""),
                new SqlParameter("@Identificacion",""),
                new SqlParameter("@Correo", ""),
                new SqlParameter("@IdCompania", ""),
                new SqlParameter("@Cargo", ""),
                new SqlParameter("@Rol", ""),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Usuario> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Usuario>> GetUsersSuperv(string Comp)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.dbSpUsuarioGet";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", ""),
                new SqlParameter("@Nombre", ""),
                new SqlParameter("@Apellido", ""),
                new SqlParameter("@Identificacion", ""),
                new SqlParameter("@Correo", ""),
                new SqlParameter("@IdCompania", Comp),
                new SqlParameter("@Cargo", ""),
                new SqlParameter("@Rol", "Supervisor"),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Usuario> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Usuario>> GetUsersOperativo(string Comp)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.dbSpUsuarioGet";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", ""),
                new SqlParameter("@Nombre", ""),
                new SqlParameter("@Apellido", ""),
                new SqlParameter("@Identificacion", ""),
                new SqlParameter("@Correo", ""),
                new SqlParameter("@IdCompania", Comp),
                new SqlParameter("@Cargo", ""),
                new SqlParameter("@Rol", "Operativo"),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Usuario> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                throw;
            }
        }


        public async Task<List<Usuario>> GetUsersComp(string Comp)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.dbSpUsuarioGet";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", ""),
                new SqlParameter("@Nombre", ""),
                new SqlParameter("@Apellido", ""),
                new SqlParameter("@Identificacion", ""),
                new SqlParameter("@Correo", ""),
                new SqlParameter("@IdCompania", Comp),
                new SqlParameter("@Cargo", ""),
                new SqlParameter("@Rol", ""),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Usuario> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                throw;
            }
        }


        public async Task<List<Usuario>> GetUser(string user)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.dbSpUsuarioGet";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@Id", user),
                new SqlParameter("@Nombre", ""),
                new SqlParameter("@Apellido", ""),
                new SqlParameter("@Identificacion", ""),
                new SqlParameter("@Correo", ""),
                new SqlParameter("@IdCompania", ""),
                new SqlParameter("@Cargo", ""),
                new SqlParameter("@Rol", ""),
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Usuario> ListaCompania = MapDataTableToList(dataTable);
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
        public async void SetUsers(string operacion, Usuario user)
        {   
            try
            {
            
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                string procedureName = "dbo.dbSpUsuarioSet";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", user.Id),
                    new SqlParameter("@Nombre", user.Nombre),
                    new SqlParameter("@Apellido", user.Apellido),
                    new SqlParameter("@Identificacion", user.Identificacion),
                    new SqlParameter("@Correo", user.Correo),
                    new SqlParameter("@IdCompania", user.IdCompania),
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
                Console.WriteLine($"Error al crear/modificar un usuario: {ex.Message}");
                throw;
            }
        }        

        // Metodo Delete
        public async void DeleteUser(string userId)
        {
            try
            {

            
                string procedureName = "dbo.dbSpUsuarioDel";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", userId)
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
        public async void ActiveUser(string userId, int estado)
        {
            try
            {            
                string procedureName = "dbo.dbSpUsuarioActive";
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
                Console.WriteLine($"Error al carbiar el estado del usuario: {ex.Message}");
                throw;
            }
        }

        private static List<Usuario> MapDataTableToList(DataTable dataTable)
        {
            List<Usuario> usuariosList = new List<Usuario>();

            foreach (DataRow row in dataTable.Rows)
            {
                Usuario usuario = new Usuario
                {
                    Id = row["Id"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Apellido = row["Apellido"].ToString(),
                    Identificacion = Convert.ToInt64(row["Identificacion"]),
                    IdCompania = row["IdCompania"].ToString(),
                    Rol = row["Rol"].ToString(),
                    Cargo = row["Cargo"].ToString(),
                    Correo = row["Correo"].ToString(),
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

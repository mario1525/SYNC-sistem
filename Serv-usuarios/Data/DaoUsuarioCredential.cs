using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoUsuarioCredential
    {
        #region Metodos 

        private readonly SqlClient _sqlClient;

        public DaoUsuarioCredential(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        public async Task<List<UsuarioCredential>> GetUserName(string username)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.dbSpUsuarioCredencialGet";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@IdUsuarioCredencial", ""),
                new SqlParameter("@Usuario", username),                
                new SqlParameter("@Estado", 1)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<UsuarioCredential> ListaCompania = MapDataTableToList(dataTable);
                return ListaCompania;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener las credenciales: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ValidCredential(string idUser)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.dbSpValidateUsuarioCredencial";

                // Definición de parámetros
                var parameters = new[]
                {
                new SqlParameter("@IdUsuario", idUser)
                };

                // Ejecutar el procedimiento almacenado
                DataTable data = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                // Verificar si hay datos y retornar el valor booleano
                if (data.Rows.Count > 0 && data.Rows[0][0] != DBNull.Value)
                {
                    return Convert.ToBoolean(data.Rows[0][0]);
                }
                else
                {
                    // Si no hay datos, asumir false 
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al validar si el usuario tiene credenciales: {ex.Message}");
                throw;
            }
        }

        // Metodo Set 
        public async void SetUsers(string operacion, UsuarioCredential user)
        {
            try
            {

                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                string procedureName = "dbo.dbSpUsuarioCredencialSet";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", user.Id),
                    new SqlParameter("@Usuario", user.Usuario),
                    new SqlParameter("@Contrasenia", user.Contrasenia),                    
                    new SqlParameter("@IdUsuario", user.IdUsuario),                    
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


                string procedureName = "dbo.dbSpUsuarioCredencialDel";
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
                string procedureName = "dbo.dbSpUsuarioCredencialActive";
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

        private static List<UsuarioCredential> MapDataTableToList(DataTable dataTable)
        {
            List<UsuarioCredential> usuariosList = new List<UsuarioCredential>();

            foreach (DataRow row in dataTable.Rows)
            {
                UsuarioCredential usuario = new UsuarioCredential
                {
                    Id = row["Id"].ToString(),
                    Usuario = row["Id"].ToString(),
                    Contrasenia = row["Contrasenia"].ToString(),
                    IdUsuario = row["IdUsuario"].ToString(),
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

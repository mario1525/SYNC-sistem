using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Data
{
    public class DaoEquipo
    {
        private readonly SqlClient _sqlClient;

        public DaoEquipo(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        // Método para obtener los registros de la tabla Equipo
        public async Task<List<Equipo>> GetEquipo(string id, string nombre, string idComp, string marca, string nSerie, bool estado)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Equipo_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                    new SqlParameter("@Id", id),
                    new SqlParameter("@Nombre", nombre),
                    new SqlParameter("@IdComp", idComp),
                    new SqlParameter("@Marca", marca),
                    new SqlParameter("@NSerie", nSerie),
                    new SqlParameter("@Estado", estado)
                };

                // Ejecutar el procedimiento almacenado
                DataTable dataTable = await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);

                List<Equipo> listaEquipo = MapDataTableToList(dataTable);
                return listaEquipo;
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al obtener Equipo: {ex.Message}");
                throw;
            }
        }

        // Método para insertar o actualizar los registros de la tabla Equipo
        public async void SetEquipo(string operacion, Equipo equipo)
        {
            try
            {
                if (equipo == null)
                {
                    throw new ArgumentNullException(nameof(equipo));
                }

                string procedureName = "dbo.db_Sp_Equipo_Set";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", equipo.Id),
                    new SqlParameter("@Nombre", equipo.Nombre),
                    new SqlParameter("@Descripcion", equipo.Descripcion),
                    new SqlParameter("@IdComp", equipo.IdComp),
                    new SqlParameter("@Modelo", equipo.Modelo),
                    new SqlParameter("@NSerie", equipo.NSerie),
                    new SqlParameter("@Ubicacion", equipo.Ubicacion),
                    new SqlParameter("@Fabricante", equipo.Fabricante),
                    new SqlParameter("@Marca", equipo.Marca),
                    new SqlParameter("@Funcion", equipo.Funcion),
                    new SqlParameter("@Peso", equipo.Peso),
                    new SqlParameter("@Cilindraje", equipo.Cilindraje),
                    new SqlParameter("@Potencia", equipo.Potencia),
                    new SqlParameter("@Ancho", equipo.Ancho),
                    new SqlParameter("@Alto", equipo.Alto),
                    new SqlParameter("@Largo", equipo.Largo),
                    new SqlParameter("@Capacidad", equipo.Capacidad),
                    new SqlParameter("@AnioFabricacion", equipo.AnioFabricacion),
                    new SqlParameter("@Caracteristicas", equipo.Caracteristicas),
                    new SqlParameter("@Seccion", equipo.Seccion),
                    new SqlParameter("@Estado", equipo.Estado),
                    new SqlParameter("@Operacion", operacion)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al crear/modificar un Equipo: {ex.Message}");
                throw;
            }
        }

        // Método para eliminar los registros de la tabla Equipo (marcar como eliminado)
        public async void DeleteEquipo(string equipoId)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Equipo_Del";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", equipoId)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al eliminar un Equipo: {ex.Message}");
                throw;
            }
        }

        // Método para activar o desactivar los registros de la tabla Equipo
        public async void ActiveEquipo(string equipoId, int estado)
        {
            try
            {
                string procedureName = "dbo.db_Sp_Equipo_Active";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Id", equipoId),
                    new SqlParameter("@Estado", estado)
                };
                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                // Manejar errores aquí
                Console.WriteLine($"Error al cambiar el estado del Equipo: {ex.Message}");
                throw;
            }
        }

        private static List<Equipo> MapDataTableToList(DataTable dataTable)
        {
            List<Equipo> equipoList = new List<Equipo>();

            foreach (DataRow row in dataTable.Rows)
            {
                Equipo equipo = new Equipo
                {
                    Id = row["Id"].ToString(),
                    Nombre = row["Nombre"].ToString(),
                    Descripcion = row["Descripcion"].ToString(),
                    IdComp = row["IdComp"].ToString(),
                    Modelo = row["Modelo"].ToString(),
                    NSerie = row["NSerie"].ToString(),
                    Ubicacion = row["Ubicacion"].ToString(),
                    Fabricante = row["Fabricante"].ToString(),
                    Marca = row["Marca"].ToString(),
                    Funcion = row["Funcion"].ToString(),
                    Peso = Convert.ToInt32(row["Peso"]),
                    Cilindraje = Convert.ToInt32(row["Cilindraje"]),
                    Potencia = Convert.ToInt32(row["Potencia"]),
                    Ancho = Convert.ToInt32(row["Ancho"]),
                    Alto = Convert.ToInt32(row["Alto"]),
                    Largo = Convert.ToInt32(row["Largo"]),
                    Capacidad = Convert.ToInt32(row["Capacidad"]),
                    AnioFabricacion = Convert.ToInt32(row["AnioFabricacion"]),
                    Caracteristicas = row["Caracteristicas"].ToString(),
                    Seccion = row["Seccion"].ToString(),
                    Estado = Convert.ToBoolean(row["Estado"]),
                    Fecha_log = Convert.ToDateTime(row["Fecha_log"])
                };
                equipoList.Add(equipo);
            }
            return equipoList;
        }
    }
}
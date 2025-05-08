using Entity;
using Data.SQLClient;
using Microsoft.Data.SqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

namespace Data
{
    public class DaoGuia
    {
        private readonly SqlClient _sqlClient;

        public DaoGuia(SqlClient dbContext)
        {
            _sqlClient = dbContext;
        }

        //metodo para obtener la guia completa 
        public async Task<Guia> GetGuiaById(string id)
        {
            try
            {
                const string procedureName = "dbo.db_Sp_Guia_Get";
                var parameters = new[]
                {
            new SqlParameter("@Id", id),
            new SqlParameter("@Nombre", DBNull.Value),
            new SqlParameter("@IdComp", DBNull.Value),
            new SqlParameter("@IdEsp", DBNull.Value),
            new SqlParameter("@Estado", DBNull.Value)
        };

                DataSet dataset = await _sqlClient.ExecuteStoredProcedureMultiResult(procedureName, parameters);
                Guia guia = MapDataSetToGuia(dataset);
                return guia;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Guia por Id: {ex.Message}");
                throw;
            }
        }

        // Método para obtener los registros de la tabla Guia
        public async Task<List<Guia>> GetGuias(string nombre, string idComp, string idEsp, int estado)
        {
            try
            {
                // Nombre del procedimiento almacenado
                const string procedureName = "dbo.db_Sp_Guia_Get";

                // Definición de parámetros
                var parameters = new[]
                {
                    new SqlParameter("@Id", ""),
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
        public async Task SetGuia(string operacion, Guia guia, List<Proced> proced, List<Valid> valid)
        {
            try
            {
                if (guia == null)
                    throw new ArgumentNullException(nameof(guia));
                if (proced == null || valid == null)
                    throw new ArgumentNullException("proced or valid cannot be null");

                    DataTable ProcedTable = new DataTable();
                    ProcedTable.Columns.Add("Id", typeof(string));
                    ProcedTable.Columns.Add("Nombre", typeof(string));
                    ProcedTable.Columns.Add("Estado", typeof(string));
                    ProcedTable.Columns.Add("Descripcion", typeof(string));

                foreach (var p in proced)
                {
                    ProcedTable.Rows.Add(p.Id, p.Nombre, p.Estado, p.Descripcion);
                }

                    DataTable ValidTable = new DataTable();
                    ValidTable.Columns.Add("Id", typeof(string));
                    ValidTable.Columns.Add("IdProced", typeof(string));
                    ValidTable.Columns.Add("Nombre", typeof(string));
                    ValidTable.Columns.Add("Estado", typeof(string));
                    ValidTable.Columns.Add("Descripcion", typeof(string));

                foreach (var v in valid)
                {
                    ValidTable.Rows.Add(v.Id, v.IdProced, v.Nombre, v.Estado, v.Descripcion);
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
                    new SqlParameter("@Proced", SqlDbType.Structured) { Value = ProcedTable },
                    new SqlParameter("@Valid", SqlDbType.Structured) { Value = ValidTable },
                    new SqlParameter("@Operacion", operacion)
                };

                await _sqlClient.ExecuteStoredProcedure(procedureName, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear/modificar una Guia: {ex}");
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
                    IdEsp = row["IdEsp"].ToString(),                    
                    Intervalo = Convert.ToInt32(row["Intervalo"]),                    
                    Personal = Convert.ToInt32(row["Personal"]),
                    Duracion = Convert.ToInt32(row["Duracion"]),                    
                    CreatedBy = row["CreatedBy"].ToString(),                   
                    Estado = Convert.ToBoolean(row["Estado"])                    
                };
                guiaList.Add(guia);
            }
            return guiaList;
        }

        private Guia MapDataSetToGuia(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            // Tabla 0: Guia
            DataRow guiaRow = ds.Tables[0].Rows[0];

            var guia = new Guia
            {
                Id = guiaRow["Id"]?.ToString(),
                Nombre = guiaRow["Nombre"]?.ToString(),
                Descripcion = guiaRow["Descripcion"]?.ToString(),
                Proceso = guiaRow["Proceso"]?.ToString(),
                Inspeccion = guiaRow["Inspeccion"]?.ToString(),
                Herramientas = guiaRow["Herramientas"]?.ToString(),
                IdComp = guiaRow["IdComp"]?.ToString(),
                IdEsp = guiaRow["IdEsp"]?.ToString(),
                SeguridadInd = guiaRow["SeguridadInd"]?.ToString(),
                SeguridadAmb = guiaRow["SeguridadAmb"]?.ToString(),
                Intervalo = guiaRow["Intervalo"] as int?,
                Importante = guiaRow["Importante"]?.ToString(),
                Insumos = guiaRow["Insumos"]?.ToString(),
                Personal = guiaRow["Personal"] as int?,
                Duracion = guiaRow["Duracion"] as int?,
                Logistica = guiaRow["Logistica"]?.ToString(),
                Situacion = guiaRow["Situacion"]?.ToString(),
                Notas = guiaRow["Notas"]?.ToString(),
                CreatedBy = guiaRow["CreatedBy"]?.ToString(),
                UpdatedBy = guiaRow["UpdatedBy"]?.ToString(),
                FechaUpdate = guiaRow["FechaUpdate"] as DateTime?,
                Estado = Convert.ToBoolean(guiaRow["Estado"]),
                Fecha_log = guiaRow["Fecha_log"] as DateTime?
            };

            // Tabla 1: Proced
            var procedTable = ds.Tables.Count > 1 ? ds.Tables[1] : null;
            var validTable = ds.Tables.Count > 2 ? ds.Tables[2] : null;

            if (procedTable != null)
            {
                guia.proced = procedTable.AsEnumerable().Select(proRow => new Proced
                {
                    Id = proRow["Id"]?.ToString(),
                    IdGuia = proRow["IdGuia"]?.ToString(),
                    Nombre = proRow["Nombre"]?.ToString(),
                    Estado = Convert.ToBoolean(proRow["Estado"]),
                    Fecha_log = proRow["Fecha_log"] as DateTime?,
                    valid = validTable?.AsEnumerable()
                        .Where(v => v["IdProced"].ToString() == proRow["Id"].ToString())
                        .Select(vr => new Valid
                        {
                            Id = vr["Id"]?.ToString(),
                            IdProced = vr["IdProced"]?.ToString(),
                            Nombre = vr["Nombre"]?.ToString(),
                            Descripcion = vr["Descripcion"]?.ToString(), // En caso de tenerlo
                            Estado = Convert.ToBoolean(vr["Estado"]),
                            Fecha_log = vr["Fecha_log"] as DateTime?
                        }).ToList()
                }).ToList();
            }

            return guia;
        }
    }
}
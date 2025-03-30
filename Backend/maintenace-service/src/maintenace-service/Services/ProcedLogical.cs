using Data;
using Entity;
using Middlewares;

namespace Services
{
    public class ProcedLogical
    {
        private readonly DaoProced _daoProced;

        public ProcedLogical(DaoProced daoProced)
        {
            _daoProced = daoProced;
        }

        // Obtener todos los procedimientos
        public async Task<List<Proced>> GetProceds(Proced proced)
        {
            try
            {
                var proceds = await _daoProced.GetProced(proced.Id, proced.Nombre, proced.IdGuia, proced.Estado ? 1 : 0);

                if (proceds == null || !proceds.Any())
                {
                    Console.WriteLine("No se encontraron procedimientos.");
                }

                return proceds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetProceds: {ex.Message}");
                throw;
            }
        }

        // Crear procedimiento
        public async Task<Mensaje> CreateProced(Proced proced)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                proced.Id = uid.ToString();
                _daoProced.SetProced("I", proced);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateProced: {ex.Message}");
                throw;
            }
        }

        // Actualizar procedimiento
        public async Task<Mensaje> UpdateProced(Proced proced)
        {
            try
            {
                if (string.IsNullOrEmpty(proced.Id))
                {
                    throw new ArgumentException("El ID del procedimiento no puede estar vacío.");
                }

                _daoProced.SetProced("A", proced);
                return new Mensaje { mensaje = "Procedimiento actualizado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateProced: {ex.Message}");
                throw;
            }
        }

        // Eliminar procedimiento
        public async Task<Mensaje> DeleteProced(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoProced.DeleteProced(id);
                return new Mensaje { mensaje = "Procedimiento eliminado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteProced: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar procedimiento
        public async Task<Mensaje> ActiveProced(string id, int estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoProced.ActiveProced(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado del procedimiento" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveProced: {ex.Message}");
                throw;
            }
        }
    }
}
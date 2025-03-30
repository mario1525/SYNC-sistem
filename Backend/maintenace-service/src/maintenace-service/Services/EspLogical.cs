using Data;
using Entity;
using Middlewares;

namespace Services
{
    public class EspLogical
    {
        private readonly DaoEsp _daoEsp;

        public EspLogical(DaoEsp daoEsp)
        {
            _daoEsp = daoEsp;
        }

        // Obtener todas las especialidades
        public async Task<List<Esp>> GetEsps(Esp esp)
        {
            try
            {
                var especialidades = await _daoEsp.GetEsp(esp.Id, esp.Nombre, esp.IdComp, esp.Estado ? 1 : 0);

                if (especialidades == null || !especialidades.Any())
                {
                    Console.WriteLine("No se encontraron especialidades.");
                }

                return especialidades;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEsps: {ex.Message}");
                throw;
            }
        }

        // Crear especialidad
        public async Task<Mensaje> CreateEsp(Esp esp)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                esp.Id = uid.ToString();
                _daoEsp.SetEsp("I", esp);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateEsp: {ex.Message}");
                throw;
            }
        }

        // Actualizar especialidad
        public async Task<Mensaje> UpdateEsp(Esp esp)
        {
            try
            {
                if (string.IsNullOrEmpty(esp.Id))
                {
                    throw new ArgumentException("El ID de la especialidad no puede estar vacío.");
                }

                _daoEsp.SetEsp("A", esp);
                return new Mensaje { mensaje = "Especialidad actualizada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateEsp: {ex.Message}");
                throw;
            }
        }

        // Eliminar especialidad
        public async Task<Mensaje> DeleteEsp(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoEsp.DeleteEsp(id);
                return new Mensaje { mensaje = "Especialidad eliminada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteEsp: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar especialidad
        public async Task<Mensaje> ActiveEsp(string id, int estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoEsp.ActiveEsp(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado de la especialidad" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveEsp: {ex.Message}");
                throw;
            }
        }
    }
}
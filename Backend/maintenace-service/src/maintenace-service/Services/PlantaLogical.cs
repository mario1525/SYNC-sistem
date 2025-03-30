using Data;
using Entity;

namespace Services
{
    public class PlantaLogical
    {
        private readonly DaoPlanta _daoPlanta;

        public PlantaLogical(DaoPlanta daoPlanta)
        {
            _daoPlanta = daoPlanta;
        }

        // Obtener todas las plantas
        public async Task<List<Planta>> GetPlantas(Planta planta)
        {
            try
            {
                var plantas = await _daoPlanta.GetPlanta(planta.Id, planta.Nombre, planta.Region, planta.Estado);

                if (plantas == null || !plantas.Any())
                {
                    Console.WriteLine("No se encontraron plantas.");
                }

                return plantas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPlantas: {ex.Message}");
                throw;
            }
        }

        // Crear planta
        public async Task<Mensaje> CreatePlanta(Planta planta)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                planta.Id = uid.ToString();
                _daoPlanta.SetPlanta("I", planta);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreatePlanta: {ex.Message}");
                throw;
            }
        }

        // Actualizar planta
        public async Task<Mensaje> UpdatePlanta(Planta planta)
        {
            try
            {
                if (string.IsNullOrEmpty(planta.Id))
                {
                    throw new ArgumentException("El ID de la planta no puede estar vacío.");
                }

                _daoPlanta.SetPlanta("A", planta);
                return new Mensaje { mensaje = "Planta actualizada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdatePlanta: {ex.Message}");
                throw;
            }
        }

        // Eliminar planta
        public async Task<Mensaje> DeletePlanta(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoPlanta.DeletePlanta(id);
                return new Mensaje { mensaje = "Planta eliminada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeletePlanta: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar planta
        public async Task<Mensaje> ActivePlanta(string id, bool estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoPlanta.ActivePlanta(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado de la planta" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActivePlanta: {ex.Message}");
                throw;
            }
        }
    }
}
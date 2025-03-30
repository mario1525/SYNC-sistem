using Data;
using Entity;

namespace Services
{
    public class PatioLogical
    {
        private readonly DaoPatio _daoPatio;

        public PatioLogical(DaoPatio daoPatio)
        {
            _daoPatio = daoPatio;
        }

        // Obtener todos los patios
        public async Task<List<Patio>> GetPatios(Patio patio)
        {
            try
            {
                var patios = await _daoPatio.GetPatio(patio.Id, patio.IdBodega, patio.Nombre, patio.Estado);

                if (patios == null || !patios.Any())
                {
                    Console.WriteLine("No se encontraron patios.");
                }

                return patios;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPatios: {ex.Message}");
                throw;
            }
        }

        // Crear patio
        public async Task<Mensaje> CreatePatio(Patio patio)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                patio.Id = uid.ToString();
                _daoPatio.SetPatio("I", patio);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreatePatio: {ex.Message}");
                throw;
            }
        }

        // Actualizar patio
        public async Task<Mensaje> UpdatePatio(Patio patio)
        {
            try
            {
                if (string.IsNullOrEmpty(patio.Id))
                {
                    throw new ArgumentException("El ID del patio no puede estar vacío.");
                }

                _daoPatio.SetPatio("A", patio);
                return new Mensaje { mensaje = "Patio actualizado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdatePatio: {ex.Message}");
                throw;
            }
        }

        // Eliminar patio
        public async Task<Mensaje> DeletePatio(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoPatio.DeletePatio(id);
                return new Mensaje { mensaje = "Patio eliminado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeletePatio: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar patio
        public async Task<Mensaje> ActivePatio(string id, bool estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoPatio.ActivePatio(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado del patio" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActivePatio: {ex.Message}");
                throw;
            }
        }
    }
}
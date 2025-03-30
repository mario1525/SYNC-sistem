using Data;
using Entity;

namespace Services
{
    public class BodegaLogical
    {
        private readonly DaoBodega _daoBodega;

        public BodegaLogical(DaoBodega daoBodega)
        {
            _daoBodega = daoBodega;
        }

        // Obtener todas las bodegas
        public async Task<List<Bodega>> GetBodegas(Bodega bodega)
        {
            try
            {
                var bodegas = await _daoBodega.GetBodega(bodega.Id, bodega.IdPlanta, bodega.Nombre, bodega.Estado);

                if (bodegas == null || !bodegas.Any())
                {
                    Console.WriteLine("No se encontraron bodegas.");
                }

                return bodegas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetBodegas: {ex.Message}");
                throw;
            }
        }

        // Crear bodega
        public async Task<Mensaje> CreateBodega(Bodega bodega)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                bodega.Id = uid.ToString();
                _daoBodega.SetBodega("I", bodega);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateBodega: {ex.Message}");
                throw;
            }
        }

        // Actualizar bodega
        public async Task<Mensaje> UpdateBodega(Bodega bodega)
        {
            try
            {
                if (string.IsNullOrEmpty(bodega.Id))
                {
                    throw new ArgumentException("El ID de la bodega no puede estar vacío.");
                }

                _daoBodega.SetBodega("A", bodega);
                return new Mensaje { mensaje = "Bodega actualizada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateBodega: {ex.Message}");
                throw;
            }
        }

        // Eliminar bodega
        public async Task<Mensaje> DeleteBodega(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoBodega.DeleteBodega(id);
                return new Mensaje { mensaje = "Bodega eliminada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteBodega: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar bodega
        public async Task<Mensaje> ActiveBodega(string id, bool estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoBodega.ActiveBodega(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado de la bodega" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveBodega: {ex.Message}");
                throw;
            }
        }
    }
}
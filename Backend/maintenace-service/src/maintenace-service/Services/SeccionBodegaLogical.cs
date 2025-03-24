using Data;
using Entity;

namespace Services
{
    public class SeccionBodegaLogical
    {
        private readonly DaoSeccionBodega _daoSeccionBodega;

        public SeccionBodegaLogical(DaoSeccionBodega daoSeccionBodega)
        {
            _daoSeccionBodega = daoSeccionBodega;
        }

        // Obtener todas las secciones de bodega
        public async Task<List<SeccionBodega>> GetSeccionBodegas(SeccionBodega seccionBodega)
        {
            try
            {
                var secciones = await _daoSeccionBodega.GetSeccionBodega(seccionBodega.Id, seccionBodega.IdBodega, seccionBodega.Nombre, seccionBodega.Estado);

                if (secciones == null || !secciones.Any())
                {
                    Console.WriteLine("No se encontraron secciones de bodega.");
                }

                return secciones;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetSeccionBodegas: {ex.Message}");
                throw;
            }
        }

        // Crear sección de bodega
        public async Task<Mensaje> CreateSeccionBodega(SeccionBodega seccionBodega)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                seccionBodega.Id = uid.ToString();
                _daoSeccionBodega.SetSeccionBodega("I", seccionBodega);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateSeccionBodega: {ex.Message}");
                throw;
            }
        }

        // Actualizar sección de bodega
        public async Task<Mensaje> UpdateSeccionBodega(SeccionBodega seccionBodega)
        {
            try
            {
                if (string.IsNullOrEmpty(seccionBodega.Id))
                {
                    throw new ArgumentException("El ID de la sección de bodega no puede estar vacío.");
                }

                _daoSeccionBodega.SetSeccionBodega("A", seccionBodega);
                return new Mensaje { mensaje = "Sección de bodega actualizada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateSeccionBodega: {ex.Message}");
                throw;
            }
        }

        // Eliminar sección de bodega
        public async Task<Mensaje> DeleteSeccionBodega(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoSeccionBodega.DeleteSeccionBodega(id);
                return new Mensaje { mensaje = "Sección de bodega eliminada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteSeccionBodega: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar sección de bodega
        public async Task<Mensaje> ActiveSeccionBodega(string id, bool estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoSeccionBodega.ActiveSeccionBodega(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado de la sección de bodega" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveSeccionBodega: {ex.Message}");
                throw;
            }
        }
    }
}
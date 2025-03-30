using Data;
using Entity;
using Middlewares;

namespace Services
{
    public class GuiaLogical
    {
        private readonly DaoGuia _daoGuia;

        public GuiaLogical(DaoGuia daoGuia)
        {
            _daoGuia = daoGuia;
        }

        // Obtener todas las guías
        public async Task<List<Guia>> GetGuias(Guia guia)
        {
            try
            {
                var guias = await _daoGuia.GetGuia(guia.Id, guia.Nombre, guia.IdComp, guia.IdEsp, 1);

                if (guias == null || !guias.Any())
                {
                    Console.WriteLine("No se encontraron guías.");
                }

                return guias;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetGuias: {ex.Message}");
                throw;
            }
        }

        // Crear guía
        public async Task<Mensaje> CreateGuia(Guia guia)
        {
            try
            {

                _daoGuia.SetGuia("I", guia);

                return new Mensaje { mensaje = "GM-0000000000"};
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateGuia: {ex.Message}");
                throw;
            }
        }

        // Actualizar guía
        public async Task<Mensaje> UpdateGuia(Guia guia)
        {
            try
            {
                if (string.IsNullOrEmpty(guia.Id))
                {
                    throw new ArgumentException("El ID de la guía no puede estar vacío.");
                }

                _daoGuia.SetGuia("A", guia);
                return new Mensaje { mensaje = "Guía actualizada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateGuia: {ex.Message}");
                throw;
            }
        }

        // Eliminar guía
        public async Task<Mensaje> DeleteGuia(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoGuia.DeleteGuia(id);
                return new Mensaje { mensaje = "Guía eliminada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteGuia: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar guía
        public async Task<Mensaje> ActiveGuia(string id, int estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoGuia.ActiveGuia(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado de la guía" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveGuia: {ex.Message}");
                throw;
            }
        }
    }
}
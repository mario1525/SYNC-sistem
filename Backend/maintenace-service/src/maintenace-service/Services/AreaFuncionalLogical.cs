using Data;
using Entity;

namespace Services
{
    public class AreaFuncionalLogical
    {
        private readonly DaoAreaFuncional _daoAreaFuncional;

        public AreaFuncionalLogical(DaoAreaFuncional daoAreaFuncional)
        {
            _daoAreaFuncional = daoAreaFuncional;
        }

        // Obtener todas las áreas funcionales
        public async Task<List<AreaFuncional>> GetAreaFuncionales(AreaFuncional areaFuncional)
        {
            try
            {
                var areas = await _daoAreaFuncional.GetAreaFuncional(areaFuncional.Id, areaFuncional.IdPlanta, areaFuncional.Nombre, areaFuncional.Estado);

                if (areas == null || !areas.Any())
                {
                    Console.WriteLine("No se encontraron áreas funcionales.");
                }

                return areas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAreaFuncionales: {ex.Message}");
                throw;
            }
        }

        // Crear área funcional
        public async Task<Mensaje> CreateAreaFuncional(AreaFuncional areaFuncional)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                areaFuncional.Id = uid.ToString();
                _daoAreaFuncional.SetAreaFuncional("I", areaFuncional);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateAreaFuncional: {ex.Message}");
                throw;
            }
        }

        // Actualizar área funcional
        public async Task<Mensaje> UpdateAreaFuncional(AreaFuncional areaFuncional)
        {
            try
            {
                if (string.IsNullOrEmpty(areaFuncional.Id))
                {
                    throw new ArgumentException("El ID del área funcional no puede estar vacío.");
                }

                _daoAreaFuncional.SetAreaFuncional("A", areaFuncional);
                return new Mensaje { mensaje = "Área funcional actualizada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateAreaFuncional: {ex.Message}");
                throw;
            }
        }

        // Eliminar área funcional
        public async Task<Mensaje> DeleteAreaFuncional(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoAreaFuncional.DeleteAreaFuncional(id);
                return new Mensaje { mensaje = "Área funcional eliminada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteAreaFuncional: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar área funcional
        public async Task<Mensaje> ActiveAreaFuncional(string id, bool estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoAreaFuncional.ActiveAreaFuncional(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado del área funcional" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveAreaFuncional: {ex.Message}");
                throw;
            }
        }
    }
}
using Data;
using Entity;


namespace Services
{
    public class ActividadEquipoLogical
    {
        private readonly DaoActividadEquipo _daoActividadEquipo;

        public ActividadEquipoLogical(DaoActividadEquipo daoActividadEquipo)
        {
            _daoActividadEquipo = daoActividadEquipo;
        }

        // Obtener todas las ActividadEquipo
        public async Task<List<ActividadEquipo>> GetActividadEquipos()
        {
            try
            {
                var ActividadEquipo = await _daoActividadEquipo.GetActividadEquipo("", 1);

                if (ActividadEquipo == null || !ActividadEquipo.Any())
                {
                    Console.WriteLine("No se encontraron ActividadEquipo.");
                }

                return ActividadEquipo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetActividadEquipos: {ex.Message}");
                throw;
            }
        }

        // Obtener ActividadEquipoañía por ID con validaciones
        public async Task<List<ActividadEquipo>> GetActividadEquipo(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                var ActividadEquipo = await _daoActividadEquipo.GetActividadEquipo(id, 1);

                if (ActividadEquipo == null || ActividadEquipo.Count == 0)
                {
                    Console.WriteLine($"No se encontró ninguna ActividadEquipo con ID: {id}");
                }

                return ActividadEquipo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetActividadEquipo: {ex.Message}");
                throw;
            }
        }

        // Crear ActividadEquipoañía
        public async Task<Mensaje> CreateActividadEquipo(ActividadEquipo ActividadEquipo)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                ActividadEquipo.Id = uid.ToString();
                _daoActividadEquipo.SetActividadEquipo("I", ActividadEquipo);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateActividadEquipo: {ex.Message}");
                throw;
            }
        }

        // Actualizar ActividadEquipoañía
        public async Task<Mensaje> UpdateActividadEquipo(ActividadEquipo ActividadEquipo)
        {
            try
            {
                _daoActividadEquipo.SetActividadEquipo("A", ActividadEquipo);
                return new Mensaje { mensaje = "ActividadEquipo actualizado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateActividadEquipo: {ex.Message}");
                throw;
            }
        }

        // Eliminar ActividadEquipoañía
        public async Task<Mensaje> DeleteActividadEquipo(string id)
        {
            try
            {
                _daoActividadEquipo.DeleteActividadEquipo(id);
                return new Mensaje { mensaje = "ActividadEquipo eliminado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteActividadEquipo: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar ActividadEquipoañía
        public async Task<Mensaje> ActiveActividadEquipo(string id, int estado)
        {
            try
            {
                _daoActividadEquipo.ActiveActividadEquipo(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado del ActividadEquipo" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveActividadEquipo: {ex.Message}");
                throw;
            }
        }
    }
}

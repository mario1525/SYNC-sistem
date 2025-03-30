using Data;
using Entity;
using Middlewares;

namespace Services
{
    public class TipoActividadLogical
    {
        private readonly DaoTipoActividad _daoTipoActividad;

        public TipoActividadLogical(DaoTipoActividad daoTipoActividad)
        {
            _daoTipoActividad = daoTipoActividad;
        }

        // Obtener todos los tipos de actividad
        public async Task<List<TipoAct>> GetTipoActividades(TipoAct tipoActividad)
        {
            try
            {
                var tipoActividades = await _daoTipoActividad.GetTipoAct(tipoActividad.Id, tipoActividad.Nombre, tipoActividad.IdComp, tipoActividad.Estado ? 1 : 0);

                if (tipoActividades == null || !tipoActividades.Any())
                {
                    Console.WriteLine("No se encontraron tipos de actividad.");
                }

                return tipoActividades;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetTipoActividades: {ex.Message}");
                throw;
            }
        }

        // Crear tipo de actividad
        public async Task<Mensaje> CreateTipoActividad(TipoAct tipoActividad)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                tipoActividad.Id = uid.ToString();
                _daoTipoActividad.SetTipoActividad("I", tipoActividad);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateTipoActividad: {ex.Message}");
                throw;
            }
        }

        // Actualizar tipo de actividad
        public async Task<Mensaje> UpdateTipoActividad(TipoAct tipoActividad)
        {
            try
            {
                if (string.IsNullOrEmpty(tipoActividad.Id))
                {
                    throw new ArgumentException("El ID del tipo de actividad no puede estar vacío.");
                }

                _daoTipoActividad.SetTipoActividad("A", tipoActividad);
                return new Mensaje { mensaje = "Tipo de actividad actualizado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateTipoActividad: {ex.Message}");
                throw;
            }
        }

        // Eliminar tipo de actividad
        public async Task<Mensaje> DeleteTipoActividad(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoTipoActividad.DeleteTipoActividad(id);
                return new Mensaje { mensaje = "Tipo de actividad eliminado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteTipoActividad: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar tipo de actividad
        public async Task<Mensaje> ActiveTipoActividad(string id, int estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoTipoActividad.ActiveTipoActividad(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado del tipo de actividad" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveTipoActividad: {ex.Message}");
                throw;
            }
        }
    }
}
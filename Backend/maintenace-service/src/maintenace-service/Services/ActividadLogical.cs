

using Data;
using Entity;

namespace Services
{
    public class ActividadLogical
    {

        private readonly DaoActividad _daoActividad;

        public ActividadLogical(DaoActividad daoActividad)
        {
            _daoActividad = daoActividad;
        }

        // Obtener todas las Actividadañías
        public async Task<List<Actividad>> GetActividads( Actividad actividad)
        {
            try
            {
                var Actividad = await _daoActividad.GetActividad(actividad.Id, actividad.IdComp, actividad.IdTipoActividad, actividad.IdCuad, actividad.Estado);

                if (Actividad == null || !Actividad.Any())
                {
                    Console.WriteLine("No se encontraron Actividad.");
                }

                return Actividad;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetActividads: {ex.Message}");
                throw;
            }
        }
       
        // Crear Actividadañía
        public async Task<Mensaje> CreateActividad(Actividad Actividad)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                Actividad.Id = uid.ToString();
                _daoActividad.SetActividad("I", Actividad);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateActividad: {ex.Message}");
                throw;
            }
        }

        // Actualizar Actividadañía
        public async Task<Mensaje> UpdateActividad(Actividad Actividad)
        {
            try
            {
                _daoActividad.SetActividad("A", Actividad);
                return new Mensaje { mensaje = "Actividad actualizado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateActividad: {ex.Message}");
                throw;
            }
        }

        // Eliminar Actividadañía
        public async Task<Mensaje> DeleteActividad(string id)
        {
            try
            {
                _daoActividad.DeleteActividad(id);
                return new Mensaje { mensaje = "Actividad eliminado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteActividad: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar Actividadañía
        public async Task<Mensaje> ActiveActividad(string id, int estado)
        {
            try
            {
                _daoActividad.ActiveActividad(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado del Actividad" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveActividad: {ex.Message}");
                throw;
            }
        }
    }
}

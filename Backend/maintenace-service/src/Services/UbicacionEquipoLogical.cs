using Data;
using Entity;

namespace Services
{
    public class UbicacionEquipoLogical
    {
        private readonly DaoUbicacionEquipo _daoUbicacionEquipo;

        public UbicacionEquipoLogical(DaoUbicacionEquipo daoUbicacionEquipo)
        {
            _daoUbicacionEquipo = daoUbicacionEquipo;
        }

        // Obtener todas las ubicaciones de equipos
        public async Task<List<UbicacionEquipo>> GetUbicacionEquipos(UbicacionEquipo ubicacionEquipo)
        {
            try
            {
                var ubicaciones = await _daoUbicacionEquipo.GetUbicacionEquipo(
                    ubicacionEquipo.Id,
                    ubicacionEquipo.IdEquipo,
                    ubicacionEquipo.TipoUbicacion,
                    ubicacionEquipo.IdPlanta,
                    ubicacionEquipo.IdAreaFuncional,
                    ubicacionEquipo.IdBodega,
                    ubicacionEquipo.IdSeccionBodega,
                    ubicacionEquipo.IdPatio,
                    ubicacionEquipo.Estado
                );

                if (ubicaciones == null || !ubicaciones.Any())
                {
                    Console.WriteLine("No se encontraron ubicaciones de equipos.");
                }

                return ubicaciones;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetUbicacionEquipos: {ex.Message}");
                throw;
            }
        }

        // Crear ubicación de equipo
        public async Task<Mensaje> CreateUbicacionEquipo(UbicacionEquipo ubicacionEquipo)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                ubicacionEquipo.Id = uid.ToString();
                _daoUbicacionEquipo.SetUbicacionEquipo("I", ubicacionEquipo);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateUbicacionEquipo: {ex.Message}");
                throw;
            }
        }

        // Actualizar ubicación de equipo
        public async Task<Mensaje> UpdateUbicacionEquipo(UbicacionEquipo ubicacionEquipo)
        {
            try
            {
                if (string.IsNullOrEmpty(ubicacionEquipo.Id))
                {
                    throw new ArgumentException("El ID de la ubicación del equipo no puede estar vacío.");
                }

                _daoUbicacionEquipo.SetUbicacionEquipo("A", ubicacionEquipo);
                return new Mensaje { mensaje = "Ubicación de equipo actualizada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateUbicacionEquipo: {ex.Message}");
                throw;
            }
        }

        // Eliminar ubicación de equipo
        public async Task<Mensaje> DeleteUbicacionEquipo(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoUbicacionEquipo.DeleteUbicacionEquipo(id);
                return new Mensaje { mensaje = "Ubicación de equipo eliminada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteUbicacionEquipo: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar ubicación de equipo
        public async Task<Mensaje> ActiveUbicacionEquipo(string id, bool estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoUbicacionEquipo.ActiveUbicacionEquipo(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado de la ubicación del equipo" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveUbicacionEquipo: {ex.Message}");
                throw;
            }
        }
    }
}
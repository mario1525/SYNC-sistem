using Data;
using Entity;
using Middlewares;

namespace Services
{
    public class GuiaEquipoLogical
    {
        private readonly DaoGuiaEquipo _daoGuiaEquipo;

        public GuiaEquipoLogical(DaoGuiaEquipo daoGuiaEquipo)
        {
            _daoGuiaEquipo = daoGuiaEquipo;
        }

        // Obtener todas las relaciones Guia_Equipo
        public async Task<List<Guia_Equipo>> GetGuiaEquipos(Guia_Equipo guiaEquipo)
        {
            try
            {
                var guiaEquipos = await _daoGuiaEquipo.GetGuiaEquipo(guiaEquipo.Id, guiaEquipo.IdGuia, guiaEquipo.IdEquipo, guiaEquipo.Estado ? 1 : 0);

                if (guiaEquipos == null || !guiaEquipos.Any())
                {
                    Console.WriteLine("No se encontraron relaciones Guia_Equipo.");
                }

                return guiaEquipos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetGuiaEquipos: {ex.Message}");
                throw;
            }
        }

        // Crear relación Guia_Equipo
        public async Task<Mensaje> CreateGuiaEquipo(Guia_Equipo guiaEquipo)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                guiaEquipo.Id = uid.ToString();
                _daoGuiaEquipo.SetGuiaEquipo("I", guiaEquipo);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateGuiaEquipo: {ex.Message}");
                throw;
            }
        }

        // Actualizar relación Guia_Equipo
        public async Task<Mensaje> UpdateGuiaEquipo(Guia_Equipo guiaEquipo)
        {
            try
            {
                if (string.IsNullOrEmpty(guiaEquipo.Id))
                {
                    throw new ArgumentException("El ID de la relación Guia_Equipo no puede estar vacío.");
                }

                _daoGuiaEquipo.SetGuiaEquipo("A", guiaEquipo);
                return new Mensaje { mensaje = "Relación Guia_Equipo actualizada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateGuiaEquipo: {ex.Message}");
                throw;
            }
        }

        // Eliminar relación Guia_Equipo
        public async Task<Mensaje> DeleteGuiaEquipo(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoGuiaEquipo.DeleteGuiaEquipo(id);
                return new Mensaje { mensaje = "Relación Guia_Equipo eliminada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteGuiaEquipo: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar relación Guia_Equipo
        public async Task<Mensaje> ActiveGuiaEquipo(string id, int estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoGuiaEquipo.ActiveGuiaEquipo(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado de la relación Guia_Equipo" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveGuiaEquipo: {ex.Message}");
                throw;
            }
        }
    }
}
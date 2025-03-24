using Data;
using Entity;
using Middlewares;

namespace Services
{
    public class EquipoLogical
    {
        private readonly DaoEquipo _daoEquipo;

        public EquipoLogical(DaoEquipo daoEquipo)
        {
            _daoEquipo = daoEquipo;
        }

        // Obtener todos los equipos
        public async Task<List<Equipo>> GetEquipos(string Id, string Nombre, string IdComp, string Marca, string NSerie, bool Estado)
        {
            try
            {
                var equipos = await _daoEquipo.GetEquipo(Id, Nombre, IdComp, Marca,NSerie, Estado);

                if (equipos == null || !equipos.Any())
                {
                    Console.WriteLine("No se encontraron equipos.");
                }

                return equipos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEquipos: {ex.Message}");
                throw;
            }
        }

        // Crear equipo
        public async Task<Mensaje> CreateEquipo(Equipo equipo)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                equipo.Id = uid.ToString();
                _daoEquipo.SetEquipo("I", equipo);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateEquipo: {ex.Message}");
                throw;
            }
        }

        // Actualizar equipo
        public async Task<Mensaje> UpdateEquipo(Equipo equipo)
        {
            try
            {
                if (string.IsNullOrEmpty(equipo.Id))
                {
                    throw new ArgumentException("El ID del equipo no puede estar vacío.");
                }

                _daoEquipo.SetEquipo("A", equipo);
                return new Mensaje { mensaje = "Equipo actualizado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateEquipo: {ex.Message}");
                throw;
            }
        }

        // Eliminar equipo
        public async Task<Mensaje> DeleteEquipo(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoEquipo.DeleteEquipo(id);
                return new Mensaje { mensaje = "Equipo eliminado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteEquipo: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar equipo
        public async Task<Mensaje> ActiveEquipo(string id, int estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoEquipo.ActiveEquipo(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado del equipo" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveEquipo: {ex.Message}");
                throw;
            }
        }
    }
}
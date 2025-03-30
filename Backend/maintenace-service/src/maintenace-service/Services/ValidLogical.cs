using Data;
using Entity;
using Middlewares;

namespace Services
{
    public class ValidLogical
    {
        private readonly DaoValid _daoValid;

        public ValidLogical(DaoValid daoValid)
        {
            _daoValid = daoValid;
        }

        // Obtener todas las validaciones
        public async Task<List<Valid>> GetValids(Valid valid)
        {
            try
            {
                var valids = await _daoValid.GetValid(valid.Id, valid.Nombre, valid.IdProced, valid.Estado ? 1 : 0);

                if (valids == null || !valids.Any())
                {
                    Console.WriteLine("No se encontraron validaciones.");
                }

                return valids;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetValids: {ex.Message}");
                throw;
            }
        }

        // Crear validación
        public async Task<Mensaje> CreateValid(Valid valid)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                valid.Id = uid.ToString();
                _daoValid.SetValid("I", valid);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateValid: {ex.Message}");
                throw;
            }
        }

        // Actualizar validación
        public async Task<Mensaje> UpdateValid(Valid valid)
        {
            try
            {
                if (string.IsNullOrEmpty(valid.Id))
                {
                    throw new ArgumentException("El ID de la validación no puede estar vacío.");
                }

                _daoValid.SetValid("A", valid);
                return new Mensaje { mensaje = "Validación actualizada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateValid: {ex.Message}");
                throw;
            }
        }

        // Eliminar validación
        public async Task<Mensaje> DeleteValid(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoValid.DeleteValid(id);
                return new Mensaje { mensaje = "Validación eliminada" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteValid: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar validación
        public async Task<Mensaje> ActiveValid(string id, int estado)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                _daoValid.ActiveValid(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado de la validación" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveValid: {ex.Message}");
                throw;
            }
        }
    }
}
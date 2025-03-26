using Data;
using Entity;
using Middlewares;

namespace Services
{
    public class CompLogical
    {
        private readonly DaoComp _daoComp;

        public CompLogical(DaoComp daoComp)
        {
            _daoComp = daoComp;
        }

        // Obtener todas las compañías
        public async Task<List<Comp>> GetComps()
        {
            try
            {
                var comp = await _daoComp.GetComp("");

                if (comp == null || !comp.Any())
                {
                    Console.WriteLine("No se encontraron compañías Log.");
                }

                return comp;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetComps: {ex.Message}");
                throw;
            }
        }

        // Obtener compañía por ID con validaciones
        public async Task<List<Comp>> GetComp(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("El ID no puede estar vacío.");
                }

                var comp = await _daoComp.GetComp(id);

                if (comp == null || comp.Count == 0)
                {
                    Console.WriteLine($"No se encontró ninguna compañía con ID: {id}");
                }

                return comp;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetComp: {ex.Message}");
                throw;
            }
        }

        // Crear compañía
        public async Task<Mensaje> CreateComp(Comp comp)
        {
            try
            {
                Guid uid = Guid.NewGuid();
                comp.Id = uid.ToString();
                _daoComp.SetComp("I", comp);

                return new Mensaje { mensaje = uid.ToString() };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateComp: {ex.Message}");
                throw;
            }
        }

        // Actualizar compañía
        public async Task<Mensaje> UpdateComp(Comp comp)
        {
            try
            {
                _daoComp.SetComp("A", comp);
                return new Mensaje { mensaje = "Comp actualizado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en UpdateComp: {ex.Message}");
                throw;
            }
        }

        // Eliminar compañía
        public async Task<Mensaje> DeleteComp(string id)
        {
            try
            {
                _daoComp.DeleteComp(id);
                return new Mensaje { mensaje = "Comp eliminado" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteComp: {ex.Message}");
                throw;
            }
        }

        // Activar/desactivar compañía
        public async Task<Mensaje> ActiveComp(string id, int estado)
        {
            try
            {
                _daoComp.ActiveComp(id, estado);
                return new Mensaje { mensaje = "Se cambió el estado del Comp" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActiveComp: {ex.Message}");
                throw;
            }
        }
    }
}

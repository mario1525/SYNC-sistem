using Data;
using Entity;
using Microsoft.Identity.Client;
using Middlewares;
using System;

namespace Services
{
    public class GuiaLogical
    {
        private readonly DaoGuia _daoGuia;
        //private readonly DaoProced _daoProced;
        //private readonly DaoValid _daoValid;

        public GuiaLogical(DaoGuia daoGuia)
        {
            _daoGuia = daoGuia;


        }

        // Obtener todas las guías
        public async Task<List<Guia>> GetGuias(Guia guia)
        {
            try
            {     
                var guias = await _daoGuia.GetGuias(guia.Nombre, guia.IdComp, guia.IdEsp, 1);

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

        public async Task<Guia> GetGuiaById(string id)
        {
            try
            {
                var guias = await _daoGuia.GetGuiaById(id);
                return guias;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetGuias: {ex.Message}");
                throw;
            }
        }

        // Crear guía
        public async Task<Mensaje> CreateGuia(Guia guiaC)
        {
            try
            {
                
                // obtengo el objeto guia
                var guia = new Guia
                {
                    Id = guiaC.Id,
                    Nombre = guiaC.Nombre,
                    Descripcion = guiaC.Descripcion,
                    Proceso = guiaC.Proceso,
                    Inspeccion = guiaC.Inspeccion,
                    Herramientas = guiaC.Herramientas,
                    IdComp = guiaC.IdComp,
                    IdEsp = guiaC.IdEsp,
                    SeguridadInd = guiaC.SeguridadInd,
                    SeguridadAmb = guiaC.SeguridadAmb,
                    Intervalo = guiaC.Intervalo,
                    Importante = guiaC.Importante,
                    Insumos = guiaC.Insumos,
                    Personal = guiaC.Personal,
                    Duracion = guiaC.Duracion,
                    Logistica = guiaC.Logistica,
                    Situacion = guiaC.Situacion,
                    Notas = guiaC.Notas,
                    CreatedBy = guiaC.CreatedBy,
                    UpdatedBy = guiaC.UpdatedBy,
                    FechaUpdate = guiaC.FechaUpdate,
                    Estado = guiaC.Estado,
                    Fecha_log = guiaC.Fecha_log
                };

                //se extre los proced
                var proced = guiaC.proced ?? new List<Proced>();

                //se recorre y se asignan los IDs 
                foreach (var proce in proced)
                {
                    Guid uid = Guid.NewGuid();
                    proce.Id = uid.ToString();

                    if (proce.valid != null)
                    {
                        foreach (var Valid in proce.valid)
                        {
                            Guid uid2 = Guid.NewGuid();
                            Valid.Id = uid2.ToString(); 
                            Valid.IdProced = uid.ToString();
                        }
                    }
                }


                // Extrae la lista de Valid
                var valid = proced
                .Where(p => p.valid != null)
                .SelectMany(p => p.valid)
                .ToList();


                //equipo.Ubicacion.Id = uid2.ToString();

                _daoGuia.SetGuia("I", guia, proced, valid );

                return new Mensaje { mensaje = "GM-0000000000"};
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateGuia: {ex.Message}");
                throw;
            }
        }

        // Actualizar guía
        public async Task<Mensaje> UpdateGuia(Guia guiaC)
        {
            try
            {
                if (string.IsNullOrEmpty(guiaC.Id))
                {
                    throw new ArgumentException("El ID de la guía no puede estar vacío.");
                }

                // obtengo el objeto guia
                var guia = new Guia
                {
                    Id = guiaC.Id,
                    Nombre = guiaC.Nombre,
                    Descripcion = guiaC.Descripcion,
                    Proceso = guiaC.Proceso,
                    Inspeccion = guiaC.Inspeccion,
                    Herramientas = guiaC.Herramientas,
                    IdComp = guiaC.IdComp,
                    IdEsp = guiaC.IdEsp,
                    SeguridadInd = guiaC.SeguridadInd,
                    SeguridadAmb = guiaC.SeguridadAmb,
                    Intervalo = guiaC.Intervalo,
                    Importante = guiaC.Importante,
                    Insumos = guiaC.Insumos,
                    Personal = guiaC.Personal,
                    Duracion = guiaC.Duracion,
                    Logistica = guiaC.Logistica,
                    Situacion = guiaC.Situacion,
                    Notas = guiaC.Notas,
                    CreatedBy = guiaC.CreatedBy,
                    UpdatedBy = guiaC.UpdatedBy,
                    FechaUpdate = guiaC.FechaUpdate,
                    Estado = guiaC.Estado,
                    Fecha_log = guiaC.Fecha_log
                };

                //se extre los proced
                var proced = guiaC.proced ?? new List<Proced>();

                // Extrae la lista de Valid
                var valid = proced
                .Where(p => p.valid != null)
                .SelectMany(p => p.valid)
                .ToList();

                _daoGuia.SetGuia("A", guia, proced, valid);
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
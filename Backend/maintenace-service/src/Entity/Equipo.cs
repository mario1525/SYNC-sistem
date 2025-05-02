namespace Entity
{
    public class Equipo
    {
        public string? Id { get; set; }              // Id interno del registro
        public string? Nombre { get; set; }          // Nombre del equipo
        public string? Descripcion { get; set; }     // Descripción del equipo
        public string IdComp { get; set; }          // FK de la tabla Compania
        public string? Modelo { get; set; }          // Modelo del equipo
        public string? NSerie { get; set; }          // Número de serie del equipo
        public string? Fabricante { get; set; }      // Fabricante del equipo
        public string? Marca { get; set; }           // Marca del equipo
        public string? Funcion { get; set; }         // Función del equipo
        public int? Peso { get; set; }               // Peso del equipo
        public int? Cilindraje { get; set; }         // Cilindraje del equipo
        public int? Potencia { get; set; }           // Potencia del equipo
        public int? Ancho { get; set; }              // Ancho del equipo
        public int? Alto { get; set; }               // Alto del equipo
        public int? Largo { get; set; }              // Largo del equipo
        public int? Capacidad { get; set; }          // Capacidad del equipo
        public int? AnioFabricacion { get; set; }    // Año de fabricación del equipo
        public string? Caracteristicas { get; set; } // Características del equipo
        public string? Seccion { get; set; }         // Sección del equipo
        public bool Estado { get; set; }            // Estado
        public DateTime? Fecha_log { get; set; }     // Log fecha
    }


    public class EquipoC
    {
        public string? Id { get; set; }              // Id interno del registro
        public string? Nombre { get; set; }          // Nombre del equipo
        public string? Descripcion { get; set; }     // Descripción del equipo
        public string IdComp { get; set; }          // FK de la tabla Compania
        public string? Modelo { get; set; }          // Modelo del equipo
        public string? NSerie { get; set; }          // Número de serie del equipo
        public UbicacionEquipo Ubicacion { get; set; }       // Ubicación del equipo
        public string Fabricante { get; set; }      // Fabricante del equipo
        public string Marca { get; set; }           // Marca del equipo
        public string Funcion { get; set; }         // Función del equipo
        public int Peso { get; set; }               // Peso del equipo
        public int Cilindraje { get; set; }         // Cilindraje del equipo
        public int Potencia { get; set; }           // Potencia del equipo
        public int Ancho { get; set; }              // Ancho del equipo
        public int Alto { get; set; }               // Alto del equipo
        public int Largo { get; set; }              // Largo del equipo
        public int Capacidad { get; set; }          // Capacidad del equipo
        public int AnioFabricacion { get; set; }    // Año de fabricación del equipo
        public string Caracteristicas { get; set; } // Características del equipo
        public string Seccion { get; set; }         // Sección del equipo
        public bool Estado { get; set; }            // Estado
        public DateTime Fecha_log { get; set; }     // Log fecha
    }
}
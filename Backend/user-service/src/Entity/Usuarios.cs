using Sync.Common.Data;

namespace Entity
{
    /// <summary>Usuario del sistema (persona). Las credenciales viven en UsuarioCredential.</summary>
    public class Users : IEntidadCompania
    {
        public string? Id { get; set; }
        public long Identificacion { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Apellido { get; set; }
        public string? Correo { get; set; }
        public string? IdComp { get; set; }
        public string? CompNombre { get; set; }
        public string? IdEsp { get; set; }
        public string? EspNombre { get; set; }
        public string? IdCuad { get; set; }
        public string? CuadNombre { get; set; }
        public string? Cargo { get; set; }
        public string Rol { get; set; } = "Tecnico";
        public string? Usuario { get; set; }
        public bool TieneCredencial { get; set; }
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class UsersFiltro : FiltroBase
    {
        public string? Nombre { get; set; }
        public long? Identificacion { get; set; }
        public string? Correo { get; set; }
        public string? IdCuad { get; set; }
        public string? IdEsp { get; set; }
        public string? Rol { get; set; }
    }

    /// <summary>Credenciales de acceso. La contraseña nunca se devuelve.</summary>
    public class UsuarioCredential : IEntidad
    {
        public string? Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        /// <summary>Solo escritura. En actualización, vacío = no cambiar.</summary>
        public string? Contrasenia { get; set; }
        public string IdUser { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;
        public DateTime? Fecha_log { get; set; }
    }

    public class CredentialFiltro : FiltroBase
    {
        public string? IdUser { get; set; }
    }

    public class Login
    {
        public string Usuario { get; set; } = string.Empty;
        public string Contrasenia { get; set; } = string.Empty;
    }

    public class Token
    {
        public string token { get; set; } = string.Empty;
        public DateTime expira { get; set; }
    }

    public class CambioContrasenia
    {
        public string Actual { get; set; } = string.Empty;
        public string Nueva { get; set; } = string.Empty;
    }

    /// <summary>Datos mínimos para emitir el token (resultado de db_Sp_User_Cred_Login).</summary>
    public class CredencialLogin
    {
        public string IdCred { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public string IdUser { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string IdComp { get; set; } = string.Empty;
    }
}

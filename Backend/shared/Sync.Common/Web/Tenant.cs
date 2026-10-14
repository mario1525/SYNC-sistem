using System.Security.Claims;

namespace Sync.Common.Web
{
    /// <summary>Roles del sistema. Root administra todas las compañías.</summary>
    public static class Roles
    {
        public const string Root = "Root";
        public const string Admin = "Admin";
        public const string Supervisor = "Supervisor";
        public const string Tecnico = "Tecnico";

        public static readonly string[] Todos = [Root, Admin, Supervisor, Tecnico];

        /// <summary>Gestión de maestros (crear/editar/eliminar).</summary>
        public static readonly string[] Gestion = [Root, Admin];
        /// <summary>Planeación y supervisión de la operación.</summary>
        public static readonly string[] Operacion = [Root, Admin, Supervisor];
        /// <summary>Solo Root.</summary>
        public static readonly string[] SoloRoot = [Root];

        public static bool EsValido(string? rol) => rol != null && Todos.Contains(rol);
    }

    /// <summary>
    /// Contexto de compañía del usuario autenticado (ADR-05).
    /// La compañía siempre sale del JWT; solo Root puede operar sobre otra.
    /// </summary>
    public sealed record Tenant(string? IdComp, bool IsRoot, string? UserId, string? Rol)
    {
        /// <summary>
        /// Compañía para filtrar consultas. Root: la solicitada (null = todas).
        /// Resto: siempre la del token, ignorando lo que venga en la solicitud.
        /// </summary>
        public string? Filtro(string? solicitada) => IsRoot ? Limpio(solicitada) : IdComp;

        /// <summary>Compañía obligatoria para crear registros.</summary>
        public string ParaEscritura(string? solicitada)
        {
            var idComp = IsRoot ? Limpio(solicitada) ?? IdComp : IdComp;
            return string.IsNullOrWhiteSpace(idComp)
                ? throw new BusinessException("No se pudo determinar la compañía (IdComp).")
                : idComp;
        }

        public bool TieneRol(params string[] roles) => Rol != null && roles.Contains(Rol);

        private static string? Limpio(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public static class TenantExtensions
    {
        public const string IdCompClaim = "IdComp";

        public static Tenant Tenant(this ClaimsPrincipal user) => new(
            user.FindFirstValue(IdCompClaim),
            user.IsInRole(Roles.Root),
            user.FindFirstValue(ClaimTypes.NameIdentifier),
            user.FindFirstValue(ClaimTypes.Role));
    }
}

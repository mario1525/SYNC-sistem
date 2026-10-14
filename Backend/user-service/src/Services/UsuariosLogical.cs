using System.Security.Claims;
using Data;
using Entity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Sync.Common.Services;
using Sync.Common.Web;

namespace Services
{
    /// <summary>Emite los JWT. Los nombres de claim se mantienen por compatibilidad con el frontend.</summary>
    public class TokenService(JwtOptions options)
    {
        private readonly JsonWebTokenHandler _handler = new();

        public Token Generar(CredencialLogin usuario)
        {
            var expira = DateTime.UtcNow.AddHours(options.ExpirationHours);
            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = options.Issuer,
                Audience = options.Audience,
                Expires = expira,
                SigningCredentials = new SigningCredentials(options.ValidationParameters().IssuerSigningKey, SecurityAlgorithms.HmacSha256),
                Claims = new Dictionary<string, object>
                {
                    [JwtRegisteredClaimNames.Sub] = usuario.Nombre,
                    [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
                    [ClaimTypes.Role] = usuario.Rol,
                    [ClaimTypes.NameIdentifier] = usuario.IdUser,
                    [TenantExtensions.IdCompClaim] = usuario.IdComp
                }
            };
            return new Token { token = _handler.CreateToken(descriptor), expira = expira };
        }
    }

    public class UsersLogical(DaoUsers dao, CuadLogical cuadrillas) : CrudLogical<Users, UsersFiltro>(dao)
    {
        protected override string Nombre => "Usuario";

        protected override async Task PrepararAsync(Users e, Users? existente, Tenant tenant, CancellationToken ct)
        {
            Requerido(e.Nombre, "Nombre");
            if (e.Identificacion <= 0)
                throw new BusinessException("La identificación es obligatoria.");
            if (!Roles.EsValido(e.Rol))
                throw new BusinessException($"Rol inválido. Valores permitidos: {string.Join(", ", Roles.Todos)}.");
            // Nadie salvo Root puede crear/promover usuarios Root ni modificar a un Root
            if (!tenant.IsRoot && (e.Rol == Roles.Root || existente?.Rol == Roles.Root))
                throw new ForbiddenException("Solo un usuario Root puede gestionar usuarios Root.");
            if (existente != null && existente.Id == tenant.UserId && e.Rol != existente.Rol)
                throw new BusinessException("No puede cambiar su propio rol.");

            await base.PrepararAsync(e, existente, tenant, ct);

            if (!string.IsNullOrWhiteSpace(e.IdCuad))
            {
                var cuad = await cuadrillas.ObtenerAsync(e.IdCuad, tenant, ct);
                if (cuad.IdComp != e.IdComp)
                    throw new BusinessException("La cuadrilla no pertenece a la compañía del usuario.");
            }
            else e.IdCuad = null;

            if (string.IsNullOrWhiteSpace(e.IdEsp)) e.IdEsp = null;
        }

        protected override Task ValidarCambioAsync(Users existente, Tenant tenant, CancellationToken ct)
        {
            if (existente.Id == tenant.UserId)
                throw new BusinessException("No puede eliminar ni desactivar su propio usuario.");
            if (existente.Rol == Roles.Root && !tenant.IsRoot)
                throw new ForbiddenException("Solo un usuario Root puede gestionar usuarios Root.");
            return Task.CompletedTask;
        }
    }

    public class CredentialLogical(DaoUsuarioCredential dao, UsersLogical usuarios, TokenService tokens)
        : CrudLogical<UsuarioCredential, CredentialFiltro>(dao)
    {
        private const int LongitudMinima = 8;
        private readonly DaoUsuarioCredential _dao = dao;

        protected override string Nombre => "Credencial";

        /// <summary>Login con usuario exacto. Devuelve null si no es válido (sin revelar el motivo).</summary>
        public async Task<Token?> LoginAsync(Login login, CancellationToken ct = default)
        {
            var credencial = await _dao.GetLoginAsync(login.Usuario.Trim(), ct);
            if (credencial == null || !BCrypt.Net.BCrypt.Verify(login.Contrasenia, credencial.Hash))
                return null;
            return tokens.Generar(credencial);
        }

        /// <summary>Cambio de contraseña del propio usuario (requiere la actual).</summary>
        public async Task<Mensaje> CambiarContraseniaAsync(CambioContrasenia cambio, Tenant tenant, CancellationToken ct = default)
        {
            var actual = (await Dao.GetAsync(new CredentialFiltro { IdUser = tenant.UserId }, ct)).FirstOrDefault()
                ?? throw new NotFoundException("El usuario no tiene credenciales.");
            var login = await _dao.GetLoginAsync(actual.Usuario, ct);
            if (login == null || !BCrypt.Net.BCrypt.Verify(cambio.Actual, login.Hash))
                throw new BusinessException("La contraseña actual no es correcta.");
            ValidarContrasenia(cambio.Nueva);
            actual.Contrasenia = BCrypt.Net.BCrypt.HashPassword(cambio.Nueva);
            await Dao.SetAsync('A', actual, ct);
            return new Mensaje("Contraseña actualizada.", actual.Id);
        }

        protected override async Task PrepararAsync(UsuarioCredential e, UsuarioCredential? existente, Tenant tenant, CancellationToken ct)
        {
            e.Usuario = e.Usuario?.Trim() ?? string.Empty;
            Requerido(e.Usuario, "Usuario");
            if (existente != null)
                e.IdUser = existente.IdUser;
            Requerido(e.IdUser, "IdUser");

            // El usuario debe ser visible para el tenant (y no ser Root si quien opera no lo es)
            var usuario = await usuarios.ObtenerAsync(e.IdUser, tenant, ct);
            if (usuario.Rol == Roles.Root && !tenant.IsRoot)
                throw new ForbiddenException("Solo un usuario Root puede gestionar credenciales Root.");

            if (existente == null || !string.IsNullOrEmpty(e.Contrasenia))
            {
                ValidarContrasenia(e.Contrasenia);
                e.Contrasenia = BCrypt.Net.BCrypt.HashPassword(e.Contrasenia);
            }
        }

        private static void ValidarContrasenia(string? contrasenia)
        {
            if (string.IsNullOrEmpty(contrasenia) || contrasenia.Length < LongitudMinima)
                throw new BusinessException($"La contraseña debe tener al menos {LongitudMinima} caracteres.");
        }
    }
}

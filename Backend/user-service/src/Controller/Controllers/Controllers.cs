using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Services;
using Sync.Common.Services;
using Sync.Common.Web;

namespace Controller.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthController(CredentialLogical credenciales, UsersLogical usuarios) : ControllerBase
    {
        /// <summary>Login. Responde 401 sin detallar si falla el usuario o la contraseña.</summary>
        [HttpPost]
        [AllowAnonymous]
        [EnableRateLimiting("login")]
        public async Task<ActionResult<Token>> Login([FromBody] Login login, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(login.Usuario) || string.IsNullOrWhiteSpace(login.Contrasenia))
                return BadRequest(new Mensaje("Usuario y contraseña son requeridos."));

            var token = await credenciales.LoginAsync(login, ct);
            return token == null ? Unauthorized(new Mensaje("Usuario o contraseña incorrectos.")) : Ok(token);
        }

        /// <summary>Perfil del usuario autenticado.</summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<Users>> Me(CancellationToken ct)
        {
            var tenant = User.Tenant();
            return Ok(await usuarios.ObtenerAsync(tenant.UserId ?? string.Empty, tenant, ct));
        }

        /// <summary>Cambio de contraseña propia.</summary>
        [HttpPost("me/password")]
        [Authorize]
        public async Task<ActionResult<Mensaje>> CambiarContrasenia([FromBody] CambioContrasenia cambio, CancellationToken ct) =>
            Ok(await credenciales.CambiarContraseniaAsync(cambio, User.Tenant(), ct));
    }

    [Route("api/Users")]
    public class UsersController(UsersLogical logic) : CrudController<Users, UsersFiltro>
    {
        protected override CrudLogical<Users, UsersFiltro> Logic => logic;
        protected override string[] RolesLectura => Roles.Operacion;
    }

    [Route("api/Credential")]
    public class CredentialController(CredentialLogical logic) : CrudController<UsuarioCredential, CredentialFiltro>
    {
        protected override CrudLogical<UsuarioCredential, CredentialFiltro> Logic => logic;
        protected override string[] RolesLectura => Roles.Gestion;
    }

    [Route("api/Cuad")]
    public class CuadController(CuadLogical logic) : CrudController<Cuad, CuadFiltro>
    {
        protected override CrudLogical<Cuad, CuadFiltro> Logic => logic;
        protected override string[] RolesEscritura => Roles.Operacion;
    }

    [Route("api/Turno")]
    public class TurnoController(TurnoLogical logic) : CrudController<Turno, TurnoFiltro>
    {
        protected override CrudLogical<Turno, TurnoFiltro> Logic => logic;
    }

    [Route("api/Horario")]
    public class HorarioController(HorarioLogical logic) : CrudController<Horario, HorarioFiltro>
    {
        protected override CrudLogical<Horario, HorarioFiltro> Logic => logic;
        protected override string[] RolesEscritura => Roles.Operacion;
    }
}

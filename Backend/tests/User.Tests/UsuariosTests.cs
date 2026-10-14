using Data;
using Entity;
using Services;
using Sync.Common.Data;
using Sync.Common.Web;

namespace User.Tests
{
    internal static class T
    {
        public const string CompA = "comp-a";
        public static readonly Tenant Root = new(CompA, true, "u-root", Roles.Root);
        public static readonly Tenant AdminA = new(CompA, false, "u-admin", Roles.Admin);
        public static readonly SqlClient Sql = new("Server=fake;Database=fake");
    }

    internal sealed class FakeDaoUsers() : DaoUsers(T.Sql)
    {
        public readonly List<Users> Filas = [];
        public override Task<List<Users>> GetAsync(UsersFiltro f, CancellationToken ct = default) =>
            Task.FromResult(Filas.Where(u => (f.Id == null || u.Id == f.Id) && (f.IdComp == null || u.IdComp == f.IdComp)).ToList());
        public override Task SetAsync(char op, Users e, CancellationToken ct = default)
        {
            Filas.RemoveAll(u => u.Id == e.Id);
            Filas.Add(e);
            return Task.CompletedTask;
        }
        public override Task DeleteAsync(string id, CancellationToken ct = default) => Task.CompletedTask;
    }

    internal sealed class FakeDaoCuad() : DaoCuad(T.Sql)
    {
        public readonly List<Cuad> Filas = [];
        public override Task<List<Cuad>> GetAsync(CuadFiltro f, CancellationToken ct = default) =>
            Task.FromResult(Filas.Where(c => (f.Id == null || c.Id == f.Id) && (f.IdComp == null || c.IdComp == f.IdComp)).ToList());
    }

    internal sealed class FakeDaoTurno() : DaoTurno(T.Sql)
    {
        public readonly List<Turno> Filas = [];
        public override Task SetAsync(char op, Turno e, CancellationToken ct = default)
        {
            Filas.Add(e);
            return Task.CompletedTask;
        }
    }

    public class UsuariosTests
    {
        private static (UsersLogical logic, FakeDaoUsers dao, FakeDaoCuad cuads) Crear()
        {
            var cuads = new FakeDaoCuad();
            var dao = new FakeDaoUsers();
            return (new UsersLogical(dao, new CuadLogical(cuads)), dao, cuads);
        }

        [Fact]
        public async Task Admin_NoPuedeCrearUsuariosRoot()
        {
            var (logic, _, _) = Crear();
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                logic.CrearAsync(new Users { Nombre = "Eve", Identificacion = 1, Rol = Roles.Root }, T.AdminA));
        }

        [Fact]
        public async Task Root_PuedeCrearUsuariosRoot()
        {
            var (logic, dao, _) = Crear();
            await logic.CrearAsync(new Users { Nombre = "Ops", Identificacion = 1, Rol = Roles.Root }, T.Root);
            Assert.Equal(Roles.Root, dao.Filas.Single().Rol);
        }

        [Theory]
        [InlineData("Usuario")]
        [InlineData("admin")]
        [InlineData("")]
        public async Task RolInvalido_Es400(string rol)
        {
            var (logic, _, _) = Crear();
            await Assert.ThrowsAsync<BusinessException>(() =>
                logic.CrearAsync(new Users { Nombre = "X", Identificacion = 1, Rol = rol }, T.AdminA));
        }

        [Fact]
        public async Task SinCuadrilla_SeGuardaComoNull()
        {
            // Antes: '' violaba la FK Users -> Cuad
            var (logic, dao, _) = Crear();
            await logic.CrearAsync(new Users { Nombre = "Téc", Identificacion = 5, Rol = Roles.Tecnico, IdCuad = "" }, T.AdminA);
            Assert.Null(dao.Filas.Single().IdCuad);
        }

        [Fact]
        public async Task CuadrillaDeOtraCompania_Es404()
        {
            var (logic, _, cuads) = Crear();
            cuads.Filas.Add(new Cuad { Id = "c-b", Nombre = "B", IdComp = "comp-b" });
            await Assert.ThrowsAsync<NotFoundException>(() =>
                logic.CrearAsync(new Users { Nombre = "X", Identificacion = 1, Rol = Roles.Tecnico, IdCuad = "c-b" }, T.AdminA));
        }

        [Fact]
        public async Task NoPuedeEliminarseASiMismo()
        {
            var (logic, dao, _) = Crear();
            dao.Filas.Add(new Users { Id = T.AdminA.UserId, Nombre = "Yo", Identificacion = 1, Rol = Roles.Admin, IdComp = T.CompA });
            await Assert.ThrowsAsync<BusinessException>(() => logic.EliminarAsync(T.AdminA.UserId!, T.AdminA));
            await Assert.ThrowsAsync<BusinessException>(() => logic.CambiarEstadoAsync(T.AdminA.UserId!, false, T.AdminA));
        }

        [Fact]
        public async Task NoPuedeCambiarSuPropioRol()
        {
            var (logic, dao, _) = Crear();
            dao.Filas.Add(new Users { Id = T.AdminA.UserId, Nombre = "Yo", Identificacion = 1, Rol = Roles.Admin, IdComp = T.CompA });
            await Assert.ThrowsAsync<BusinessException>(() =>
                logic.ActualizarAsync(T.AdminA.UserId!, new Users { Nombre = "Yo", Identificacion = 1, Rol = Roles.Tecnico }, T.AdminA));
        }

        [Theory]
        [InlineData("6:00", "14:00", "06:00", "14:00")]
        [InlineData("22:00", "06:00", "22:00", "06:00")]
        public async Task Turno_NormalizaHoras(string ini, string fin, string iniEsperado, string finEsperado)
        {
            var dao = new FakeDaoTurno();
            await new TurnoLogical(dao).CrearAsync(new Turno { Nombre = "T", HoraInicio = ini, HoraFin = fin }, T.AdminA);
            Assert.Equal(iniEsperado, dao.Filas.Single().HoraInicio);
            Assert.Equal(finEsperado, dao.Filas.Single().HoraFin);
        }

        [Theory]
        [InlineData("25:00", "14:00")]
        [InlineData("abc", "14:00")]
        [InlineData("08:00", "08:00")]
        public async Task Turno_HorasInvalidas_Es400(string ini, string fin)
        {
            await Assert.ThrowsAsync<BusinessException>(() =>
                new TurnoLogical(new FakeDaoTurno()).CrearAsync(new Turno { Nombre = "T", HoraInicio = ini, HoraFin = fin }, T.AdminA));
        }

        [Fact]
        public async Task Credencial_ContraseniaCorta_Es400()
        {
            var (users, dao, _) = Crear();
            dao.Filas.Add(new Users { Id = "u1", Nombre = "X", Identificacion = 1, Rol = Roles.Tecnico, IdComp = T.CompA });
            var tokens = new TokenService(new JwtOptions { SecretKey = new string('k', 40), Issuer = "i", Audience = "a" });
            var logic = new CredentialLogical(new DaoUsuarioCredential(T.Sql), users, tokens);

            await Assert.ThrowsAsync<BusinessException>(() =>
                logic.CrearAsync(new UsuarioCredential { Usuario = "x", Contrasenia = "1234567", IdUser = "u1" }, T.AdminA));
        }

        [Fact]
        public void Token_IncluyeClaimsQueUsaElFrontend()
        {
            var tokens = new TokenService(new JwtOptions { SecretKey = new string('k', 40), Issuer = "i", Audience = "a", ExpirationHours = 1 });
            var t = tokens.Generar(new CredencialLogin { IdUser = "u1", Nombre = "Ana", Rol = Roles.Admin, IdComp = T.CompA });

            var jwt = new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(t.token);
            Assert.Equal(T.CompA, jwt.GetClaim("IdComp").Value);
            Assert.Equal(Roles.Admin, jwt.GetClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value);
            Assert.Equal("u1", jwt.GetClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
            Assert.True(t.expira > DateTime.UtcNow);
        }
    }
}

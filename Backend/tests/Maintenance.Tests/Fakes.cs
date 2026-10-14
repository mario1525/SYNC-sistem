using Data;
using Entity;
using Sync.Common.Data;
using Sync.Common.Web;

namespace Maintenance.Tests
{
    /// <summary>Contextos de usuario de prueba.</summary>
    internal static class T
    {
        public const string CompA = "comp-a";
        public const string CompB = "comp-b";

        public static readonly Tenant Root = new(CompA, true, "u-root", Roles.Root);
        public static readonly Tenant AdminA = new(CompA, false, "u-admin-a", Roles.Admin);
        public static readonly Tenant AdminB = new(CompB, false, "u-admin-b", Roles.Admin);

        /// <summary>SqlClient que nunca se usa (los DAO falsos no tocan la BD).</summary>
        public static readonly SqlClient Sql = new("Server=fake;Database=fake");
    }

    /// <summary>Almacén en memoria con la misma semántica de filtro Id/IdComp que los SP.</summary>
    internal sealed class Memoria<T> where T : IEntidad
    {
        public readonly List<T> Filas = [];
        private readonly Func<T, string?> _idComp;

        public Memoria(Func<T, string?> idComp) => _idComp = idComp;

        public Task<List<T>> Get(IFiltro f) => Task.FromResult(Filas
            .Where(r => (f.Id == null || r.Id == f.Id) && (f.IdComp == null || _idComp(r) == f.IdComp || _idComp(r) == null))
            .ToList());

        public Task Set(T e)
        {
            Filas.RemoveAll(r => r.Id == e.Id);
            Filas.Add(e);
            return Task.CompletedTask;
        }
    }

    internal sealed class FakeDaoPlanta() : DaoPlanta(T.Sql)
    {
        public readonly Memoria<Planta> Db = new(p => p.IdComp);
        public override Task<List<Planta>> GetAsync(PlantaFiltro f, CancellationToken ct = default) => Db.Get(f);
        public override Task SetAsync(char op, Planta e, CancellationToken ct = default) => Db.Set(e);
    }

    internal sealed class FakeDaoEsp() : DaoEsp(T.Sql)
    {
        public readonly Memoria<Esp> Db = new(e => e.IdComp);
        public override Task<List<Esp>> GetAsync(EspFiltro f, CancellationToken ct = default) => Db.Get(f);
        public override Task SetAsync(char op, Esp e, CancellationToken ct = default) => Db.Set(e);
        public override Task DeleteAsync(string id, CancellationToken ct = default) => Task.CompletedTask;
    }

    internal sealed class FakeDaoArea() : DaoArea(T.Sql)
    {
        public readonly Memoria<Area> Db = new(a => a.IdComp);
        public override Task<List<Area>> GetAsync(AreaFiltro f, CancellationToken ct = default) => Db.Get(f);
        public override Task SetAsync(char op, Area e, CancellationToken ct = default) => Db.Set(e);
    }

    internal sealed class FakeDaoSeccion() : DaoSeccionArea(T.Sql)
    {
        public readonly Memoria<SeccionArea> Db = new(s => s.IdComp);
        public override Task<List<SeccionArea>> GetAsync(SeccionAreaFiltro f, CancellationToken ct = default) => Db.Get(f);
        public override Task SetAsync(char op, SeccionArea e, CancellationToken ct = default) => Db.Set(e);
    }

    internal sealed class FakeDaoTipoArea() : DaoTipoArea(T.Sql)
    {
        public readonly Memoria<TipoArea> Db = new(_ => null);
        public override Task<List<TipoArea>> GetAsync(TipoAreaFiltro f, CancellationToken ct = default) => Db.Get(f);
    }

    internal sealed class FakeDaoEquipo() : DaoEquipo(T.Sql)
    {
        public readonly Memoria<Equipo> Db = new(e => e.IdComp);
        public override Task<List<Equipo>> GetAsync(EquipoFiltro f, CancellationToken ct = default) => Db.Get(f);
        public override Task SetAsync(char op, Equipo e, CancellationToken ct = default) => Db.Set(e);
    }

    internal sealed class FakeDaoGuia() : DaoGuia(T.Sql)
    {
        public readonly Memoria<Guia> Db = new(g => g.IdComp);
        public override Task<List<Guia>> GetAsync(GuiaFiltro f, CancellationToken ct = default) => Db.Get(f);
        public override Task SetAsync(char op, Guia e, CancellationToken ct = default) => Db.Set(e);
    }
}

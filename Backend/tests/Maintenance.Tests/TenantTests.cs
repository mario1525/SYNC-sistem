using Data;
using Entity;
using Services;
using Sync.Common.Web;

namespace Maintenance.Tests
{
    /// <summary>Aislamiento entre compañías (ADR-05) sobre la lógica CRUD común.</summary>
    public class TenantTests
    {
        [Fact]
        public void Filtro_NoRoot_IgnoraLaCompaniaSolicitada()
        {
            Assert.Equal(T.CompA, T.AdminA.Filtro(T.CompB));
            Assert.Equal(T.CompB, T.Root.Filtro(T.CompB));
            Assert.Null(T.Root.Filtro(null));
        }

        [Fact]
        public void ParaEscritura_SinCompania_Lanza()
        {
            var huerfano = new Tenant(null, false, "u", Roles.Admin);
            Assert.Throws<BusinessException>(() => huerfano.ParaEscritura(null));
        }

        [Fact]
        public async Task Crear_AdminForzadoASuCompania_AunqueEnvieOtra()
        {
            var dao = new FakeDaoPlanta();
            var logic = new PlantaLogical(dao);

            var r = await logic.CrearAsync(new Planta { Nombre = "P1", IdComp = T.CompB }, T.AdminA);

            Assert.Equal(T.CompA, dao.Db.Filas.Single(p => p.Id == r.id).IdComp);
        }

        [Fact]
        public async Task Crear_RootUsaLaCompaniaIndicada()
        {
            var dao = new FakeDaoPlanta();
            var r = await new PlantaLogical(dao).CrearAsync(new Planta { Nombre = "P1", IdComp = T.CompB }, T.Root);
            Assert.Equal(T.CompB, dao.Db.Filas.Single(p => p.Id == r.id).IdComp);
        }

        [Fact]
        public async Task Actualizar_ConservaLaCompaniaOriginal()
        {
            var dao = new FakeDaoPlanta();
            dao.Db.Filas.Add(new Planta { Id = "p1", Nombre = "P1", IdComp = T.CompA });

            await new PlantaLogical(dao).ActualizarAsync("p1", new Planta { Nombre = "Nuevo", IdComp = T.CompB }, T.Root);

            var p = dao.Db.Filas.Single();
            Assert.Equal("Nuevo", p.Nombre);
            Assert.Equal(T.CompA, p.IdComp);
        }

        [Fact]
        public async Task Obtener_RegistroDeOtraCompania_Es404()
        {
            var dao = new FakeDaoPlanta();
            dao.Db.Filas.Add(new Planta { Id = "p1", Nombre = "P1", IdComp = T.CompA });
            var logic = new PlantaLogical(dao);

            await Assert.ThrowsAsync<NotFoundException>(() => logic.ObtenerAsync("p1", T.AdminB));
            await Assert.ThrowsAsync<NotFoundException>(() => logic.ActualizarAsync("p1", new Planta { Nombre = "x" }, T.AdminB));
            await Assert.ThrowsAsync<NotFoundException>(() => logic.EliminarAsync("p1", T.AdminB));
        }

        [Fact]
        public async Task Listar_NoRoot_SoloSuCompania()
        {
            var dao = new FakeDaoPlanta();
            dao.Db.Filas.Add(new Planta { Id = "a", Nombre = "A", IdComp = T.CompA });
            dao.Db.Filas.Add(new Planta { Id = "b", Nombre = "B", IdComp = T.CompB });

            var lista = await new PlantaLogical(dao).ListarAsync(new PlantaFiltro { IdComp = T.CompA }, T.AdminB);

            Assert.Equal("b", Assert.Single(lista).Id);
        }

        [Fact]
        public async Task CrearHijo_ConPadreDeOtraCompania_Es404()
        {
            var plantas = new FakeDaoPlanta();
            plantas.Db.Filas.Add(new Planta { Id = "pA", Nombre = "Planta A", IdComp = T.CompA });
            var tipos = new FakeDaoTipoArea();
            tipos.Db.Filas.Add(new TipoArea { Id = TipoArea.Produccion, Nombre = "Produccion" });
            var areas = new AreaLogical(new FakeDaoArea(), new PlantaLogical(plantas), new TipoAreaLogical(tipos));

            await Assert.ThrowsAsync<NotFoundException>(() =>
                areas.CrearAsync(new Area { IdPlanta = "pA", Nombre = "Intrusa", TipoArea = TipoArea.Produccion }, T.AdminB));
            var ok = await areas.CrearAsync(new Area { IdPlanta = "pA", Nombre = "Línea 1", TipoArea = TipoArea.Produccion }, T.AdminA);
            Assert.NotNull(ok.id);
        }

        [Fact]
        public async Task EspecialidadGlobal_SoloLaModificaRoot()
        {
            var dao = new FakeDaoEsp();
            dao.Db.Filas.Add(new Esp { Id = "global", Nombre = "Mecánica", IdComp = null });
            var logic = new EspLogical(dao);

            await Assert.ThrowsAsync<ForbiddenException>(() => logic.ActualizarAsync("global", new Esp { Nombre = "x" }, T.AdminA));
            await Assert.ThrowsAsync<ForbiddenException>(() => logic.EliminarAsync("global", T.AdminA));
            await logic.ActualizarAsync("global", new Esp { Nombre = "Mecánica industrial" }, T.Root);
            Assert.Null(dao.Db.Filas.Single().IdComp);
        }
    }
}

using Entity;
using Services;
using Sync.Common.Web;

namespace Maintenance.Tests
{
    public class ReglasTests
    {
        private static (EquipoLogical logic, FakeDaoEquipo dao, FakeDaoSeccion secciones) Equipos()
        {
            var secciones = new FakeDaoSeccion();
            var plantas = new FakeDaoPlanta();
            var areas = new AreaLogical(new FakeDaoArea(), new PlantaLogical(plantas), new TipoAreaLogical(new FakeDaoTipoArea()));
            var dao = new FakeDaoEquipo();
            return (new EquipoLogical(dao, new SeccionAreaLogical(secciones, areas)), dao, secciones);
        }

        [Fact]
        public async Task Equipo_SinUbicacion_SeCreaSinError()
        {
            // Antes: NullReferenceException en equipo.Ubicacion.Id
            var (logic, dao, _) = Equipos();
            await logic.CrearAsync(new Equipo { Nombre = "Bomba" }, T.AdminA);
            Assert.Null(dao.Db.Filas.Single().Ubicacion);
        }

        [Theory]
        [InlineData("Bodega")]
        [InlineData("")]
        [InlineData("produccion")]
        public async Task Equipo_TipoUbicacionInvalido_Es400(string tipo)
        {
            var (logic, _, _) = Equipos();
            await Assert.ThrowsAsync<BusinessException>(() =>
                logic.CrearAsync(new Equipo { Nombre = "Bomba", Ubicacion = new UbicacionEquipo { TipoUbicacion = tipo } }, T.AdminA));
        }

        [Fact]
        public async Task Equipo_SeccionDeOtraCompania_Es404()
        {
            var (logic, _, secciones) = Equipos();
            secciones.Db.Filas.Add(new SeccionArea { Id = "sB", Nombre = "B", IdArea = "x", IdComp = T.CompB });
            await Assert.ThrowsAsync<NotFoundException>(() => logic.CrearAsync(new Equipo
            {
                Nombre = "Bomba",
                Ubicacion = new UbicacionEquipo { TipoUbicacion = UbicacionEquipo.Produccion, IdSeccionArea = "sB" }
            }, T.AdminA));
        }

        [Fact]
        public async Task Equipo_AnioFabricacionFueraDeRango_Es400()
        {
            var (logic, _, _) = Equipos();
            await Assert.ThrowsAsync<BusinessException>(() => logic.CrearAsync(new Equipo { Nombre = "X", AnioFabricacion = 1800 }, T.AdminA));
        }

        [Fact]
        public async Task Guia_AsignaIdsAlArbolYRegistraAutor()
        {
            var dao = new FakeDaoGuia();
            var logic = new GuiaLogical(dao, null!, null!, new EspLogical(new FakeDaoEsp()));
            var guia = new Guia
            {
                Nombre = "PM bomba",
                proced = [new Proced { Nombre = "Paso 1", valid = [new Valid { Nombre = "V1" }, new Valid { Id = "v-existente", Nombre = "V2" }] }]
            };

            await logic.CrearAsync(guia, T.AdminA);

            var g = dao.Db.Filas.Single();
            Assert.Equal(T.AdminA.UserId, g.UpdatedBy);
            Assert.False(string.IsNullOrEmpty(g.proced![0].Id));
            Assert.False(string.IsNullOrEmpty(g.proced[0].valid![0].Id));
            Assert.Equal("v-existente", g.proced[0].valid![1].Id);
        }

        [Fact]
        public async Task Guia_PasoSinNombre_Es400()
        {
            var logic = new GuiaLogical(new FakeDaoGuia(), null!, null!, new EspLogical(new FakeDaoEsp()));
            await Assert.ThrowsAsync<BusinessException>(() =>
                logic.CrearAsync(new Guia { Nombre = "G", proced = [new Proced { Nombre = "" }] }, T.AdminA));
        }

        [Fact]
        public async Task Guia_ValoresNegativos_Es400()
        {
            var logic = new GuiaLogical(new FakeDaoGuia(), null!, null!, new EspLogical(new FakeDaoEsp()));
            await Assert.ThrowsAsync<BusinessException>(() => logic.CrearAsync(new Guia { Nombre = "G", Duracion = -1 }, T.AdminA));
        }
    }
}

using Data;
using Entity;
using Services;
using Sync.Common.Web;

namespace Maintenance.Tests
{
    /// <summary>Reglas de la operación en campo: reportes de falla y ejecución del checklist.</summary>
    public class CampoTests
    {
        private static readonly Tenant TecnicoA = new(T.CompA, false, "u-tec-a", Roles.Tecnico);

        // ------------------------------------------------------------ Fallas

        private static (FallaLogical logic, FakeDaoFalla dao) Fallas()
        {
            var equipos = new FakeDaoEquipo();
            equipos.Db.Filas.Add(new Equipo { Id = "eqA", Nombre = "Bomba", IdComp = T.CompA });
            equipos.Db.Filas.Add(new Equipo { Id = "eqB", Nombre = "Compresor", IdComp = T.CompB });
            var dao = new FakeDaoFalla();
            return (new FallaLogical(dao, new EquipoLogical(equipos, null!)), dao);
        }

        [Fact]
        public async Task Falla_NuevaQuedaAbiertaYRegistraQuienReporta()
        {
            var (logic, dao) = Fallas();
            await logic.CrearAsync(new Falla { IdEquipo = "eqA", Tipo = "Fuga", Prioridad = "Alta", EstadoFalla = "Cerrada" }, TecnicoA);

            var f = dao.Db.Filas.Single();
            Assert.Equal("Abierta", f.EstadoFalla);
            Assert.Equal(TecnicoA.UserId, f.IdUserReporta);
            Assert.Equal(T.CompA, f.IdComp);
        }

        [Fact]
        public async Task Falla_EquipoDeOtraCompania_Es404()
        {
            var (logic, _) = Fallas();
            await Assert.ThrowsAsync<NotFoundException>(() =>
                logic.CrearAsync(new Falla { IdEquipo = "eqB", Tipo = "Fuga", Prioridad = "Alta" }, TecnicoA));
        }

        [Theory]
        [InlineData("Explosion", "Alta")]
        [InlineData("Fuga", "Urgente")]
        public async Task Falla_TipoOPrioridadInvalidos_Es400(string tipo, string prioridad)
        {
            var (logic, _) = Fallas();
            await Assert.ThrowsAsync<BusinessException>(() =>
                logic.CrearAsync(new Falla { IdEquipo = "eqA", Tipo = tipo, Prioridad = prioridad }, TecnicoA));
        }

        // ------------------------------------------------------------ Ejecución

        private static (EjecucionLogical logic, FakeDaoActividadEquipo ae, FakeDaoAdjunto adjuntos) Ejecucion()
        {
            var guias = new FakeDaoGuia();
            guias.Db.Filas.Add(new Guia { Id = "g1", Nombre = "PM", IdComp = T.CompA });
            var pasos = new FakeDaoProced();
            pasos.Filas.Add(new Proced { Id = "p1", IdGuia = "g1", Nombre = "Paso 1" });
            var verificaciones = new FakeDaoValid();
            verificaciones.Filas.Add(new Valid { Id = "v1", IdProced = "p1", IdGuia = "g1", Nombre = "Sin fugas" });
            verificaciones.Filas.Add(new Valid { Id = "v2", IdProced = "p1", IdGuia = "g1", Nombre = "Sello nuevo", RequiereFoto = true });

            var ae = new FakeDaoActividadEquipo();
            ae.Db.Filas.Add(new ActividadEquipo { Id = "ae1", IdActividad = "a1", IdEquipo = "eqA", IdGuia = "g1" });
            var adjuntos = new FakeDaoAdjunto();

            var guiaLogic = new GuiaLogical(guias, pasos, verificaciones, null!);
            var aeLogic = new ActividadEquipoLogical(ae, null!, null!, guiaLogic);
            return (new EjecucionLogical(ae, aeLogic, guiaLogic, adjuntos), ae, adjuntos);
        }

        [Fact]
        public async Task Ejecucion_VerificacionDeOtraGuia_Es404()
        {
            var (logic, _, _) = Ejecucion();
            await Assert.ThrowsAsync<NotFoundException>(() =>
                logic.MarcarAsync("ae1", "v-otra", new ResultadoValid { Cumple = true }, TecnicoA));
        }

        [Fact]
        public async Task Ejecucion_FinalizarConPendientes_Es400()
        {
            var (logic, ae, _) = Ejecucion();
            await logic.MarcarAsync("ae1", "v1", new ResultadoValid { Cumple = true }, TecnicoA);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => logic.FinalizarAsync("ae1", new CierreEjecucion(), TecnicoA));
            Assert.Contains("1 verificaciones", ex.Message);
            Assert.Null(ae.EstadoFinal);
        }

        [Fact]
        public async Task Ejecucion_FinalizarSinFotoRequerida_Es400()
        {
            var (logic, ae, _) = Ejecucion();
            await logic.MarcarAsync("ae1", "v1", new ResultadoValid { Cumple = true }, TecnicoA);
            await logic.MarcarAsync("ae1", "v2", new ResultadoValid { Cumple = true }, TecnicoA);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => logic.FinalizarAsync("ae1", new CierreEjecucion(), TecnicoA));
            Assert.Contains("foto", ex.Message);
            Assert.Null(ae.EstadoFinal);
        }

        [Fact]
        public async Task Ejecucion_CompletaConFoto_SeFinalizaYNoAdmiteCambios()
        {
            var (logic, ae, adjuntos) = Ejecucion();
            await logic.MarcarAsync("ae1", "v1", new ResultadoValid { Cumple = true }, TecnicoA);
            await logic.MarcarAsync("ae1", "v2", new ResultadoValid { Cumple = true }, TecnicoA);
            adjuntos.Filas.Add(new Adjunto { Id = "f1", Entidad = Adjunto.Ejecucion, IdEntidad = "ae1", Ref = "v2" });

            await logic.FinalizarAsync("ae1", new CierreEjecucion { Observacion = "OK" }, TecnicoA);
            Assert.Equal(ActividadEquipo.Completada, ae.EstadoFinal);

            ae.Db.Filas.Single().EstadoEjec = ActividadEquipo.Completada;
            await Assert.ThrowsAsync<BusinessException>(() =>
                logic.MarcarAsync("ae1", "v1", new ResultadoValid { Cumple = false }, TecnicoA));
        }
    }

    // ------------------------------------------------------------ Fakes

    internal sealed class FakeDaoFalla() : DaoFalla(T.Sql)
    {
        public new readonly Memoria<Falla> Db = new(f => f.IdComp);
        public override Task<List<Falla>> GetAsync(FallaFiltro f, CancellationToken ct = default) => Db.Get(f);
        public override Task SetAsync(char op, Falla e, CancellationToken ct = default) => Db.Set(e);
    }

    internal sealed class FakeDaoActividadEquipo() : DaoActividadEquipo(T.Sql)
    {
        public new readonly Memoria<ActividadEquipo> Db = new(_ => null);
        public readonly List<EjecucionValid> Resultados = [];
        public string? EstadoFinal;

        public override Task<List<ActividadEquipo>> GetAsync(ActividadEquipoFiltro f, CancellationToken ct = default) => Db.Get(f);

        public override Task<List<EjecucionValid>> GetResultadosAsync(string id, CancellationToken ct = default) =>
            Task.FromResult(Resultados.Where(r => r.IdActividadEquipo == id).ToList());

        public override Task SetResultadoAsync(EjecucionValid e, CancellationToken ct = default)
        {
            Resultados.RemoveAll(r => r.IdActividadEquipo == e.IdActividadEquipo && r.IdValid == e.IdValid);
            Resultados.Add(e);
            return Task.CompletedTask;
        }

        public override Task SetEjecucionAsync(string id, string estadoEjec, string? idUser, string? observacion, CancellationToken ct = default)
        {
            EstadoFinal = estadoEjec;
            return Task.CompletedTask;
        }
    }

    internal sealed class FakeDaoProced() : DaoProced(T.Sql)
    {
        public readonly List<Proced> Filas = [];
        public override Task<List<Proced>> GetAsync(ProcedFiltro f, CancellationToken ct = default) =>
            Task.FromResult(Filas.Where(p => f.IdGuia == null || p.IdGuia == f.IdGuia).ToList());
    }

    internal sealed class FakeDaoValid() : DaoValid(T.Sql)
    {
        public readonly List<Valid> Filas = [];
        public override Task<List<Valid>> GetAsync(ValidFiltro f, CancellationToken ct = default) =>
            Task.FromResult(Filas.Where(v => f.IdGuia == null || v.IdGuia == f.IdGuia).ToList());
    }

    internal sealed class FakeDaoAdjunto() : DaoAdjunto(T.Sql)
    {
        public readonly List<Adjunto> Filas = [];
        public override Task<List<Adjunto>> GetAsync(string? id, string? idComp, string? entidad, string? idEntidad, CancellationToken ct = default) =>
            Task.FromResult(Filas.Where(a => (id == null || a.Id == id) && (entidad == null || a.Entidad == entidad)
                                             && (idEntidad == null || a.IdEntidad == idEntidad)).ToList());
    }
}

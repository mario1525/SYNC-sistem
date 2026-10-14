# SYNC — Diagnóstico, plan de trabajo y plan del modelo predictivo

> Fecha del escaneo: 2026-10-13 · Último commit analizado: `3a63aad refactor proyect` (2025-11-16)
> Alcance: `Backend/` (api-gateway, user-service, maintenace-service, db-migrator), `Database/`, `Docker/`, `Frontend/`.

---

## 0. Estado de ejecución (actualizado 2026-10-13, rama `fase-0-1-2`)

**Fases 0, 1 y 2: implementadas y verificadas.** Las secciones 1 y 2 describen el estado *anterior* (diagnóstico original).

| Verificación | Resultado |
|---|---|
| `dotnet build` de la solución `Backend/Sync.sln` (.NET 10) | ✅ sin errores |
| Pruebas unitarias backend (xUnit) | ✅ 34/34 |
| Migraciones sobre BD vacía y re-ejecución (idempotencia) | ✅ |
| Smoke test end-to-end a través del gateway, stack completo en Docker | ✅ 49/49 verificaciones (auth, CRUD, ubicaciones, árbol de guía, tenant, roles, errores) |
| Frontend Angular 22: lint, pruebas (Vitest) y build de producción | ✅ lint limpio · 8/8 pruebas · build OK |
| Verificación manual en navegador (login, equipos, planta, guía, cuadrilla, usuarios, perfil, móvil) | ✅ |

**Desviaciones respecto al plan original** (decididas durante la ejecución):

- **ADR-02 ajustado:** las tablas existentes se mantienen en `dbo` para no romper datos ni contratos; los esquemas `ops`/`pdm` se usarán para las tablas nuevas de las fases 3 y 5.
- **ADR-04 ajustado:** se conserva el prefijo `dbo.db_Sp_<Entidad>_<Op>`, pero ahora hay **un solo nombre por entidad** y todos los SP son `CREATE OR ALTER`. Los duplicados y obsoletos se eliminan en `099__obsoletos.sql`.
- **Logging:** se usa el logger JSON nativo de .NET (estructurado, con trace/span id) en lugar de Serilog, para no añadir dependencias.
- **Tests de integración de SP:** en lugar de Testcontainers, el smoke test corre contra el stack real en Docker, tanto en local como en CI.
- **Frontend reconstruido** (no migrado por pasos): standalone, signals y sin zone.js, con layout común y servicio CRUD genérico. La fase 2 cubría Actividad, Cuadrilla, Turnos, Horarios, Perfil, Usuarios, Área y Sección, Equipo con ubicación y guías, y el editor de Proced→Valid; todo quedó implementado.

**Pendientes de las fases 0–2** (no bloquean la fase 3):

- [ ] Login SQL con mínimo privilegio (`sync_app`, solo EXECUTE): el compose todavía usa `sa`.
- [ ] Los secretos antiguos (clave JWT y contraseña SA) siguen en el **historial de git** y deben considerarse comprometidos. Los nuevos viven solo en `Docker/.env`.
- [ ] Node local ≥ 22.22 para ejecutar `ng serve` fuera de Docker (la máquina actual tiene la 22.19).
- [ ] `SMALLDATETIME` tiene precisión de minuto; para la fase 3 (OT, lecturas) se recomienda `DATETIME2`.

---

## 1. Estado actual (resumen)

| Capa | Qué existe | Madurez |
|---|---|---|
| **Base de datos** | `script.sql` (22 tablas en una sola BD `DB_SYNC`), 2 paquetes de SP unificados + 20 archivos SP sueltos | 60 % — desalineada con el código |
| **user-service** | Login JWT + BCrypt, CRUD de Users, Credential, Cuad | 65 % — faltan Turno/Horario y el aislamiento por compañía (tenant) |
| **maintenace-service** | 15 controladores CRUD (Comp, Planta, Equipo, Guía, Proced, Valid, Actividad, etc.) | 45 % — solo maestros; ninguna ejecución de mantenimiento |
| **api-gateway** | Ocelot con unas 40 rutas, CORS | 50 % — rutas incompletas y sin validación de auth |
| **db-migrator** | Programa DbUp listo | 20 % — **no hay carpeta `sql/`**, ni Dockerfile, ni entrada en compose |
| **Frontend** | Angular 17 + Material: login, Comp/Planta/Bodega, Equipo, Guía y lista de usuarios | 40 % — Actividad, Cuadrilla, Perfil, Registro, Bodega y Área funcional son esqueletos; no hay nada de PWA ni offline |
| **Infra / calidad** | docker-compose con 4 servicios | 15 % — sin tests reales, sin CI, sin logging, secretos en el repo |
| **Mantenimiento predictivo** | — | 0 % — tampoco existen los datos que lo alimentarían |

**Conclusión:** SYNC hoy es un **catálogo de activos y guías** (qué equipos hay, dónde están y cómo se mantienen). Le falta el **núcleo operativo**: órdenes de trabajo, ejecución, historial, fallas y lecturas. Sin ese núcleo no hay datos para un modelo predictivo, así que el plan construye primero el núcleo y después el modelo.

---

## 2. Hallazgos críticos (corregir primero)

### 2.1 Bugs que pierden datos o responden en falso
1. **`async void` en 56 métodos DAO** (`Backend/maintenace-service/src/maintenace-service/Data/*.cs`, p. ej. `DaoEquipo.SetEquipo`). Los servicios `*Logical` los llaman sin `await` (`EquipoLogical.CreateEquipo`). Consecuencias:
   - la API responde `"EQP-0000000000"` / "Equipo actualizado" **aunque el INSERT falle**;
   - una excepción en un `async void` puede **tumbar el proceso** (no se puede capturar).
   → Cambiar todos a `async Task` y poner `await` en toda la cadena.
2. **`CreateEquipo` lanza NullReferenceException** si el body no trae `Ubicacion` (`equipo.Ubicacion.Id = ...`). Además la ubicación **nunca se persiste**: el comentario del DAO dice que debe llamarse `DaoUbicacionEquipo`, pero nadie lo hace.
3. **Valores de ubicación incompatibles:** el front envía `tipoUbicacion: 'Bodega'` y la tabla tiene `CHECK (TipoUbicacion IN ('Almacen','Produccion'))`. Cada insert desde el formulario fallará.

### 2.2 Esquema ↔ código desalineados
4. **Las tablas `Bodega` y `Patio` no existen** en `script.sql`, pero existen `SP_Bodega.sql`, `SP_Patio.sql`, sus DAO, controladores, servicios del front y rutas del gateway. El esquema real usa `TipoArea / Area / SeccionArea`. `DaoAreaFuncional` llama `db_Sp_Area_*` y `DaoSeccionBodega` llama `db_Sp_SeccionArea_*`: hay dos modelos de ubicación conviviendo.
5. **Tres convenciones de nombres para el mismo SP:**
   - `DaoActividadEquipo` → `db_Sp_Act_Equipo_*` (**no existe en ningún script**)
   - `SP_Act_equipo.sql` → `db_Sp_Guia_Act_Equipo_*`
   - `SP_Unified_Operations.sql` → `db_Sp_ActividadEquipo_*`
   - Igual con `Guia_Equipo` y `GuiaEquipo`. El resultado depende del orden en que se ejecuten los scripts.
6. **Borrado inconsistente:** casi todo usa soft-delete (`Eliminado=1`), pero `db_Sp_Bodega_Del` y `db_Sp_Patio_Del` hacen `DELETE` físico. `Planta`, `Area`, `SeccionArea` y `UbicacionEquipo` no tienen columna `Eliminado`.
7. **`Actividad.IdCuad`** no tiene FK. `Equipo.Seccion` es texto libre, redundante con `UbicacionEquipo`.

### 2.3 Configuración y seguridad
8. **Las variables de entorno se ignoran:** `Program.cs` arma su propio `ConfigurationBuilder` solo con `appsettings.json` y lee `Configuracion:connectionString`. El compose define `ConnectionStrings__DefaultConnection` (con catálogo `ProductTrack`), pero no se usa.
9. **Secretos en el repositorio:** la clave JWT y la contraseña de SA están en `appsettings.json`, `docker-compose.yml` y `db-migrator/Program.cs`. Hay que rotarlos y moverlos a `.env` o user-secrets.
10. **No hay aislamiento entre compañías (multi-tenant):** el JWT trae el claim `IdComp`, pero los endpoints aceptan `IdComp` por query string. Un Admin de la compañía A puede listar equipos de la compañía B.
11. **El gateway no valida tokens**, solo reenvía. Faltan rutas para `PATCH {id}/estado`, `UbicacionEquipo`, `Valid`, `GET {id}` en varias entidades, y Turno/Horario.
12. SQL Server con usuario **SA** para las aplicaciones. Conviene un login con permisos mínimos (solo EXECUTE).

### 2.4 Deuda estructural
13. **.NET 7 está fuera de soporte desde mayo de 2024.** Migrar a **.NET 10 LTS**. .NET 8 también deja de tener soporte en noviembre de 2026, así que no vale la pena como paso intermedio.
14. **Angular 17 está fuera de soporte.** Migrar paso a paso a la versión vigente (`ng update`, una major a la vez). Hoy conviven `angular-eslint 19` con Angular 17.
15. **Boilerplate duplicado:** cada servicio tiene Razor Pages, `wwwroot/lib` (bootstrap/jquery) y proyectos host fantasma (`maintenace-service/maintenace-service`, `user-service/Backend`, `user-service/Serv-usuarios`). `api-gateway` conserva `WeatherForecastController`.
16. **Paquetes NuGet sobrantes o en conflicto:** `Microsoft.AspNet.Cors 5.3` (.NET Framework), `Swagger.Net`, dos versiones de Swashbuckle y `System.Data.SqlClient` junto con `Microsoft.Data.SqlClient`.
17. Typos que se arrastran: `maintenace`, `envirodment.prod.ts`, `ActividadEquipoCotroller.cs`, `UsusarioLogical.cs`.
18. **El README promete** PWA, offline, código de barras, notificaciones, Consul/Eureka, EF Core y notification-service. **Nada de eso existe.** Hay que alinear el README o el roadmap.
19. **Sin tests** (solo los `.spec.ts` autogenerados), **sin CI** y con logging por `Console.WriteLine`.

---

## 3. Decisiones de diseño (ADR resumidas)

| # | Decisión | Alternativa descartada | Por qué |
|---|---|---|---|
| ADR-01 | **Mantener stored procedures + ADO.NET**, migrados con **DbUp** | Pasar a EF Core | Ya hay unos 90 SP funcionando. Reescribir no agrega valor ahora. DbUp ya está montado. |
| ADR-02 | **Una sola BD `DB_SYNC` con esquemas por dominio** (`usr`, `mnt`, `ops`, `pdm`) | Una BD por microservicio (lo que asume el migrator) | `Users` tiene FK a `Comp`/`Esp`, que viven en maintenance. Separar BDs rompe esas FK. Los esquemas dan límites claros sin coste operativo. |
| ADR-03 | **Modelo de ubicación único:** `Planta → Area (TipoArea: Producción/Bodega/Patio) → SeccionArea → UbicacionEquipo` | Mantener tablas `Bodega`/`Patio` aparte | El esquema ya lo modela así. Bodega y Patio pasan a ser *tipos de área*. Se eliminan `SP_Bodega`/`SP_Patio` y los DAO/rutas duplicados, y el front pasa a usar Área + TipoArea. |
| ADR-04 | **Convención de SP:** `<esquema>.sp_<Entidad>_<Get|Set|Del|Active>` con `CREATE OR ALTER` | Las tres convenciones actuales | Un solo nombre por entidad, idempotente y verificable en CI. |
| ADR-05 | **Tenant = `IdComp` tomado del JWT**, nunca del query string (Root es la excepción) | Confiar en el cliente | Cierra la fuga de datos entre compañías (hallazgo 10). |
| ADR-06 | **Gateway valida el JWT** (Ocelot `AuthenticationOptions`) y los servicios lo vuelven a validar | Solo validan los servicios | Defensa en profundidad y rechazo temprano. |
| ADR-07 | **Modelo predictivo como microservicio Python** (`predictive-service`: FastAPI + scikit-learn/LightGBM) | Implementarlo en C# con ML.NET | El ecosistema de ML, survival analysis y experimentación está en Python. Se integra por HTTP vía el gateway y por lectura de la BD. |
| ADR-08 | **Predicción por lotes (batch nocturno) que escribe en `pdm.Prediccion`**, más un endpoint on-demand | Inferencia en tiempo real por request | El front solo lee una tabla (rápido y apto para offline). El riesgo de falla no cambia minuto a minuto si no hay sensores. |
| ADR-09 | **Modelo predictivo por escalones:** reglas/estadística → ML supervisado → RUL con sensores | Ir directo a deep learning | No hay datos históricos. Cada escalón entrega valor y genera los datos del siguiente. |
| ADR-10 | **Offline (PWA) solo para ejecución de OT** (descargar OTs asignadas, ejecutar checklist y sincronizar) | Toda la app offline | Es el caso real del técnico en planta. La sincronización se limita a una cola de "ejecuciones" con resolución *last-write-wins* + log de conflictos. |
| ADR-11 | **.NET 10 LTS + Angular vigente**, standalone components | Quedarse en .NET 7 / Angular 17 | Soporte de seguridad. Se hace en la fase 1, antes de crecer el código. |
| ADR-12 | **Notificaciones con SignalR** dentro de maintenace-service (no un servicio nuevo) | notification-service + Consul/Eureka | Con 3 servicios no se justifica service discovery. Docker DNS alcanza. |

---

## 4. Plan de trabajo por fases

Estimaciones para **1 desarrollador**, en semanas.

### Fase 0 — Estabilizar (1–1,5 sem) 🔴 bloqueante
- [x] DAO y Logical: `async void` → `async Task` y `await` en toda la cadena (56 métodos).
- [x] `CreateEquipo`/`UpdateEquipo`: validar `Ubicacion` y persistirla con `DaoUbicacionEquipo` en la misma transacción (SP con `BEGIN TRAN`).
- [x] Unificar los valores de `TipoUbicacion` entre front y DB.
- [x] Corregir los nombres de SP de `DaoActividadEquipo` y `DaoGuiaEquipo`.
- [x] `Program.cs`: usar `builder.Configuration` (que respeta las env vars) y `ConnectionStrings:DefaultConnection`.
- [x] Sacar secretos a `.env` (con `.env.example` en el repo), rotar la clave JWT y la contraseña SA, y crear el login SQL `sync_app` con solo EXECUTE. *(login `sync_app` pendiente, ver sección 0)*
- [x] Middleware global de errores con respuestas `ProblemDetails` y códigos HTTP correctos (hoy un GET vacío devuelve 404).

### Fase 1 — Cimientos (2–3 sem)
- [x] **Migraciones:** crear `Backend/db-migrator/sql/` con `001__schemas.sql`, `010__tables_*.sql`, `100__sp_*.sql` (`CREATE OR ALTER`), `900__seed_*.sql`. Borrar los SP duplicados. Añadir Dockerfile y servicio one-shot en compose, del que dependen las apps.
- [x] Aplicar **ADR-03**: eliminar Bodega/Patio, añadir `Eliminado` donde falta, FK `Actividad.IdCuad`, quitar `Equipo.Seccion`.
- [x] **Migración a .NET 10**, limpiar paquetes, borrar Razor Pages, `wwwroot` y proyectos host fantasma. Renombrar `maintenace` → `maintenance`.
- [x] **Seguridad:** ADR-05 (filtro de tenant en el controlador base), ADR-06 (auth en gateway), políticas por rol (`Root`, `Admin`, `Supervisor`, `Tecnico`).
- [x] Gateway: completar las rutas (PATCH estado, GET por id, UbicacionEquipo, Valid, Turno, Horario, TipoArea) y borrar WeatherForecast.
- [x] Logging estructurado (JSON nativo; ver desviaciones), endpoints `/health` en todos los servicios y healthchecks en compose.
- [x] **CI en GitHub Actions:** build .NET + `ng build` + lint, y migraciones contra SQL Server en contenedor.
- [x] Tests: xUnit para Logical (con DAO mockeados) y tests de integración de SP con Testcontainers. *(cubierto con el smoke test sobre Docker)*

### Fase 2 — Completar los módulos existentes (2–3 sem)
- [x] Front: **Actividad** (lista y formulario), **Cuadrilla**, **Turno/Horario**, **Perfil**, **Registro**, Área funcional y Sección (reemplazan Bodega/Patio).
- [x] Back user-service: controladores **Turno** y **Horario** (los DAO ya existen).
- [x] Equipo: ficha completa con ubicación, guías asociadas (`Guia_Equipo`) e historial (fase 3).
- [x] Guía: editor de **Proced → Valid** (los pasos del checklist).
- [x] Upgrade de Angular a la versión vigente, migrar a standalone y unificar las carpetas `Services`/`services`.

### Fase 3 — Núcleo operativo de mantenimiento (4–5 sem) ⭐ base del predictivo
Tablas nuevas en el esquema `ops`:

| Tabla | Propósito | Campos clave |
|---|---|---|
| `PlanMantenimiento` | Programación preventiva por equipo | IdEquipo, IdGuia, TipoDisparador (`Calendario`/`Horometro`/`Ciclos`), Intervalo, UltimaEjecucion, ProximaFecha/ProximaLectura |
| `OrdenTrabajo` (OT) | Unidad de trabajo | Codigo, IdComp, IdEquipo, Tipo (`Preventiva`/`Correctiva`/`Predictiva`/`Inspeccion`), Prioridad, Estado (`Abierta→Asignada→EnEjecucion→Pausada→Cerrada→Cancelada`), IdCuad, IdPlan, IdPrediccion, FechaProgramada, Inicio, Fin, HorasHombre, Costo |
| `OT_Ejecucion` | Resultado por paso del checklist | IdOT, IdProced, IdValid, Resultado (`OK`/`NOK`/`NA`), ValorMedido, Observacion, IdUser, Fecha |
| `Falla` | Registro de fallas | IdEquipo, IdOT, IdModoFalla, FechaInicio, FechaFin, **ParadaProduccion (bit)**, Severidad, CausaRaiz |
| `ModoFalla` | Catálogo (estilo ISO 14224) | Codigo, Nombre, Componente, Mecanismo |
| `Medidor` / `Lectura` | Horómetro, odómetro, temperatura, vibración, presión… | Medidor(IdEquipo, Tipo, Unidad, LimiteMin/Max); Lectura(IdMedidor, Valor, Fecha, Origen `Manual`/`IoT`) |
| `Repuesto` / `OT_Repuesto` | Consumo de repuestos (base de costos e inventario) | IdOT, IdRepuesto, Cantidad, CostoUnit |
| `Adjunto` | Fotos y evidencias | IdOT, Url, Tipo |

Funcionalidad:
- [ ] Job (`BackgroundService` / Hangfire) que **genera OTs preventivas** desde `PlanMantenimiento` (por calendario o por umbral de horómetro).
- [ ] Flujo de OT con máquina de estados, asignación a cuadrilla y cierre con checklist obligatorio.
- [ ] Registro de **fallas** en las OT correctivas (modo de falla obligatorio al cerrar: **es la etiqueta del modelo**).
- [ ] **Captura de lecturas** de medidores (manual en el formulario, con API lista para IoT).
- [ ] **Dashboard de KPIs:** MTBF, MTTR, disponibilidad, % cumplimiento preventivo, backlog y costo por equipo.
- [ ] Notificaciones con SignalR (OT asignada, OT vencida, lectura fuera de rango).

### Fase 4 — PWA y offline para técnicos (2–3 sem)
- [ ] `@angular/pwa` (service worker y manifest) y almacenamiento en IndexedDB (Dexie).
- [ ] "Mis OT del turno": descarga, ejecución offline del checklist con fotos, cola de sincronización e indicador de estado.
- [ ] **Lectura de código de barras/QR** (cámara con `BarcodeDetector` o ZXing) para abrir la ficha o la OT del equipo. Generar e imprimir el QR por equipo.
- [ ] Endpoint `POST /sync/ejecuciones` idempotente (`ClientId` + `UpdatedAt`).

### Fase 5 — Mantenimiento predictivo (ver sección 5) (5–8 sem, en paralelo con la recolección de datos)

### Fase 6 — Producción (1–2 sem)
- [ ] Reverse proxy con HTTPS, contenedor del front (nginx), backups automáticos de SQL Server, ambientes `dev`/`staging`/`prod`.
- [ ] Observabilidad: OpenTelemetry → Grafana/Seq, alertas.
- [ ] Documentación: `docs/architecture.md`, OpenAPI publicada, README alineado con la realidad.

**Total estimado: 18–26 semanas.** La ruta crítica es Fase 0 → 1 → 3, porque el predictivo depende del historial de la Fase 3.

---

## 5. Plan de implementación — Modelo predictivo de mantenimiento

### 5.1 Objetivo y preguntas que responde
1. **¿Qué equipos tienen mayor probabilidad de fallar en los próximos N días?** (N = 7/30). Es una clasificación binaria con ventana.
2. **¿Cuánta vida útil le queda (RUL) a un equipo o componente?** Es regresión o análisis de supervivencia.
3. **¿Qué lecturas se están comportando de forma anómala?** Detección de anomalías, sin etiquetas.
4. **Acción:** recomendar o adelantar una OT predictiva y priorizar el backlog por riesgo × criticidad.

### 5.2 Requisito de datos (realidad del proyecto)
Hoy **no hay ningún dato** de fallas, ejecuciones ni lecturas. El modelo depende de la Fase 3. Mínimos orientativos para cada escalón:

| Escalón | Datos mínimos | Cuándo se puede activar |
|---|---|---|
| E1 Reglas + estadística | Plan preventivo + lecturas con límites + algunas fallas | Desde el día 1 de la Fase 3 |
| E2 ML supervisado | ~6–12 meses de historial y **≥ 30–50 fallas etiquetadas** por familia de equipo | Tras acumular historial (o con importación de historial previo en Excel/CMMS) |
| E3 RUL con sensores | Series temporales frecuentes (vibración/temperatura) vía IoT + fallas | Cuando haya sensores conectados |

👉 **Acción temprana:** crear un **importador CSV/Excel** de historial de OTs y fallas que el cliente ya tenga. Es la forma más rápida de llegar a E2.

### 5.3 Arquitectura

```
                 ┌───────────────┐
 Angular PWA ───▶│  api-gateway  │──▶ user-service
                 │   (Ocelot)    │──▶ maintenance-service ──▶ SQL Server (DB_SYNC)
                 └──────┬────────┘                              ▲   ▲
                        │ /apigateway/pdm/*                    │   │ lee vistas pdm.vw_*
                        ▼                                      │   │ escribe pdm.Prediccion
                 ┌──────────────────────┐                      │   │
                 │ predictive-service   │──────────────────────┘   │
                 │ FastAPI + LightGBM   │   job nocturno (APScheduler / cron)
                 │ lifelines, sklearn   │──▶ /models (volumen: artefactos .joblib)
                 │ MLflow (opcional)    │
                 └──────────────────────┘
 IoT / PLC (futuro) ──MQTT──▶ ingestor ──▶ ops.Lectura
```

- **Lectura** desde vistas SQL de solo lectura (`pdm.vw_FeaturesEquipoDia`) para no acoplarse a las tablas.
- **Escritura** solo en el esquema `pdm`, con un login SQL propio (`sync_pdm`: SELECT en `pdm.vw_*`, INSERT/UPDATE en `pdm.*`).
- **Endpoints:** `GET /pdm/riesgo?idComp=` (ranking), `GET /pdm/equipo/{id}` (historial de predicciones y explicación), `POST /pdm/recalcular/{idEquipo}`, `POST /pdm/entrenar` (solo Root), `GET /pdm/modelos`.

### 5.4 Tablas del esquema `pdm`

| Tabla | Campos |
|---|---|
| `ModeloVersion` | Id, Nombre, Tipo (`Reglas`/`LGBM`/`Weibull`/`IsolationForest`), FamiliaEquipo, FechaEntreno, Metricas (JSON), RutaArtefacto, Activo |
| `Prediccion` | Id, IdEquipo, IdModeloVersion, Fecha, Horizonte (días), ProbFalla, RUL_Dias, NivelRiesgo (`Bajo`/`Medio`/`Alto`/`Critico`), TopFactores (JSON, SHAP), IdOTGenerada |
| `Anomalia` | Id, IdMedidor, Fecha, Valor, Score, Regla/Modelo, Atendida |
| `Criticidad` | IdEquipo, Impacto en producción (1–5), Seguridad (1–5), Costo (1–5) → índice para priorizar |
| `FeedbackPrediccion` | IdPrediccion, ¿Hubo falla real? (bit), Comentario del técnico → reentrenamiento |

Además falta un dato en `Equipo`: **`FamiliaEquipo`/`Tipo`** (bomba, motor, compresor…). Es indispensable para entrenar por familia.

### 5.5 Features (por equipo y por día)

| Grupo | Ejemplos |
|---|---|
| Edad/uso | Edad (`AnioFabricacion`), horas acumuladas (horómetro), horas desde la última OT preventiva o correctiva |
| Historial de fallas | Nº de fallas en 30/90/365 días, días desde la última falla, MTBF propio, modos de falla recurrentes |
| Cumplimiento | % de preventivas a tiempo, días de atraso de la próxima preventiva, Nº de pasos `NOK` en los últimos checklists |
| Lecturas | Media, máx, desviación, pendiente (tendencia) en ventanas de 7 y 30 días; % de tiempo fuera de límite |
| Contexto | Familia, marca/modelo, potencia, capacidad, área/planta, turno o cuadrilla que lo opera |

**Etiqueta (E2):** `falla_en_N_dias = 1` si existe un `ops.Falla` en `(fecha, fecha+N]`.

### 5.6 Escalones del modelo

**E1 — Reglas y estadística (semanas 1–2 de la Fase 5)**
- Alertas por umbral y por tendencia en lecturas (pendiente de regresión lineal que cruza el límite en < X días).
- **Weibull** por familia (con `lifelines`): estima la probabilidad de falla dado el tiempo desde la última falla, aunque haya pocas fallas.
- Índice de riesgo = f(atraso del preventivo, NOK recientes, Weibull) × criticidad.
- Detección de anomalías: z-score robusto (MAD) o IsolationForest sobre las lecturas.

**E2 — ML supervisado (semanas 3–6)**
- **LightGBM** de clasificación por familia (o global con familia como categoría), horizontes de 7 y 30 días.
- **Validación temporal** (rolling-origin / `TimeSeriesSplit`). Nunca un split aleatorio, porque filtra el futuro al entrenamiento.
- Desbalance: `scale_pos_weight` y calibración (isotónica) para que la probabilidad sea interpretable.
- Métricas: **PR-AUC**, recall@top-k (¿cuántas fallas reales estaban en el 10 % de equipos con más riesgo?) y Brier score. Baseline: el modelo E1. Solo se promueve a producción si lo supera.
- **Explicabilidad:** SHAP top-3 factores por predicción, que se muestran en el front ("riesgo alto por: 12 días de atraso en el preventivo, vibración en tendencia ↑, 3 NOK recientes").
- Alternativa de supervivencia: Cox / Random Survival Forest (`scikit-survival`) para obtener RUL.

**E3 — RUL con sensores (cuando haya IoT)**
- Ingesta MQTT → `ops.Lectura` (o una tabla de series temporales con particionado).
- Features espectrales de vibración y modelos de degradación. Se evalúa cuando existan datos.

### 5.7 MLOps ligero
- **Entrenamiento** mensual (o manual) → registro en `pdm.ModeloVersion` y artefacto `.joblib` versionado (MLflow opcional).
- **Inferencia** nocturna para todos los equipos activos → `pdm.Prediccion`.
- **Monitoreo:** drift de features (PSI) y precisión real usando `FeedbackPrediccion` y las fallas que efectivamente ocurrieron.
- **Cierre del ciclo:** riesgo `Alto/Critico` → crear una OT `Predictiva` sugerida (estado `Propuesta`, la aprueba el supervisor). El resultado de esa OT etiqueta la predicción.
- **Tests:** pruebas unitarias de las features (casos con fechas límite), un test de "no fuga temporal" y un snapshot de métricas mínimo en CI.

### 5.8 Front — módulo "Predictivo"
- Mapa de calor de riesgo por planta y área, y ranking de equipos (probabilidad × criticidad).
- Ficha del equipo: curva de riesgo histórica, top factores, lecturas con bandas de límite, botón "crear OT predictiva".
- Bandeja de anomalías para atender o descartar (el descarte también sirve de feedback).

### 5.9 Cronograma del predictivo

| Semana | Entregable |
|---|---|
| 1 | Esquema `pdm`, vistas de features, scaffold de `predictive-service` (FastAPI, Docker, compose, ruta en el gateway) |
| 2 | E1: reglas de lecturas, Weibull por familia, índice de riesgo, job nocturno, endpoint de ranking |
| 3 | Importador de historial CSV/Excel y dataset de entrenamiento con etiquetas por ventana |
| 4–5 | E2: LightGBM, validación temporal, calibración, SHAP, comparación contra E1 |
| 6 | Front: dashboard de riesgo, ficha del equipo, OT predictiva sugerida |
| 7–8 | Monitoreo, feedback, reentrenamiento programado y documentación (model card) |

---

## 6. Próximos pasos inmediatos (esta semana)
1. Crear la rama `fix/fase-0` y corregir `async void`, `Ubicacion` y los nombres de SP (hallazgos 1–5).
2. Rotar secretos y crear `.env`.
3. Decidir y confirmar **ADR-03** (Bodega/Patio como TipoArea), porque condiciona front, back y BD.
4. Poblar `db-migrator/sql/` y levantar todo con `docker compose up` desde cero, para verificar que el sistema arranca limpio.
5. Preguntar al cliente o usuario si existe **historial de mantenimiento previo** (Excel o CMMS anterior). Define cuándo se puede activar el escalón E2.

# SYNC — Sistema de Información y Coordinación de Mantenimiento

Gestión de mantenimiento de equipos industriales: compañías y plantas, ubicación de activos,
guías de mantenimiento con checklist, actividades programadas, cuadrillas, turnos y usuarios.
Multi-compañía: cada usuario solo ve los datos de su compañía (salvo el rol Root).

> Estado y hoja de ruta: [docs/PLAN_TRABAJO.md](docs/PLAN_TRABAJO.md)

## Arquitectura

```
Navegador ──► frontend (Angular 22, nginx :4200)
                 │  /apigateway/*
                 ▼
             api-gateway (Ocelot, valida JWT, :7021)
              ├──► user-service         (auth, usuarios, credenciales, cuadrillas, turnos, horarios)
              └──► maintenance-service  (compañías, plantas, áreas, equipos, guías, actividades)
                         │
                         ▼
                   SQL Server (DB_SYNC)  ◄── db-migrator (DbUp: migraciones + procedimientos)
```

| Carpeta | Contenido |
|---|---|
| `Backend/shared/Sync.Common` | Base común: acceso SQL, CRUD genérico con aislamiento por compañía, errores ProblemDetails, JWT, health checks |
| `Backend/user-service` · `Backend/maintenance-service` | Microservicios .NET 10 (capas Entity → Data → Services → Controllers) |
| `Backend/api-gateway` | Ocelot; rutas en `ocelot.json` |
| `Backend/db-migrator` | Migraciones DbUp (`sql/migrations`, una vez) y procedimientos (`sql/procedures`, siempre) |
| `Backend/tests` | Pruebas unitarias (xUnit) y smoke test end-to-end (`tests/smoke/smoke_test.py`) |
| `Frontend` | Angular 22 standalone + signals, sin zone.js |
| `Docker` | `docker-compose.yml` y `.env.example` |

## Puesta en marcha (Docker)

Requisitos: Docker Desktop.

```bash
cd Docker
cp .env.example .env      # edite contraseñas y JWT_SECRET
docker compose up -d --build
```

- Aplicación: http://localhost:4200 — usuario inicial `SEED_ROOT_USER` / `SEED_ROOT_PASSWORD` del `.env`
- API (gateway): http://localhost:7021/apigateway/...
- El migrador crea la base, aplica migraciones y crea el usuario Root solo si no existen credenciales.

## Desarrollo local

| Herramienta | Versión |
|---|---|
| .NET SDK | 10.0 |
| Node.js | ≥ 22.22 o 24.x (requisito de Angular CLI 22) |
| Docker | para SQL Server y el stack completo |

```bash
# Backend: compilar y probar
dotnet build Backend/Sync.sln
dotnet test Backend/Sync.sln

# Migrar una base local
dotnet run --project Backend/db-migrator -- --connection "Data Source=localhost,1433;Initial Catalog=DB_SYNC;User ID=sa;Password=...;TrustServerCertificate=True"

# Frontend (con el gateway corriendo en :7021; ng serve hace proxy de /apigateway)
cd Frontend && npm ci && npm start
npm test && npm run lint
```

Los servicios leen su configuración de `appsettings.json` y variables de entorno:
`ConnectionStrings__DefaultConnection`, `Jwt__SecretKey` (≥ 32 caracteres), `Jwt__Issuer`, `Jwt__Audience`, `Cors__Origins`.
Los secretos nunca van en el repositorio.

## Roles

| Rol | Puede |
|---|---|
| Root | Todo, en todas las compañías; crear compañías y usuarios Root; catálogo global de tipos de área |
| Admin | Gestionar su compañía: plantas, equipos, guías, usuarios, credenciales, catálogos |
| Supervisor | Programar actividades, gestionar cuadrillas y horarios; consultar el resto |
| Tecnico | Consultar equipos, guías y actividades; su perfil |

## API

Todos los recursos siguen el mismo contrato: `GET /` (filtros por query), `GET /{id}`, `POST`, `PUT /{id}`,
`DELETE /{id}` (lógico) y `PATCH /{id}/estado`. Errores en formato RFC 7807 (`title`, `status`).
OpenAPI de cada servicio en `/openapi/v1.json` (entorno Development).

## Licencia

MIT — Copyright (c) 2025 SYNC

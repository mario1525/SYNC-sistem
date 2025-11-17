# Database Migration Model (DbUp-based)

This module provides repeatable, idempotent SQL migrations using DbUp for both databases:
- user-service (DB_SYNC_USER)
- maintenace-service (DB_SYNC_MAINTENANCE)

It is designed to:
- Version SQL changes via ordered files (001__, 002__, ...)
- Record applied migrations in a journal table (dbo.DbUpJournal_{user|maintenance})
- Run in local dev (CLI) or as a container (Docker Compose/CI)


## Layout

Place your SQL files here, ordered by numeric prefix:

```
Backend/db-migrator/
├─ DbMigrator.csproj
├─ Program.cs
└─ sql/
   ├─ user/
   │  ├─ 001__baseline.sql
   │  ├─ 010__procedures__users.sql
   │  └─ 020__seed__initial_users.sql
   └─ maintenance/
      ├─ 001__baseline.sql
      ├─ 010__procedures__comp_planta.sql
      └─ 020__seed__catalogs.sql
```

Naming convention:
- 3-digit numeric prefix for ordering (001, 002, 010, ...)
- Double underscore
- A short, meaningful description
- Use multiple files by feature (schemas, tables, procs, seeds)


## Sourcing current scripts

You already have scripts in:
- user-service: [ScriptCreacion.sql](Backend/user-service/database/Script/ScriptCreacion.sql:1) and stored procedures under [database/Script/](Backend/user-service/database/Script/ScriptCreacion.sql)
- maintenace-service: [script.sql](Backend/maintenace-service/database/Script/script.sql:1) and stored procedures under [database/Script/](Backend/maintenace-service/database/Script/script.sql)

Recommended split (example):
- Copy/create into Backend/db-migrator/sql/user
  - 001__baseline.sql — schema + tables only (from ScriptCreacion.sql without test data)
  - 010__procs__users.sql — all SPs (SP_Users.sql, SP_User_Cred.sql, SP_Cuad.sql, SP_Horario.sql, SP_Turno.sql) concatenated with clear separators
  - 020__seed__users.sql — optional seed data
- Copy/create into Backend/db-migrator/sql/maintenance
  - 001__baseline.sql — schema + tables only (from script.sql without test data)
  - 010__procs__comp_planta_equipo.sql — all SPs by topic (SP_Comp.sql, SP_Planta.sql, SP_Equipo.sql, etc.)
  - 020__seed__catalogs.sql — seed catalogs if needed

Important:
- Each file should be rerunnable safely if possible (CREATE OR ALTER for procs/views; IF NOT EXISTS checks for tables/indexes)
- Separate DDL from data where practical


## Running locally (CLI)

Using default connection strings (Docker network names) or env variables.

Examples:
- User DB:
  ```
  dotnet run --project ./Backend/db-migrator/DbMigrator.csproj -- --db user --connection "Data Source=localhost,1433;Initial Catalog=DB_SYNC_USER;User ID=SA;Password=*****;TrustServerCertificate=True"
  ```
- Maintenance DB:
  ```
  dotnet run --project ./Backend/db-migrator/DbMigrator.csproj -- --db maintenance --connection "Data Source=localhost,1433;Initial Catalog=DB_SYNC_MAINTENANCE;User ID=SA;Password=*****;TrustServerCertificate=True"
  ```

Environment variables (alternative):
- DB_CONNECTION="your-connection-string"
- DB_CHOICE="user" | "maintenance"

Scripts directory override:
- Use `--scripts` to point to a different set of files (absolute or relative)
  ```
  dotnet run --project ./Backend/db-migrator/DbMigrator.csproj -- --db user --scripts ./Backend/db-migrator/sql/user
  ```


## Dockerization (suggested)

Create a Dockerfile at [Dockerfile](Backend/db-migrator/Dockerfile) with:

```
# build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish DbMigrator.csproj -c Release -o /app/publish

# run
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# copy SQL
COPY ./sql ./sql
ENTRYPOINT ["dotnet", "DbMigrator.dll"]
```

Add one-shot services to [docker-compose.yml](Docker/docker-compose.yml:1) (example):

```
  db-migrator-user:
    build:
      context: ../Backend/db-migrator
    environment:
      - DB_CHOICE=user
      - DB_CONNECTION=Data Source=user-sqlserver,1433;Initial Catalog=DB_SYNC_USER;Persist Security Info=True;TrustServerCertificate=True;User ID=SA;Password=logPaSs#123$32
    depends_on:
      - user-sqlserver
    command: ["dotnet", "DbMigrator.dll", "--db", "user"]

  db-migrator-maintenance:
    build:
      context: ../Backend/db-migrator
    environment:
      - DB_CHOICE=maintenance
      - DB_CONNECTION=Data Source=user-sqlserver,1433;Initial Catalog=DB_SYNC_MAINTENANCE;Persist Security Info=True;TrustServerCertificate=True;User ID=SA;Password=logPaSs#123$32
    depends_on:
      - user-sqlserver
    command: ["dotnet", "DbMigrator.dll", "--db", "maintenance"]
```

Execution order ideas:
- Run migrators once (manual) before bringing up the app
- Or as init-jobs that run, exit 0, and app services depend_on them


## CI/CD integration (example)

- Step 1: Build migrator
  ```
  dotnet publish ./Backend/db-migrator/DbMigrator.csproj -c Release -o ./out/db-migrator
  ```
- Step 2: Run migrations per env
  ```
  dotnet ./out/db-migrator/DbMigrator.dll --db user --connection "%DB_CONNECTION_USER%"
  dotnet ./out/db-migrator/DbMigrator.dll --db maintenance --connection "%DB_CONNECTION_MAINT%"
  ```
- Step 3: Build and deploy services


## Best practices

- Use CREATE OR ALTER for programmable objects (SPs, views, functions)
- Guard DDL with IF NOT EXISTS checks for tables/indexes/constraints
- Keep each script idempotent when possible
- Never modify applied scripts; create a new higher-numbered script
- Keep a strict numeric ordering across the team
- Test migrations on a disposable DB before running on shared environments


## First steps checklist

1) Move/split existing scripts:
   - User DB:
     - [ScriptCreacion.sql](Backend/user-service/database/Script/ScriptCreacion.sql:1) → 001__baseline.sql
     - All "SP_*" into 010__procedures__*.sql
   - Maintenance DB:
     - [script.sql](Backend/maintenace-service/database/Script/script.sql:1) → 001__baseline.sql
     - All "SP_*" into 010__procedures__*.sql

2) Run locally with your connection string and verify:
   ```
   dotnet run --project ./Backend/db-migrator/DbMigrator.csproj -- --db user --scripts ./Backend/db-migrator/sql/user
   dotnet run --project ./Backend/db-migrator/DbMigrator.csproj -- --db maintenance --scripts ./Backend/db-migrator/sql/maintenance
   ```

3) Wire into Docker (optional but recommended) and add to CI.


## Notes

- The migrator automatically creates the database if it does not exist (EnsureDatabase).
- Journal table per DB choice: dbo.DbUpJournal_user, dbo.DbUpJournal_maintenance.
- You can safely re-run the migrator; DbUp will only apply new scripts that have not yet been recorded in the journal.
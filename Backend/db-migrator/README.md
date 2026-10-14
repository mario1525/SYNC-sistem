# SYNC DbMigrator

Migrador de base de datos basado en DbUp. Una sola base: `DB_SYNC`.

```
sql/
├─ migrations/   001__baseline.sql, 002__..., 003__...   → se ejecutan UNA vez (journal dbo.SchemaVersions)
└─ procedures/   099__obsoletos.sql, 100__..., 160__...  → se ejecutan SIEMPRE (CREATE OR ALTER)
```

## Reglas

- **Migraciones**: nunca modifique un script ya aplicado; cree uno nuevo con número mayor
  (`004__descripcion.sql`). Escríbalos idempotentes (`IF NOT EXISTS`, `COL_LENGTH`...).
- **Procedimientos**: un archivo por dominio; se editan libremente porque se recrean en cada despliegue.
  Convención: `dbo.db_Sp_<Entidad>_<Get|Set|Del|Active>`; `Get` acepta `@IdComp` y filtra `Eliminado = 0`;
  `Del` es borrado lógico. Errores de negocio con `THROW 50404|50409|504xx` (la API los traduce a 404/409/400).

## Uso

```bash
dotnet run --project Backend/db-migrator -- --connection "<cadena>"
```

Variables de entorno equivalentes: `DB_CONNECTION`, `SQL_SCRIPTS` (carpeta de scripts).

Seed del usuario Root (solo si no existe ninguna credencial):
`SEED_ROOT_USER`, `SEED_ROOT_PASSWORD`, `SEED_ROOT_COMP`.

En Docker Compose corre como servicio `db-migrator` antes de los microservicios
(ellos esperan a que termine con éxito).

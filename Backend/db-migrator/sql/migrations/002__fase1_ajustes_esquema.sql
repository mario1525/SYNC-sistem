-- ===================================================================================
-- Fase 1 - Ajustes de esquema (idempotente)
--  * Soft-delete uniforme (columna Eliminado en todas las tablas)
--  * Esp por compañía (IdComp NULL = catálogo global)
--  * FKs opcionales como NULL (antes '' rompía las FK al insertar)
--  * FK Actividad.IdCuad -> Cuad
--  * Equipo.Seccion eliminado (la ubicación vive en UbicacionEquipo)
--  * Índices de rendimiento y unicidad
-- ===================================================================================

-- Utilidad temporal: elimina el DEFAULT de una columna (necesario antes de ALTER COLUMN)
CREATE OR ALTER PROCEDURE dbo.tmp_DropDefault @Table SYSNAME, @Column SYSNAME
AS
BEGIN
    DECLARE @name SYSNAME, @sql NVARCHAR(400);
    SELECT @name = dc.name
    FROM sys.default_constraints dc
    JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
    WHERE dc.parent_object_id = OBJECT_ID(@Table) AND c.name = @Column;
    IF @name IS NOT NULL
    BEGIN
        SET @sql = N'ALTER TABLE ' + @Table + N' DROP CONSTRAINT ' + QUOTENAME(@name);
        EXEC sp_executesql @sql;
    END
END
GO

------------------------------------------------------------
-- 1. Columna Eliminado donde faltaba
------------------------------------------------------------
IF COL_LENGTH('dbo.Planta', 'Eliminado') IS NULL
    ALTER TABLE dbo.Planta ADD Eliminado BIT NOT NULL CONSTRAINT DF_Planta_Eliminado DEFAULT 0;
IF COL_LENGTH('dbo.TipoArea', 'Eliminado') IS NULL
    ALTER TABLE dbo.TipoArea ADD Eliminado BIT NOT NULL CONSTRAINT DF_TipoArea_Eliminado DEFAULT 0;
IF COL_LENGTH('dbo.Area', 'Eliminado') IS NULL
    ALTER TABLE dbo.Area ADD Eliminado BIT NOT NULL CONSTRAINT DF_Area_Eliminado DEFAULT 0;
IF COL_LENGTH('dbo.SeccionArea', 'Eliminado') IS NULL
    ALTER TABLE dbo.SeccionArea ADD Eliminado BIT NOT NULL CONSTRAINT DF_SeccionArea_Eliminado DEFAULT 0;
IF COL_LENGTH('dbo.UbicacionEquipo', 'Eliminado') IS NULL
    ALTER TABLE dbo.UbicacionEquipo ADD Eliminado BIT NOT NULL CONSTRAINT DF_UbicacionEquipo_Eliminado DEFAULT 0;
IF COL_LENGTH('dbo.Turno', 'Estado') IS NULL
    ALTER TABLE dbo.Turno ADD Estado BIT NOT NULL CONSTRAINT DF_Turno_Estado DEFAULT 1;
GO

------------------------------------------------------------
-- 2. Especialidades por compañía
------------------------------------------------------------
IF COL_LENGTH('dbo.Esp', 'IdComp') IS NULL
    ALTER TABLE dbo.Esp ADD IdComp VARCHAR(36) NULL;
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FKEsp_Comp')
    ALTER TABLE dbo.Esp ADD CONSTRAINT FKEsp_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id);
GO

------------------------------------------------------------
-- 3. FKs opcionales: '' -> NULL
------------------------------------------------------------
-- Users.IdCuad / Users.IdEsp
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'IdCuad' AND is_nullable = 0)
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FKUsers_Cuad') ALTER TABLE dbo.Users DROP CONSTRAINT FKUsers_Cuad;
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FKUsers_Esp')  ALTER TABLE dbo.Users DROP CONSTRAINT FKUsers_Esp;
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_IdCuad') DROP INDEX IX_Users_IdCuad ON dbo.Users;
    EXEC dbo.tmp_DropDefault 'dbo.Users', 'IdCuad';
    EXEC dbo.tmp_DropDefault 'dbo.Users', 'IdEsp';
    ALTER TABLE dbo.Users ALTER COLUMN IdCuad VARCHAR(36) NULL;
    ALTER TABLE dbo.Users ALTER COLUMN IdEsp VARCHAR(36) NULL;
END
GO
UPDATE dbo.Users SET IdCuad = NULL WHERE IdCuad = '';
UPDATE dbo.Users SET IdEsp = NULL WHERE IdEsp = '';
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FKUsers_Cuad')
    ALTER TABLE dbo.Users ADD CONSTRAINT FKUsers_Cuad FOREIGN KEY (IdCuad) REFERENCES dbo.Cuad(Id);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FKUsers_Esp')
    ALTER TABLE dbo.Users ADD CONSTRAINT FKUsers_Esp FOREIGN KEY (IdEsp) REFERENCES dbo.Esp(Id);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_IdCuad')
    CREATE INDEX IX_Users_IdCuad ON dbo.Users(IdCuad);
GO

-- Guia.IdEsp
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Guia') AND name = 'IdEsp' AND is_nullable = 0)
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FKGuia_Esp') ALTER TABLE dbo.Guia DROP CONSTRAINT FKGuia_Esp;
    EXEC dbo.tmp_DropDefault 'dbo.Guia', 'IdEsp';
    ALTER TABLE dbo.Guia ALTER COLUMN IdEsp VARCHAR(36) NULL;
END
GO
UPDATE dbo.Guia SET IdEsp = NULL WHERE IdEsp = '';
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FKGuia_Esp')
    ALTER TABLE dbo.Guia ADD CONSTRAINT FKGuia_Esp FOREIGN KEY (IdEsp) REFERENCES dbo.Esp(Id);
GO

-- Actividad.IdCuad (+ FK nueva)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Actividad') AND name = 'IdCuad' AND is_nullable = 0)
BEGIN
    EXEC dbo.tmp_DropDefault 'dbo.Actividad', 'IdCuad';
    ALTER TABLE dbo.Actividad ALTER COLUMN IdCuad VARCHAR(36) NULL;
END
GO
UPDATE dbo.Actividad SET IdCuad = NULL WHERE IdCuad = '' OR IdCuad NOT IN (SELECT Id FROM dbo.Cuad);
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FKActividad_Cuad')
    ALTER TABLE dbo.Actividad ADD CONSTRAINT FKActividad_Cuad FOREIGN KEY (IdCuad) REFERENCES dbo.Cuad(Id);
GO

-- Actividad_Equipo.IdGuia
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Actividad_Equipo') AND name = 'IdGuia' AND is_nullable = 0)
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FKActividad_Equipo_Guia') ALTER TABLE dbo.Actividad_Equipo DROP CONSTRAINT FKActividad_Equipo_Guia;
    EXEC dbo.tmp_DropDefault 'dbo.Actividad_Equipo', 'IdGuia';
    ALTER TABLE dbo.Actividad_Equipo ALTER COLUMN IdGuia VARCHAR(36) NULL;
END
GO
UPDATE dbo.Actividad_Equipo SET IdGuia = NULL WHERE IdGuia = '';
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FKActividad_Equipo_Guia')
    ALTER TABLE dbo.Actividad_Equipo ADD CONSTRAINT FKActividad_Equipo_Guia FOREIGN KEY (IdGuia) REFERENCES dbo.Guia(Id);
GO

------------------------------------------------------------
-- 4. Equipo.Seccion -> se reemplaza por UbicacionEquipo
------------------------------------------------------------
IF COL_LENGTH('dbo.Equipo', 'Seccion') IS NOT NULL
BEGIN
    EXEC dbo.tmp_DropDefault 'dbo.Equipo', 'Seccion';
    ALTER TABLE dbo.Equipo DROP COLUMN Seccion;
END
GO

------------------------------------------------------------
-- 5. Unicidad e índices
------------------------------------------------------------
-- Solo una ubicación vigente por equipo: se conservan la más reciente
;WITH dup AS (
    SELECT Id, ROW_NUMBER() OVER (PARTITION BY IdEquipo ORDER BY Fecha_log DESC, Id) AS rn
    FROM dbo.UbicacionEquipo WHERE Eliminado = 0
)
UPDATE u SET Eliminado = 1 FROM dbo.UbicacionEquipo u JOIN dup ON dup.Id = u.Id WHERE dup.rn > 1;
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_UbicacionEquipo_Vigente')
    CREATE UNIQUE INDEX UX_UbicacionEquipo_Vigente ON dbo.UbicacionEquipo(IdEquipo) WHERE Eliminado = 0;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_User_Cred_Usuario')
    CREATE UNIQUE INDEX UX_User_Cred_Usuario ON dbo.User_Cred(Usuario) WHERE Eliminado = 0;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Planta_IdComp')
    CREATE INDEX IX_Planta_IdComp ON dbo.Planta(IdComp);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Area_IdPlanta')
    CREATE INDEX IX_Area_IdPlanta ON dbo.Area(IdPlanta);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SeccionArea_IdArea')
    CREATE INDEX IX_SeccionArea_IdArea ON dbo.SeccionArea(IdArea);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Equipo_IdComp')
    CREATE INDEX IX_Equipo_IdComp ON dbo.Equipo(IdComp);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Guia_IdComp')
    CREATE INDEX IX_Guia_IdComp ON dbo.Guia(IdComp);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Proced_IdGuia')
    CREATE INDEX IX_Proced_IdGuia ON dbo.Proced(IdGuia);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Valid_IdProced')
    CREATE INDEX IX_Valid_IdProced ON dbo.Valid(IdProced);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Guia_Equipo_IdEquipo')
    CREATE INDEX IX_Guia_Equipo_IdEquipo ON dbo.Guia_Equipo(IdEquipo);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Actividad_IdComp')
    CREATE INDEX IX_Actividad_IdComp ON dbo.Actividad(IdComp, FechaEjecucion);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Actividad_Equipo_IdActividad')
    CREATE INDEX IX_Actividad_Equipo_IdActividad ON dbo.Actividad_Equipo(IdActividad);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TipoActividad_IdComp')
    CREATE INDEX IX_TipoActividad_IdComp ON dbo.TipoActividad(IdComp);
GO

DROP PROCEDURE IF EXISTS dbo.tmp_DropDefault;
GO

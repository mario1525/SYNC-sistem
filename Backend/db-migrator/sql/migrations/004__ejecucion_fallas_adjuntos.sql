-- ===================================================================================
-- Ejecución en campo, reportes de falla y adjuntos (fotos)
--  * Valid.RequiereFoto: la guía puede exigir evidencia fotográfica en una verificación
--  * Actividad_Equipo: estado de ejecución (Pendiente → EnCurso → Completada)
--  * EjecucionValid: resultado de cada verificación del checklist
--  * Falla: reportes de falla hechos desde campo
--  * Adjunto: fotos de equipos, evidencias de ejecución y fotos de fallas
-- Fechas nuevas en DATETIME2 (ver pendiente de PLAN_TRABAJO: SMALLDATETIME tiene precisión de minuto).
-- ===================================================================================

------------------------------------------------------------
-- 1. Valid.RequiereFoto
------------------------------------------------------------
IF COL_LENGTH('dbo.Valid', 'RequiereFoto') IS NULL
    ALTER TABLE dbo.Valid ADD RequiereFoto BIT NOT NULL CONSTRAINT DF_Valid_RequiereFoto DEFAULT 0;
GO

------------------------------------------------------------
-- 2. Estado de ejecución de cada equipo de una actividad
------------------------------------------------------------
IF COL_LENGTH('dbo.Actividad_Equipo', 'EstadoEjec') IS NULL
    ALTER TABLE dbo.Actividad_Equipo ADD
        EstadoEjec  VARCHAR(20) NOT NULL CONSTRAINT DF_Actividad_Equipo_EstadoEjec DEFAULT 'Pendiente'
                    CONSTRAINT CK_Actividad_Equipo_EstadoEjec CHECK (EstadoEjec IN ('Pendiente', 'EnCurso', 'Completada')),
        FechaInicio DATETIME2(0) NULL,
        FechaFin    DATETIME2(0) NULL,
        IdUserEjec  VARCHAR(36) NULL,
        Observacion VARCHAR(MAX) NULL;
GO

------------------------------------------------------------
-- 3. Resultado de cada verificación
------------------------------------------------------------
IF OBJECT_ID('dbo.EjecucionValid', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.EjecucionValid(
        Id                VARCHAR(36)  NOT NULL PRIMARY KEY,
        IdActividadEquipo VARCHAR(36)  NOT NULL CONSTRAINT FKEjecucionValid_AE REFERENCES dbo.Actividad_Equipo(Id),
        IdValid           VARCHAR(36)  NOT NULL CONSTRAINT FKEjecucionValid_Valid REFERENCES dbo.Valid(Id),
        Cumple            BIT          NOT NULL,
        Observacion       VARCHAR(MAX) NULL,
        IdUser            VARCHAR(36)  NULL,
        Fecha             DATETIME2(0) NOT NULL CONSTRAINT DF_EjecucionValid_Fecha DEFAULT SYSDATETIME()
    );
    CREATE UNIQUE INDEX UX_EjecucionValid ON dbo.EjecucionValid(IdActividadEquipo, IdValid);
END
GO

------------------------------------------------------------
-- 4. Reportes de falla
------------------------------------------------------------
IF OBJECT_ID('dbo.Falla', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Falla(
        Id             VARCHAR(36)  NOT NULL PRIMARY KEY,
        IdComp         VARCHAR(36)  NOT NULL CONSTRAINT FKFalla_Comp REFERENCES dbo.Comp(Id),
        IdEquipo       VARCHAR(36)  NOT NULL CONSTRAINT FKFalla_Equipo REFERENCES dbo.Equipo(Id),
        Tipo           VARCHAR(30)  NOT NULL CONSTRAINT CK_Falla_Tipo
                       CHECK (Tipo IN ('Fuga', 'RuidoVibracion', 'Temperatura', 'Electrica', 'NoArranca', 'Otro')),
        Prioridad      VARCHAR(10)  NOT NULL CONSTRAINT CK_Falla_Prioridad
                       CHECK (Prioridad IN ('Baja', 'Media', 'Alta', 'Critica')),
        EquipoDetenido BIT          NOT NULL CONSTRAINT DF_Falla_Detenido DEFAULT 0,
        Descripcion    VARCHAR(MAX) NULL,
        EstadoFalla    VARCHAR(15)  NOT NULL CONSTRAINT DF_Falla_EstadoFalla DEFAULT 'Abierta'
                       CONSTRAINT CK_Falla_EstadoFalla CHECK (EstadoFalla IN ('Abierta', 'EnAtencion', 'Cerrada')),
        IdUserReporta  VARCHAR(36)  NULL,
        FechaReporte   DATETIME2(0) NOT NULL CONSTRAINT DF_Falla_FechaReporte DEFAULT SYSDATETIME(),
        FechaCierre    DATETIME2(0) NULL,
        Estado         BIT          NOT NULL CONSTRAINT DF_Falla_Estado DEFAULT 1,
        Eliminado      BIT          NOT NULL CONSTRAINT DF_Falla_Eliminado DEFAULT 0,
        Fecha_log      SMALLDATETIME NULL CONSTRAINT DF_Falla_Fecha_log DEFAULT CURRENT_TIMESTAMP
    );
    CREATE INDEX IX_Falla_IdComp ON dbo.Falla(IdComp, EstadoFalla);
    CREATE INDEX IX_Falla_IdEquipo ON dbo.Falla(IdEquipo);
END
GO

------------------------------------------------------------
-- 5. Adjuntos (los bytes se guardan en la BD: sin volúmenes adicionales)
------------------------------------------------------------
IF OBJECT_ID('dbo.Adjunto', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Adjunto(
        Id          VARCHAR(36)    NOT NULL PRIMARY KEY,
        IdComp      VARCHAR(36)    NOT NULL CONSTRAINT FKAdjunto_Comp REFERENCES dbo.Comp(Id),
        Entidad     VARCHAR(20)    NOT NULL CONSTRAINT CK_Adjunto_Entidad CHECK (Entidad IN ('Equipo', 'Ejecucion', 'Falla')),
        IdEntidad   VARCHAR(36)    NOT NULL,
        Ref         VARCHAR(36)    NULL,          -- Ejecucion: IdValid de la verificación evidenciada
        Nombre      VARCHAR(255)   NOT NULL,
        ContentType VARCHAR(100)   NOT NULL,
        Tamano      INT            NOT NULL,
        Datos       VARBINARY(MAX) NOT NULL,
        IdUser      VARCHAR(36)    NULL,
        Fecha       DATETIME2(0)   NOT NULL CONSTRAINT DF_Adjunto_Fecha DEFAULT SYSDATETIME(),
        Eliminado   BIT            NOT NULL CONSTRAINT DF_Adjunto_Eliminado DEFAULT 0
    );
    CREATE INDEX IX_Adjunto_Entidad ON dbo.Adjunto(Entidad, IdEntidad) WHERE Eliminado = 0;
END
GO

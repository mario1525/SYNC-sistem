-- ===================================================================================
-- Guía de mantenimiento -> Procedimientos (Proced) -> Validaciones (Valid)
-- Guia_Equipo: qué guías aplican a qué equipos.
-- ===================================================================================

CREATE OR ALTER PROCEDURE dbo.db_Sp_Guia_Get
    @Id     VARCHAR(36)  = NULL,
    @IdComp VARCHAR(36)  = NULL,
    @IdEsp  VARCHAR(36)  = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Estado BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT g.Id, g.Nombre, g.Descripcion, g.Proceso, g.Inspeccion, g.Herramientas, g.IdComp, g.IdEsp,
           es.Nombre AS EspNombre, g.SeguridadInd, g.SeguridadAmb, g.Intervalo, g.Importante, g.Insumos,
           g.Personal, g.Duracion, g.Logistica, g.Situacion, g.Notas, g.CreatedBy, g.UpdatedBy,
           g.FechaUpdate, g.Estado, g.Fecha_log
    FROM dbo.Guia g
    LEFT JOIN dbo.Esp es ON es.Id = g.IdEsp
    WHERE g.Eliminado = 0
      AND (@Id IS NULL OR g.Id = @Id)
      AND (@IdComp IS NULL OR g.IdComp = @IdComp)
      AND (@IdEsp IS NULL OR g.IdEsp = @IdEsp)
      AND (@Nombre IS NULL OR g.Nombre LIKE '%' + @Nombre + '%')
      AND (@Estado IS NULL OR g.Estado = @Estado)
    ORDER BY g.Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Guia_Set
    @Id           VARCHAR(36),
    @Nombre       VARCHAR(255),
    @Descripcion  VARCHAR(MAX),
    @Proceso      VARCHAR(MAX),
    @Inspeccion   VARCHAR(MAX),
    @Herramientas VARCHAR(MAX),
    @IdComp       VARCHAR(36),
    @IdEsp        VARCHAR(36) = NULL,
    @SeguridadInd VARCHAR(MAX),
    @SeguridadAmb VARCHAR(MAX),
    @Intervalo    INT,
    @Importante   VARCHAR(MAX),
    @Insumos      VARCHAR(MAX),
    @Personal     INT,
    @Duracion     INT,
    @Logistica    VARCHAR(MAX),
    @Situacion    VARCHAR(255),
    @Notas        VARCHAR(MAX),
    @UpdatedBy    VARCHAR(36),
    @Estado       BIT,
    @ProcedJson   NVARCHAR(MAX) = NULL,  -- [{id,nombre,descripcion,valid:[{id,nombre,descripcion}]}]; NULL = no tocar pasos
    @Operacion    CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRAN;

    IF @Operacion = 'I'
        INSERT INTO dbo.Guia (Id, Nombre, Descripcion, Proceso, Inspeccion, Herramientas, IdComp, IdEsp,
            SeguridadInd, SeguridadAmb, Intervalo, Importante, Insumos, Personal, Duracion, Logistica,
            Situacion, Notas, CreatedBy, UpdatedBy, Estado, Eliminado)
        VALUES (@Id, @Nombre, ISNULL(@Descripcion, ''), ISNULL(@Proceso, ''), ISNULL(@Inspeccion, ''),
            ISNULL(@Herramientas, ''), @IdComp, @IdEsp, ISNULL(@SeguridadInd, ''), ISNULL(@SeguridadAmb, ''),
            @Intervalo, ISNULL(@Importante, ''), ISNULL(@Insumos, ''), @Personal, @Duracion,
            ISNULL(@Logistica, ''), ISNULL(@Situacion, ''), ISNULL(@Notas, ''), @UpdatedBy, @UpdatedBy, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Guia
        SET Nombre = @Nombre, Descripcion = ISNULL(@Descripcion, ''), Proceso = ISNULL(@Proceso, ''),
            Inspeccion = ISNULL(@Inspeccion, ''), Herramientas = ISNULL(@Herramientas, ''), IdEsp = @IdEsp,
            SeguridadInd = ISNULL(@SeguridadInd, ''), SeguridadAmb = ISNULL(@SeguridadAmb, ''),
            Intervalo = @Intervalo, Importante = ISNULL(@Importante, ''), Insumos = ISNULL(@Insumos, ''),
            Personal = @Personal, Duracion = @Duracion, Logistica = ISNULL(@Logistica, ''),
            Situacion = ISNULL(@Situacion, ''), Notas = ISNULL(@Notas, ''), UpdatedBy = @UpdatedBy,
            Estado = @Estado, FechaUpdate = GETDATE()
        WHERE Id = @Id AND IdComp = @IdComp AND Eliminado = 0;

    -- Sincroniza el árbol Proced -> Valid con lo recibido (los Id vienen generados desde la API)
    IF @ProcedJson IS NOT NULL
    BEGIN
        DECLARE @p TABLE (Id VARCHAR(36), Nombre VARCHAR(255), Descripcion VARCHAR(MAX), Orden INT);
        DECLARE @v TABLE (Id VARCHAR(36), IdProced VARCHAR(36), Nombre VARCHAR(255), Descripcion VARCHAR(MAX), RequiereFoto BIT);

        INSERT INTO @p (Id, Nombre, Descripcion, Orden)
        SELECT j.Id, j.Nombre, ISNULL(j.Descripcion, ''), CAST(k.[key] AS INT)
        FROM OPENJSON(@ProcedJson) k
        CROSS APPLY OPENJSON(k.value) WITH (Id VARCHAR(36) '$.id', Nombre VARCHAR(255) '$.nombre', Descripcion VARCHAR(MAX) '$.descripcion') j;

        INSERT INTO @v (Id, IdProced, Nombre, Descripcion, RequiereFoto)
        SELECT v.Id, JSON_VALUE(k.value, '$.id'), v.Nombre, ISNULL(v.Descripcion, ''), ISNULL(v.RequiereFoto, 0)
        FROM OPENJSON(@ProcedJson) k
        CROSS APPLY OPENJSON(k.value, '$.valid') WITH (Id VARCHAR(36) '$.id', Nombre VARCHAR(255) '$.nombre',
            Descripcion VARCHAR(MAX) '$.descripcion', RequiereFoto BIT '$.requiereFoto') v;

        -- Bajas: lo que ya no viene en el JSON
        UPDATE dbo.Valid SET Eliminado = 1
        WHERE Eliminado = 0 AND IdProced IN (SELECT Id FROM dbo.Proced WHERE IdGuia = @Id)
          AND Id NOT IN (SELECT Id FROM @v);
        UPDATE dbo.Proced SET Eliminado = 1
        WHERE Eliminado = 0 AND IdGuia = @Id AND Id NOT IN (SELECT Id FROM @p);

        -- Altas / cambios de Proced (Fecha_log se usa como orden de presentación)
        UPDATE pr SET Nombre = p.Nombre, Descripcion = p.Descripcion, Fecha_log = DATEADD(MINUTE, p.Orden, '2000-01-01')
        FROM dbo.Proced pr JOIN @p p ON p.Id = pr.Id WHERE pr.IdGuia = @Id;
        INSERT INTO dbo.Proced (Id, IdGuia, Nombre, Descripcion, Estado, Eliminado, Fecha_log)
        SELECT p.Id, @Id, p.Nombre, p.Descripcion, 1, 0, DATEADD(MINUTE, p.Orden, '2000-01-01')
        FROM @p p WHERE NOT EXISTS (SELECT 1 FROM dbo.Proced x WHERE x.Id = p.Id);

        -- Altas / cambios de Valid
        UPDATE va SET Nombre = v.Nombre, Descripcion = v.Descripcion, IdProced = v.IdProced, RequiereFoto = v.RequiereFoto
        FROM dbo.Valid va JOIN @v v ON v.Id = va.Id
        WHERE va.IdProced IN (SELECT Id FROM dbo.Proced WHERE IdGuia = @Id);
        INSERT INTO dbo.Valid (Id, IdProced, Nombre, Descripcion, RequiereFoto, Estado, Eliminado)
        SELECT v.Id, v.IdProced, v.Nombre, v.Descripcion, v.RequiereFoto, 1, 0
        FROM @v v WHERE NOT EXISTS (SELECT 1 FROM dbo.Valid x WHERE x.Id = v.Id);
    END

    COMMIT;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Guia_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Guia SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Guia_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Guia SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- Proced (pasos de la guía)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_Proced_Get
    @Id     VARCHAR(36)  = NULL,
    @IdGuia VARCHAR(36)  = NULL,
    @Nombre VARCHAR(255) = NULL,
    @IdComp VARCHAR(36)  = NULL,
    @Estado BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT pr.Id, pr.IdGuia, pr.Nombre, pr.Descripcion, pr.Estado, pr.Fecha_log
    FROM dbo.Proced pr
    JOIN dbo.Guia g ON g.Id = pr.IdGuia AND g.Eliminado = 0
    WHERE pr.Eliminado = 0
      AND (@Id IS NULL OR pr.Id = @Id)
      AND (@IdGuia IS NULL OR pr.IdGuia = @IdGuia)
      AND (@Nombre IS NULL OR pr.Nombre LIKE '%' + @Nombre + '%')
      AND (@IdComp IS NULL OR g.IdComp = @IdComp)
      AND (@Estado IS NULL OR pr.Estado = @Estado)
    ORDER BY pr.Fecha_log, pr.Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Proced_Set
    @Id          VARCHAR(36),
    @IdGuia      VARCHAR(36),
    @Nombre      VARCHAR(255),
    @Descripcion VARCHAR(MAX),
    @Estado      BIT,
    @Operacion   CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.Proced (Id, IdGuia, Nombre, Descripcion, Estado, Eliminado)
        VALUES (@Id, @IdGuia, @Nombre, ISNULL(@Descripcion, ''), @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Proced SET Nombre = @Nombre, Descripcion = ISNULL(@Descripcion, ''), Estado = @Estado
        WHERE Id = @Id AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Proced_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Proced SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Proced_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Proced SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- Valid (puntos de verificación de cada procedimiento)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_Valid_Get
    @Id       VARCHAR(36)  = NULL,
    @IdProced VARCHAR(36)  = NULL,
    @IdGuia   VARCHAR(36)  = NULL,
    @Nombre   VARCHAR(255) = NULL,
    @IdComp   VARCHAR(36)  = NULL,
    @Estado   BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT v.Id, v.IdProced, pr.IdGuia, v.Nombre, v.Descripcion, v.RequiereFoto, v.Estado, v.Fecha_log
    FROM dbo.Valid v
    JOIN dbo.Proced pr ON pr.Id = v.IdProced AND pr.Eliminado = 0
    JOIN dbo.Guia g    ON g.Id = pr.IdGuia AND g.Eliminado = 0
    WHERE v.Eliminado = 0
      AND (@Id IS NULL OR v.Id = @Id)
      AND (@IdProced IS NULL OR v.IdProced = @IdProced)
      AND (@IdGuia IS NULL OR pr.IdGuia = @IdGuia)
      AND (@Nombre IS NULL OR v.Nombre LIKE '%' + @Nombre + '%')
      AND (@IdComp IS NULL OR g.IdComp = @IdComp)
      AND (@Estado IS NULL OR v.Estado = @Estado)
    ORDER BY v.Fecha_log, v.Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Valid_Set
    @Id          VARCHAR(36),
    @IdProced    VARCHAR(36),
    @Nombre      VARCHAR(255),
    @Descripcion VARCHAR(MAX),
    @RequiereFoto BIT = 0,
    @Estado      BIT,
    @Operacion   CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.Valid (Id, IdProced, Nombre, Descripcion, RequiereFoto, Estado, Eliminado)
        VALUES (@Id, @IdProced, @Nombre, ISNULL(@Descripcion, ''), ISNULL(@RequiereFoto, 0), @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Valid SET Nombre = @Nombre, Descripcion = ISNULL(@Descripcion, ''),
            RequiereFoto = ISNULL(@RequiereFoto, 0), Estado = @Estado
        WHERE Id = @Id AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Valid_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Valid SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Valid_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Valid SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- GuiaEquipo (relación N:M)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_GuiaEquipo_Get
    @Id       VARCHAR(36) = NULL,
    @IdGuia   VARCHAR(36) = NULL,
    @IdEquipo VARCHAR(36) = NULL,
    @IdComp   VARCHAR(36) = NULL,
    @Estado   BIT         = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ge.Id, ge.IdGuia, g.Nombre AS GuiaNombre, ge.IdEquipo, e.Nombre AS EquipoNombre,
           g.Intervalo, ge.Estado, ge.Fecha_log
    FROM dbo.Guia_Equipo ge
    JOIN dbo.Guia g   ON g.Id = ge.IdGuia AND g.Eliminado = 0
    JOIN dbo.Equipo e ON e.Id = ge.IdEquipo AND e.Eliminado = 0
    WHERE ge.Eliminado = 0
      AND (@Id IS NULL OR ge.Id = @Id)
      AND (@IdGuia IS NULL OR ge.IdGuia = @IdGuia)
      AND (@IdEquipo IS NULL OR ge.IdEquipo = @IdEquipo)
      AND (@IdComp IS NULL OR g.IdComp = @IdComp)
      AND (@Estado IS NULL OR ge.Estado = @Estado)
    ORDER BY g.Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_GuiaEquipo_Set
    @Id        VARCHAR(36),
    @IdGuia    VARCHAR(36),
    @IdEquipo  VARCHAR(36),
    @Estado    BIT,
    @Operacion CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Guia_Equipo WHERE IdGuia = @IdGuia AND IdEquipo = @IdEquipo AND Eliminado = 0)
            THROW 50409, 'La guía ya está asignada a este equipo.', 1;
        INSERT INTO dbo.Guia_Equipo (Id, IdGuia, IdEquipo, Estado, Eliminado)
        VALUES (@Id, @IdGuia, @IdEquipo, @Estado, 0);
    END
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Guia_Equipo SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_GuiaEquipo_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Guia_Equipo SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_GuiaEquipo_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Guia_Equipo SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

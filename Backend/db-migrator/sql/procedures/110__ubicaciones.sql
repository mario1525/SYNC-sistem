-- ===================================================================================
-- Jerarquía de ubicación (ADR-03): Planta -> Area (TipoArea) -> SeccionArea
-- Las consultas de hijos aceptan @IdComp y filtran a través de la jerarquía.
-- ===================================================================================

------------------------------------------------------------
-- Planta
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_Planta_Get
    @Id     VARCHAR(36)  = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Region VARCHAR(255) = NULL,
    @IdComp VARCHAR(36)  = NULL,
    @Estado BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Region, IdComp, Estado, Fecha_log
    FROM dbo.Planta
    WHERE Eliminado = 0
      AND (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Region IS NULL OR Region LIKE '%' + @Region + '%')
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@Estado IS NULL OR Estado = @Estado)
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Planta_Set
    @Id        VARCHAR(36),
    @Nombre    VARCHAR(255),
    @Region    VARCHAR(255),
    @IdComp    VARCHAR(36),
    @Estado    BIT,
    @Operacion CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.Planta (Id, Nombre, Region, IdComp, Estado, Eliminado)
        VALUES (@Id, @Nombre, ISNULL(@Region, ''), @IdComp, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Planta
        SET Nombre = @Nombre, Region = ISNULL(@Region, ''), Estado = @Estado
        WHERE Id = @Id AND IdComp = @IdComp AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Planta_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Planta SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Planta_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Planta SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- TipoArea (catálogo global)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_TipoArea_Get
    @Id     VARCHAR(36)  = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Estado BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Descripcion, Estado, Fecha_log
    FROM dbo.TipoArea
    WHERE Eliminado = 0
      AND (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_TipoArea_Set
    @Id          VARCHAR(36),
    @Nombre      VARCHAR(255),
    @Descripcion VARCHAR(255) = NULL,
    @Estado      BIT,
    @Operacion   CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.TipoArea (Id, Nombre, Descripcion, Estado, Eliminado)
        VALUES (@Id, @Nombre, @Descripcion, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.TipoArea SET Nombre = @Nombre, Descripcion = @Descripcion, Estado = @Estado
        WHERE Id = @Id AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_TipoArea_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TipoArea SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_TipoArea_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TipoArea SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- Area
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_Area_Get
    @Id       VARCHAR(36)  = NULL,
    @IdPlanta VARCHAR(36)  = NULL,
    @Nombre   VARCHAR(255) = NULL,
    @TipoArea VARCHAR(36)  = NULL,
    @IdComp   VARCHAR(36)  = NULL,
    @Estado   BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.Id, a.IdPlanta, a.Nombre, a.TipoArea, t.Nombre AS TipoAreaNombre,
           p.Nombre AS PlantaNombre, p.IdComp, a.Estado, a.Fecha_log
    FROM dbo.Area a
    JOIN dbo.Planta p ON p.Id = a.IdPlanta AND p.Eliminado = 0
    LEFT JOIN dbo.TipoArea t ON t.Id = a.TipoArea
    WHERE a.Eliminado = 0
      AND (@Id IS NULL OR a.Id = @Id)
      AND (@IdPlanta IS NULL OR a.IdPlanta = @IdPlanta)
      AND (@Nombre IS NULL OR a.Nombre LIKE '%' + @Nombre + '%')
      AND (@TipoArea IS NULL OR a.TipoArea = @TipoArea)
      AND (@IdComp IS NULL OR p.IdComp = @IdComp)
      AND (@Estado IS NULL OR a.Estado = @Estado)
    ORDER BY p.Nombre, a.Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Area_Set
    @Id        VARCHAR(36),
    @IdPlanta  VARCHAR(36),
    @Nombre    VARCHAR(255),
    @TipoArea  VARCHAR(36),
    @Estado    BIT,
    @Operacion CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.Area (Id, IdPlanta, Nombre, TipoArea, Estado, Eliminado)
        VALUES (@Id, @IdPlanta, @Nombre, @TipoArea, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Area SET IdPlanta = @IdPlanta, Nombre = @Nombre, TipoArea = @TipoArea, Estado = @Estado
        WHERE Id = @Id AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Area_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Area SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Area_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Area SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- SeccionArea
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_SeccionArea_Get
    @Id       VARCHAR(36)  = NULL,
    @IdArea   VARCHAR(36)  = NULL,
    @IdPlanta VARCHAR(36)  = NULL,
    @Nombre   VARCHAR(255) = NULL,
    @IdComp   VARCHAR(36)  = NULL,
    @Estado   BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.Id, s.IdArea, s.Nombre, a.Nombre AS AreaNombre, a.IdPlanta, p.Nombre AS PlantaNombre,
           t.Nombre AS TipoAreaNombre, p.IdComp, s.Estado, s.Fecha_log
    FROM dbo.SeccionArea s
    JOIN dbo.Area a   ON a.Id = s.IdArea AND a.Eliminado = 0
    JOIN dbo.Planta p ON p.Id = a.IdPlanta AND p.Eliminado = 0
    LEFT JOIN dbo.TipoArea t ON t.Id = a.TipoArea
    WHERE s.Eliminado = 0
      AND (@Id IS NULL OR s.Id = @Id)
      AND (@IdArea IS NULL OR s.IdArea = @IdArea)
      AND (@IdPlanta IS NULL OR a.IdPlanta = @IdPlanta)
      AND (@Nombre IS NULL OR s.Nombre LIKE '%' + @Nombre + '%')
      AND (@IdComp IS NULL OR p.IdComp = @IdComp)
      AND (@Estado IS NULL OR s.Estado = @Estado)
    ORDER BY p.Nombre, a.Nombre, s.Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_SeccionArea_Set
    @Id        VARCHAR(36),
    @IdArea    VARCHAR(36),
    @Nombre    VARCHAR(255),
    @Estado    BIT,
    @Operacion CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.SeccionArea (Id, IdArea, Nombre, Estado, Eliminado)
        VALUES (@Id, @IdArea, @Nombre, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.SeccionArea SET IdArea = @IdArea, Nombre = @Nombre, Estado = @Estado
        WHERE Id = @Id AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_SeccionArea_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.SeccionArea SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_SeccionArea_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.SeccionArea SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

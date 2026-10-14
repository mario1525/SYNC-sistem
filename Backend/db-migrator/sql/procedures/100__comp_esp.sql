-- ===================================================================================
-- Comp (Compañía) y Esp (Especialidad)
-- Convención: dbo.db_Sp_<Entidad>_<Get|Set|Del|Active>, CREATE OR ALTER, soft-delete.
-- ===================================================================================

CREATE OR ALTER PROCEDURE dbo.db_Sp_Comp_Get
    @Id     VARCHAR(36)  = NULL,
    @Nombre VARCHAR(255) = NULL,
    @NIT    VARCHAR(255) = NULL,
    @Estado BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Ciudad, NIT, Direccion, Sector, Estado, Fecha_log
    FROM dbo.Comp
    WHERE Eliminado = 0
      AND (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@NIT IS NULL OR NIT LIKE '%' + @NIT + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Comp_Set
    @Id        VARCHAR(36),
    @Nombre    VARCHAR(255),
    @NIT       VARCHAR(255),
    @Direccion VARCHAR(255),
    @Sector    VARCHAR(255),
    @Ciudad    VARCHAR(255),
    @Estado    BIT,
    @Operacion CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.Comp (Id, Nombre, Ciudad, NIT, Direccion, Sector, Estado, Eliminado)
        VALUES (@Id, @Nombre, @Ciudad, @NIT, @Direccion, @Sector, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Comp
        SET Nombre = @Nombre, Ciudad = @Ciudad, NIT = @NIT, Direccion = @Direccion, Sector = @Sector, Estado = @Estado
        WHERE Id = @Id AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Comp_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Comp SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Comp_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Comp SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- Esp: IdComp NULL = especialidad global visible para todas las compañías
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_Esp_Get
    @Id     VARCHAR(36)  = NULL,
    @Nombre VARCHAR(255) = NULL,
    @IdComp VARCHAR(36)  = NULL,
    @Estado BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Descripcion, IdComp, Estado, Fecha_log
    FROM dbo.Esp
    WHERE Eliminado = 0
      AND (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@IdComp IS NULL OR IdComp = @IdComp OR IdComp IS NULL)
      AND (@Estado IS NULL OR Estado = @Estado)
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Esp_Set
    @Id          VARCHAR(36),
    @Nombre      VARCHAR(255),
    @Descripcion VARCHAR(MAX),
    @IdComp      VARCHAR(36) = NULL,
    @Estado      BIT,
    @Operacion   CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.Esp (Id, Nombre, Descripcion, IdComp, Estado, Eliminado)
        VALUES (@Id, @Nombre, ISNULL(@Descripcion, ''), @IdComp, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Esp
        SET Nombre = @Nombre, Descripcion = ISNULL(@Descripcion, ''), Estado = @Estado
        WHERE Id = @Id AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Esp_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Esp SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Esp_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Esp SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

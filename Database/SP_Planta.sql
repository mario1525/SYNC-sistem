-- =============================================
-- Autor:        Mario Beltran
-- Fecha:        2024/06/5
-- Descripción:  Creación de procedimientos almacenados
--               para la gestión de las tablas de ubicación.
-- =============================================

PRINT 'Creación de procedimientos para la tabla Planta'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Planta_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Planta_Get
    DROP PROCEDURE dbo.db_Sp_Planta_Set
    DROP PROCEDURE dbo.db_Sp_Planta_Del
    DROP PROCEDURE dbo.db_Sp_Planta_Active
END
GO

-- Obtener datos de Planta
CREATE PROCEDURE dbo.db_Sp_Planta_Get
    @Id VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Region VARCHAR(255) = NULL,
    @Estado BIT = NULL
AS 
BEGIN
    SELECT Id, Nombre, Region, IdComp, Estado, Fecha_log
    FROM dbo.Planta
    WHERE (Id = @Id OR @Id IS NULL)
      AND (Nombre LIKE '%' + @Nombre + '%' OR @Nombre IS NULL)
      AND (Region LIKE '%' + @Region + '%' OR @Region IS NULL)
      AND (Estado = @Estado OR @Estado IS NULL)
END
GO

-- Insertar o actualizar Planta
CREATE PROCEDURE dbo.db_Sp_Planta_Set
    @Id VARCHAR(36),
    @Nombre VARCHAR(255),
    @Region VARCHAR(255),
    @IdComp VARCHAR(36),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Planta(Id, Nombre, Region, IdComp, Estado, Fecha_log)
        VALUES(@Id, @Nombre, @Region, @IdComp, @Estado, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Planta
        SET Nombre = @Nombre, Region = @Region, IdComp = @IdComp, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Eliminar Planta (marcar como inactiva)
CREATE PROCEDURE dbo.db_Sp_Planta_Del
    @Id VARCHAR(36)
AS
BEGIN
    UPDATE dbo.Planta SET Estado = 0 WHERE Id = @Id;
    SELECT Estado FROM dbo.Planta WHERE Id = @Id;
END
GO

-- Activar/Desactivar Planta
CREATE PROCEDURE dbo.db_Sp_Planta_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Planta SET Estado = @Estado WHERE Id = @Id;
END
GO

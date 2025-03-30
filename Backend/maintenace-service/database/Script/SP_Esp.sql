-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/5
-- Description: Creación de los procedimientos almacenados
--         para la tabla Esp de la DB DB_SYNC
-- ========================================================

PRINT 'Creación de procedimientos para la tabla Esp'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Esp_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Esp_Get
    DROP PROCEDURE dbo.db_Sp_Esp_Set
    DROP PROCEDURE dbo.db_Sp_Esp_Del
    DROP PROCEDURE dbo.db_Sp_Esp_Active
END
GO

-- Procedimiento para obtener los datos
PRINT 'Creación del procedimiento Esp Get'
GO
CREATE PROCEDURE dbo.db_Sp_Esp_Get
    @Id          VARCHAR(36) = NULL,
    @Nombre      VARCHAR(255) = NULL,
    @IdComp      VARCHAR(36) = NULL,
    @Estado      INT = NULL
AS 
BEGIN
    SELECT Id, Nombre, IdComp, Estado, Fecha_log     
    FROM dbo.Esp
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND Nombre LIKE CASE WHEN ISNULL(@Nombre,'')='' THEN Nombre ELSE '%'+@Nombre+'%' END    
    AND IdComp = CASE WHEN ISNULL(@IdComp,'')='' THEN IdComp ELSE @IdComp END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Eliminado = 0
END
GO

-- Procedimiento para insertar o actualizar los datos
PRINT 'Creación del procedimiento Esp Set'
GO
CREATE PROCEDURE dbo.db_Sp_Esp_Set
    @Id          VARCHAR(36),
    @Nombre      VARCHAR(255),
    @IdComp      VARCHAR(36),
    @Estado      BIT,
    @Operacion   VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Esp(Id, Nombre, IdComp, Estado, Fecha_log, Eliminado)
        VALUES(@Id, @Nombre, @IdComp, @Estado, DEFAULT, 0)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Esp
        SET Nombre = @Nombre, IdComp = @IdComp, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos (marcar como eliminado)
PRINT 'Creación del procedimiento Esp Del'
GO
CREATE PROCEDURE dbo.db_Sp_Esp_Del
    @Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.Esp
    SET Eliminado = 1
    WHERE Id = @Id;
    
    -- Obtiene el estado "Eliminado" después de la actualización 
    SELECT Eliminado
    FROM dbo.Esp
    WHERE Id = @Id;    
END
GO

-- Procedimiento para activar o desactivar los datos
PRINT 'Creación del procedimiento Esp Active'
GO
CREATE PROCEDURE dbo.db_Sp_Esp_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Esp
    SET Estado = @Estado
    WHERE Id = @Id
END
GO
-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/5
-- Description: Creación de los procedimientos almacenados
--         para la tabla Proced de la DB DB_SYNC
-- ========================================================

PRINT 'Creación de procedimientos para la tabla Proced'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Proced_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Proced_Get
    DROP PROCEDURE dbo.db_Sp_Proced_Set
    DROP PROCEDURE dbo.db_Sp_Proced_Del
    DROP PROCEDURE dbo.db_Sp_Proced_Active
END
GO

-- Procedimiento para obtener los datos
PRINT 'Creación del procedimiento Proced Get'
GO
CREATE PROCEDURE dbo.db_Sp_Proced_Get
    @Id          VARCHAR(36) = NULL,
    @Nombre      VARCHAR(255) = NULL,
    @IdGuia      VARCHAR(36) = NULL,
    @Estado      INT = NULL
AS 
BEGIN
    SELECT Id, Nombre, IdGuia, Estado, Fecha_log     
    FROM dbo.Proced
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND Nombre LIKE CASE WHEN ISNULL(@Nombre,'')='' THEN Nombre ELSE '%'+@Nombre+'%' END    
    AND IdGuia = CASE WHEN ISNULL(@IdGuia,'')='' THEN IdGuia ELSE @IdGuia END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Eliminado = 0
END
GO

-- Procedimiento para insertar o actualizar los datos
PRINT 'Creación del procedimiento Proced Set'
GO
CREATE PROCEDURE dbo.db_Sp_Proced_Set
    @Id              VARCHAR(36),
    @IdGuia          VARCHAR(36),
    @Nombre          VARCHAR(255),
    @Descripcion     VARCHAR(max),
    @Estado          BIT,
    @Operacion       VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Proced(Id, IdGuia, Nombre, Descripcion, Estado, Eliminado, Fecha_log)
        VALUES(@Id, @IdGuia, @Nombre, @Descripcion, @Estado, 0, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Proced
        SET IdGuia = @IdGuia, Nombre = @Nombre, Descripcion = @Descripcion, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos (marcar como eliminado)
PRINT 'Creación del procedimiento Proced Del'
GO
CREATE PROCEDURE dbo.db_Sp_Proced_Del
    @Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.Proced
    SET Eliminado = 1
    WHERE Id = @Id;
    
    -- Obtiene el estado "Eliminado" después de la actualización 
    SELECT Eliminado
    FROM dbo.Proced
    WHERE Id = @Id;    
END
GO

-- Procedimiento para activar o desactivar los datos
PRINT 'Creación del procedimiento Proced Active'
GO
CREATE PROCEDURE dbo.db_Sp_Proced_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Proced
    SET Estado = @Estado
    WHERE Id = @Id
END
GO
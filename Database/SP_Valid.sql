-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/5
-- Description: Creación de los procedimientos almacenados
--         para la tabla Valid de la DB DB_SYNC
-- ========================================================

PRINT 'Creación de procedimientos para la tabla Valid'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Valid_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Valid_Get
    DROP PROCEDURE dbo.db_Sp_Valid_Set
    DROP PROCEDURE dbo.db_Sp_Valid_Del
    DROP PROCEDURE dbo.db_Sp_Valid_Active
END
GO

-- Procedimiento para obtener los datos
PRINT 'Creación del procedimiento Valid Get'
GO
CREATE PROCEDURE dbo.db_Sp_Valid_Get
    @Id          VARCHAR(36) = NULL,
    @Nombre      VARCHAR(255) = NULL,
    @IdProced    VARCHAR(36) = NULL,
    @Estado      INT = NULL
AS 
BEGIN
    SELECT Id, Nombre, IdProced, Estado, Fecha_log     
    FROM dbo.Valid
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND Nombre LIKE CASE WHEN ISNULL(@Nombre,'')='' THEN Nombre ELSE '%'+@Nombre+'%' END    
    AND IdProced = CASE WHEN ISNULL(@IdProced,'')='' THEN IdProced ELSE @IdProced END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Eliminado = 0
END
GO

-- Procedimiento para insertar o actualizar los datos
PRINT 'Creación del procedimiento Valid Set'
GO
CREATE PROCEDURE dbo.db_Sp_Valid_Set
    @Id              VARCHAR(36),
    @IdProced        VARCHAR(36),
    @Nombre          VARCHAR(255),
    @Descripcion     VARCHAR(max),
    @Estado          BIT,
    @Operacion       VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Valid(Id, IdProced, Nombre, Descripcion, Estado, Eliminado, Fecha_log)
        VALUES(@Id, @IdProced, @Nombre, @Descripcion, @Estado, 0, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Valid
        SET IdProced = @IdProced, Nombre = @Nombre, Descripcion = @Descripcion, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos (marcar como eliminado)
PRINT 'Creación del procedimiento Valid Del'
GO
CREATE PROCEDURE dbo.db_Sp_Valid_Del
    @Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.Valid
    SET Eliminado = 1
    WHERE Id = @Id;
    
    -- Obtiene el estado "Eliminado" después de la actualización 
    SELECT Eliminado
    FROM dbo.Valid
    WHERE Id = @Id;    
END
GO

-- Procedimiento para activar o desactivar los datos
PRINT 'Creación del procedimiento Valid Active'
GO
CREATE PROCEDURE dbo.db_Sp_Valid_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Valid
    SET Estado = @Estado
    WHERE Id = @Id
END
GO
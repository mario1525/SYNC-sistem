PRINT 'Creación de procedimientos para la tabla SeccionBodega'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_SeccionBodega_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_SeccionBodega_Get
    DROP PROCEDURE dbo.db_Sp_SeccionBodega_Set
    DROP PROCEDURE dbo.db_Sp_SeccionBodega_Del
    DROP PROCEDURE dbo.db_Sp_SeccionBodega_Active
END
GO

-- Procedimiento para obtener los datos
PRINT 'Creación del procedimiento SeccionBodega Get'
GO
CREATE PROCEDURE dbo.db_Sp_SeccionBodega_Get
    @Id       VARCHAR(36) = NULL,
    @IdBodega VARCHAR(36) = NULL,
    @Nombre   VARCHAR(255) = NULL,
    @Estado   INT = NULL
AS 
BEGIN
    SELECT Id, IdBodega, Nombre, Estado, Fecha_log     
    FROM dbo.SeccionBodega
    WHERE Id = COALESCE(@Id, Id)
    AND IdBodega = COALESCE(@IdBodega, IdBodega)
    AND Nombre LIKE COALESCE('%'+@Nombre+'%', Nombre)
    AND Estado = COALESCE(@Estado, Estado)
END
GO

-- Procedimiento para insertar o actualizar los datos
PRINT 'Creación del procedimiento SeccionBodega Set'
GO
CREATE PROCEDURE dbo.db_Sp_SeccionBodega_Set
    @Id       VARCHAR(36),
    @IdBodega VARCHAR(36),
    @Nombre   VARCHAR(255),
    @Estado   BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.SeccionBodega(Id, IdBodega, Nombre, Estado, Fecha_log)
        VALUES(@Id, @IdBodega, @Nombre, @Estado, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.SeccionBodega
        SET IdBodega = @IdBodega, Nombre = @Nombre, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos (marcar como eliminado)
PRINT 'Creación del procedimiento SeccionBodega Del'
GO
CREATE PROCEDURE dbo.db_Sp_SeccionBodega_Del
    @Id VARCHAR(36)
AS
BEGIN
    DELETE FROM dbo.SeccionBodega WHERE Id = @Id
END
GO

-- Procedimiento para activar o desactivar los datos
PRINT 'Creación del procedimiento SeccionBodega Active'
GO
CREATE PROCEDURE dbo.db_Sp_SeccionBodega_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.SeccionBodega
    SET Estado = @Estado
    WHERE Id = @Id
END
GO

PRINT 'Creación de procedimientos para la tabla Patio'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Patio_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Patio_Get
    DROP PROCEDURE dbo.db_Sp_Patio_Set
    DROP PROCEDURE dbo.db_Sp_Patio_Del
    DROP PROCEDURE dbo.db_Sp_Patio_Active
END
GO

-- Procedimiento para obtener los datos
PRINT 'Creación del procedimiento Patio Get'
GO
CREATE PROCEDURE dbo.db_Sp_Patio_Get
    @Id       VARCHAR(36) = NULL,
    @IdBodega VARCHAR(36) = NULL,
    @Nombre   VARCHAR(255) = NULL,
    @Estado   INT = NULL
AS 
BEGIN
    SELECT Id, IdBodega, Nombre, Estado, Fecha_log     
    FROM dbo.Patio
    WHERE Id = COALESCE(@Id, Id)
    AND IdBodega = COALESCE(@IdBodega, IdBodega)
    AND Nombre LIKE COALESCE('%'+@Nombre+'%', Nombre)
    AND Estado = COALESCE(@Estado, Estado)
END
GO

-- Procedimiento para insertar o actualizar los datos
PRINT 'Creación del procedimiento Patio Set'
GO
CREATE PROCEDURE dbo.db_Sp_Patio_Set
    @Id       VARCHAR(36),
    @IdBodega VARCHAR(36),
    @Nombre   VARCHAR(255),
    @Estado   BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Patio(Id, IdBodega, Nombre, Estado, Fecha_log)
        VALUES(@Id, @IdBodega, @Nombre, @Estado, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Patio
        SET IdBodega = @IdBodega, Nombre = @Nombre, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos (marcar como eliminado)
PRINT 'Creación del procedimiento Patio Del'
GO
CREATE PROCEDURE dbo.db_Sp_Patio_Del
    @Id VARCHAR(36)
AS
BEGIN
    DELETE FROM dbo.Patio WHERE Id = @Id
END
GO

-- Procedimiento para activar o desactivar los datos
PRINT 'Creación del procedimiento Patio Active'
GO
CREATE PROCEDURE dbo.db_Sp_Patio_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Patio
    SET Estado = @Estado
    WHERE Id = @Id
END
GO

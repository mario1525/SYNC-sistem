-- Procedimiento para obtener los datos de Bodega
PRINT 'Creación del procedimiento Bodega Get'
GO
CREATE PROCEDURE dbo.db_Sp_Bodega_Get
    @Id        VARCHAR(36) = NULL,
    @IdPlanta  VARCHAR(36) = NULL,
    @Nombre    VARCHAR(255) = NULL,
    @Estado    BIT = NULL
AS
BEGIN
    SELECT Id, IdPlanta, Nombre, Estado, Fecha_log
    FROM dbo.Bodega
    WHERE (@Id IS NULL OR Id = @Id)
    AND (@IdPlanta IS NULL OR IdPlanta = @IdPlanta)
    AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
    AND (@Estado IS NULL OR Estado = @Estado)
END
GO

-- Procedimiento para insertar o actualizar los datos de Bodega
PRINT 'Creación del procedimiento Bodega Set'
GO
CREATE PROCEDURE dbo.db_Sp_Bodega_Set
    @Id        VARCHAR(36),
    @IdPlanta  VARCHAR(36),
    @Nombre    VARCHAR(255),
    @Estado    BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Bodega (Id, IdPlanta, Nombre, Estado, Fecha_log)
        VALUES (@Id, @IdPlanta, @Nombre, @Estado, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Bodega
        SET IdPlanta = @IdPlanta, Nombre = @Nombre, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos de Bodega (marcar como eliminado)
PRINT 'Creación del procedimiento Bodega Del'
GO
CREATE PROCEDURE dbo.db_Sp_Bodega_Del
    @Id VARCHAR(36)
AS
BEGIN
    DELETE FROM dbo.Bodega WHERE Id = @Id
END
GO

-- Procedimiento para activar o desactivar los datos de Bodega
PRINT 'Creación del procedimiento Bodega Active'
GO
CREATE PROCEDURE dbo.db_Sp_Bodega_Active
    @Id     VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Bodega
    SET Estado = @Estado
    WHERE Id = @Id
END
GO
-- ========================================================
-- Author:        Mario Beltran
-- Create Date:   2024/06/5
-- Description:   Creación de los procedimientos almacenados
--                para la tabla AreaFuncional de la DB
-- ========================================================

PRINT 'Creación de procedimientos para la tabla AreaFuncional'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_AreaFuncional_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_AreaFuncional_Get
    DROP PROCEDURE dbo.db_Sp_AreaFuncional_Set
    DROP PROCEDURE dbo.db_Sp_AreaFuncional_Del
    DROP PROCEDURE dbo.db_Sp_AreaFuncional_Active
END
GO

-- Procedimiento para obtener los datos
PRINT 'Creación del procedimiento AreaFuncional Get'
GO
CREATE PROCEDURE dbo.db_Sp_AreaFuncional_Get
    @Id       VARCHAR(36) = NULL,
    @IdPlanta VARCHAR(36) = NULL,
    @Nombre   VARCHAR(255) = NULL,
    @Estado   BIT = NULL
AS 
BEGIN
    SELECT Id, IdPlanta, Nombre, Estado, Fecha_log     
    FROM dbo.AreaFuncional
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND IdPlanta = CASE WHEN ISNULL(@IdPlanta,'')='' THEN IdPlanta ELSE @IdPlanta END
    AND Nombre LIKE CASE WHEN ISNULL(@Nombre,'')='' THEN Nombre ELSE '%'+@Nombre+'%' END    
    AND Estado = CASE WHEN @Estado IS NULL THEN Estado ELSE @Estado END
END
GO

-- Procedimiento para insertar o actualizar los datos
PRINT 'Creación del procedimiento AreaFuncional Set'
GO
CREATE PROCEDURE dbo.db_Sp_AreaFuncional_Set
    @Id       VARCHAR(36),
    @IdPlanta VARCHAR(36),
    @Nombre   VARCHAR(255),
    @Estado   BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.AreaFuncional(Id, IdPlanta, Nombre, Estado, Fecha_log)
        VALUES(@Id, @IdPlanta, @Nombre, @Estado, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.AreaFuncional
        SET IdPlanta = @IdPlanta, Nombre = @Nombre, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos (marcar como eliminado)
PRINT 'Creación del procedimiento AreaFuncional Del'
GO
CREATE PROCEDURE dbo.db_Sp_AreaFuncional_Del
    @Id VARCHAR(36)
AS
BEGIN
    DELETE FROM dbo.AreaFuncional WHERE Id = @Id;
END
GO

-- Procedimiento para activar o desactivar los datos
PRINT 'Creación del procedimiento AreaFuncional Active'
GO
CREATE PROCEDURE dbo.db_Sp_AreaFuncional_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.AreaFuncional
    SET Estado = @Estado
    WHERE Id = @Id
END
GO

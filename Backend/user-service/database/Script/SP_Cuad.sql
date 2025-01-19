
-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2025/01/19
-- Description: creacion de los procedimientos almacenados
-- para la tabla Cuad de la DB DB_SYNC_USER
-- ========================================================

PRINT 'Creacion procedimientos tabla Cuad'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Cuad%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Cuad_Get
	DROP PROCEDURE dbo.db_Sp_Cuad_Set
	DROP PROCEDURE dbo.db_Sp_Cuad_Det
	DROP PROCEDURE dbo.db_Sp_Cuad_Active
END

IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Cuad%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Cuad_Get
    DROP PROCEDURE dbo.db_Sp_Cuad_Set
    DROP PROCEDURE dbo.db_Sp_Cuad_Det
    DROP PROCEDURE dbo.db_Sp_Cuad_Active
END
GO

-- sp para obtener los datos
PRINT 'Creacion procedimiento Cuad Get '
GO
CREATE PROCEDURE dbo.db_Sp_Cuad_Get
    @Id                               VARCHAR(36) = NULL,
    @Nombre                           VARCHAR(40) = NULL,
    @IdComp                           VARCHAR(36) = NULL,
    @IdEsp                            VARCHAR(36) = NULL,
    @Estado                           INT = NULL
AS
BEGIN
    SELECT DISTINCT c.Id, c.Nombre, c.IdComp, c.Estado, c.Fecha_log
    FROM dbo.Cuad c
    INNER JOIN dbo.Users u ON c.Id = u.IdCuad
    WHERE (@Id IS NULL OR c.Id = @Id)
    AND (@Nombre IS NULL OR c.Nombre LIKE '%' + @Nombre + '%')
    AND (@IdComp IS NULL OR c.IdComp = @IdComp)
    AND (@IdEsp IS NULL OR u.IdEsp = @IdEsp)
    AND (@Estado IS NULL OR c.Estado = @Estado)
    AND c.Eliminado = 0
END
GO

-- sp para insertar y actualizar
PRINT 'Creacion procedimiento Cuad Set '
GO
CREATE PROCEDURE dbo.db_Sp_Cuad_Set
    @Id                 VARCHAR(36),
    @Nombre             VARCHAR(40),
    @IdComp             VARCHAR(36),
    @Estado             INT,
    @Operacion          VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Cuad(Id, Nombre, IdComp, Estado, Fecha_log, Eliminado)
        VALUES(@Id, @Nombre, @IdComp, @Estado, DEFAULT, 0)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Cuad
        SET Nombre = @Nombre, IdComp = @IdComp, Estado = @Estado
        WHERE Id = @Id
    END
END

-- sp para eliminar
PRINT 'Creacion procedimiento Cuad Del '
GO
CREATE PROCEDURE dbo.db_Sp_Cuad_Del
    @Id VARCHAR(36) 
AS
BEGIN
    UPDATE dbo.Cuad
    SET Eliminado = 1
    WHERE Id = @Id
END

-- sp para activar
PRINT 'Creacion procedimiento Cuad Active '
GO
CREATE PROCEDURE dbo.db_Sp_Cuad_Active
    @Id VARCHAR(36)
AS
BEGIN
    UPDATE dbo.Cuad
    SET Estado = 1
    WHERE Id = @Id
END


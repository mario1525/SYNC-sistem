
-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2025/01/19
-- Description: creacion de los procedimientos almacenados
-- para la tabla Turno de la DB DB_SYNC_USER
-- ========================================================

PRINT 'Creacion procedimientos tabla Turno'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Turno%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Turno_Get
    DROP PROCEDURE dbo.db_Sp_Turno_Set
    DROP PROCEDURE dbo.db_Sp_Turno_Det
END

-- sp para obtener los datos
PRINT 'Creacion procedimiento Turno Get '
GO
CREATE PROCEDURE dbo.db_Sp_Turno_Get
    @Id              VARCHAR(36) = NULL, 
    @Nombre          VARCHAR(255) = NULL, 
    @IdComp          VARCHAR(36) = NULL     

AS
BEGIN
    SELECT Id, Nombre, HoraInicio, HoraFin, Fecha_log     
    FROM dbo.Turno
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND Nombre LIKE CASE WHEN ISNULL(@Nombre,'')='' THEN Nombre ELSE '%'+@Nombre+'%' END    
    AND IdComp LIKE CASE WHEN ISNULL(@IdComp,'')='' THEN IdComp ELSE '%'+@IdComp+'%' END
    AND Eliminado = 0
END            

-- sp para insertar y actualizar
PRINT 'Creacion procedimiento Turno Set '
GO
CREATE PROCEDURE dbo.db_Sp_Turno_Set
    @Id              VARCHAR(36),
    @Nombre          VARCHAR(255),
    @HoraInicio      TIME,
    @HoraFin         TIME,
    @IdComp          VARCHAR(36),
    @Operacion       VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Turno(Id, Nombre, HoraInicio, HoraFin, IdComp, Fecha_log, Eliminado)
        VALUES(@Id, @Nombre, @HoraInicio, @HoraFin, @IdComp, DEFAULT, 0)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Turno
        SET Nombre = @Nombre, HoraInicio = @HoraInicio, HoraFin = @HoraFin, IdComp = @IdComp
        WHERE Id = @Id
    END
END

-- sp para eliminar
PRINT 'Creacion procedimiento Turno Det '
GO
CREATE PROCEDURE dbo.db_Sp_Turno_Det
    @Id              VARCHAR(36)
AS
BEGIN
    UPDATE dbo.Turno
    SET Eliminado = 1
    WHERE Id = @Id
END

    
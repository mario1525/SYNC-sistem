
-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2025/01/19
-- Description: creacion de los procedimientos almacenados
-- para la tabla Horario de la DB DB_SYNC_USER
-- ========================================================

PRINT 'Creacion procedimientos tabla Horario'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Horario%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Horario_Get
    DROP PROCEDURE dbo.db_Sp_Horario_Set
    DROP PROCEDURE dbo.db_Sp_Horario_Det
    DROP PROCEDURE dbo.db_Sp_Horario_Active
END 

-- sp para obtener los datos
PRINT 'Creacion procedimiento Horario Get '
GO
CREATE PROCEDURE dbo.db_Sp_Horario_Get
    @Id              VARCHAR(36) = NULL, 
    @IdTurno         VARCHAR(36) = NULL, 
    @IdCuad          VARCHAR(36) = NULL,      
    @DiaSemana       INT = NULL,
    @EsFestivo       INT = NULL,
    @Estado          INT = NULL
AS
BEGIN
    SELECT Id, IdTurno, IdCuad, DiaSemana, EsFestivo, Estado, Fecha_log     
    FROM dbo.Horario
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND IdTurno LIKE CASE WHEN ISNULL(@IdTurno,'')='' THEN IdTurno ELSE '%'+@IdTurno+'%' END    
    AND IdCuad LIKE CASE WHEN ISNULL(@IdCuad,'')='' THEN IdCuad ELSE '%'+@IdCuad+'%' END
    AND DiaSemana = CASE WHEN ISNULL(@DiaSemana,0)=0 THEN DiaSemana ELSE @DiaSemana END
    AND EsFestivo = CASE WHEN ISNULL(@EsFestivo,0)=0 THEN EsFestivo ELSE @EsFestivo END
    AND Estado = CASE WHEN ISNULL(@Estado,0)=0 THEN Estado ELSE @Estado END
    AND Eliminado = 0
END

-- sp para insertar y actualizar
PRINT 'Creacion procedimiento Horario Set '
GO
CREATE PROCEDURE dbo.db_Sp_Horario_Set
    @Id              VARCHAR(36),
    @IdTurno         VARCHAR(36),
    @IdCuad          VARCHAR(36),
    @DiaSemana       INT,
    @EsFestivo       INT,
    @Estado          BIT,
    @Operacion       VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Horario(Id, IdTurno, IdCuad, DiaSemana, EsFestivo, Estado, Fecha_log, Eliminado)
        VALUES(@Id, @IdTurno, @IdCuad, @DiaSemana, @EsFestivo, @Estado, DEFAULT, 0)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Horario
        SET IdTurno = @IdTurno, IdCuad = @IdCuad, DiaSemana = @DiaSemana, EsFestivo = @EsFestivo, Estado = @Estado
        WHERE Id = @Id
    END
END

-- sp para eliminar
PRINT 'Creacion procedimiento Horario Det '
GO
CREATE PROCEDURE dbo.db_Sp_Horario_Det
    @Id              VARCHAR(36)
AS
BEGIN
    UPDATE dbo.Horario
    SET Eliminado = 1
    WHERE Id = @Id
END

-- sp para activar
PRINT 'Creacion procedimiento Horario Active '
GO
CREATE PROCEDURE dbo.db_Sp_Horario_Active
    @Id              VARCHAR(36)
AS
BEGIN
    UPDATE dbo.Horario
    SET Estado = 1
    WHERE Id = @Id
END



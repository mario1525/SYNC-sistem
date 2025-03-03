-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/5
-- Description: Creación de los procedimientos almacenados
--         para la tabla Actividad_Equipo de la DB DB_SYNC
-- ========================================================

PRINT 'Creación de procedimientos para la tabla Actividad_Equipo'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Guia_Act_Equipo_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Guia_Act_Equipo_Get
    DROP PROCEDURE dbo.db_Sp_Guia_Act_Equipo_Set
    DROP PROCEDURE dbo.db_Sp_Guia_Act_Equipo_Del
    DROP PROCEDURE dbo.db_Sp_Guia_Act_Equipo_Active
END
GO

-- Procedimiento para obtener los datos
PRINT 'Creación del procedimiento Actividad_Equipo Get'
GO
CREATE PROCEDURE dbo.db_Sp_Guia_Act_Equipo_Get
    @IdActividad VARCHAR(36) = NULL,
    @Estado      INT = NULL
AS 
BEGIN
    SELECT Id, IdActividad, IdEquipo, IdGuia, Estado, Fecha_log     
    FROM dbo.Actividad_Equipo
    WHERE IdActividad = CASE WHEN ISNULL(@IdActividad,'')='' THEN IdActividad ELSE @IdActividad END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Eliminado = 0
END
GO

-- Procedimiento para insertar o actualizar los datos
PRINT 'Creación del procedimiento Actividad_Equipo Set'
GO
CREATE PROCEDURE dbo.db_Sp_Guia_Act_Equipo_Set
    @Id              VARCHAR(36),
    @IdActividad     VARCHAR(36),
    @IdEquipo        VARCHAR(36),
    @IdGuia          VARCHAR(36),
    @Estado          BIT,
    @Operacion       VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Actividad_Equipo(Id, IdActividad, IdEquipo, IdGuia, Estado, Eliminado, Fecha_log)
        VALUES(@Id, @IdActividad, @IdEquipo, @IdGuia, @Estado, 0, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Actividad_Equipo
        SET IdActividad = @IdActividad, IdEquipo = @IdEquipo, IdGuia = @IdGuia, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos (marcar como eliminado)
PRINT 'Creación del procedimiento Actividad_Equipo Del'
GO
CREATE PROCEDURE dbo.db_Sp_Guia_Act_Equipo_Del
    @Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.Actividad_Equipo
    SET Eliminado = 1
    WHERE Id = @Id;
    
    -- Obtiene el estado "Eliminado" después de la actualización 
    SELECT Eliminado
    FROM dbo.Actividad_Equipo
    WHERE Id = @Id;    
END
GO

-- Procedimiento para activar o desactivar los datos
PRINT 'Creación del procedimiento Actividad_Equipo Active'
GO
CREATE PROCEDURE dbo.db_Sp_Guia_Act_Equipo_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Actividad_Equipo
    SET Estado = @Estado
    WHERE Id = @Id
END
GO
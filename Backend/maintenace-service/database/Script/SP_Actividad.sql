-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/5
-- Description: Creación de los procedimientos almacenados
--         para la tabla Actividad de la DB DB_SYNC
-- ========================================================

PRINT 'Creación de procedimientos para la tabla Actividad'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Actividad_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Actividad_Get
    DROP PROCEDURE dbo.db_Sp_Actividad_Set
    DROP PROCEDURE dbo.db_Sp_Actividad_Del
    DROP PROCEDURE dbo.db_Sp_Actividad_Active
END
GO

-- Procedimiento para obtener los datos
PRINT 'Creación del procedimiento Actividad Get'
GO
CREATE PROCEDURE dbo.db_Sp_Actividad_Get
    @Id              VARCHAR(36) = NULL,
    @IdComp          VARCHAR(36) = NULL,
    @IdTipoActividad VARCHAR(36) = NULL,
    @IdCuad          VARCHAR(36) = NULL,
    @Estado          INT = NULL
AS 
BEGIN
    SELECT Id, Descripcion, IdTipoActividad, Ubicacion, FechaEjecucion, IdCuad, Detalle, Intervalo, Estado, Fecha_log     
    FROM dbo.Actividad
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND IdTipoActividad = CASE WHEN ISNULL(@IdTipoActividad,'')='' THEN IdTipoActividad ELSE @IdTipoActividad END    
    AND IdCuad = CASE WHEN ISNULL(@IdCuad,'')='' THEN IdCuad ELSE @IdCuad END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Eliminado = 0
END
GO

-- Procedimiento para insertar o actualizar los datos
PRINT 'Creación del procedimiento Actividad Set'
GO
CREATE PROCEDURE dbo.db_Sp_Actividad_Set
    @Id              VARCHAR(36),
    @IdComp          VARCHAR(36),
    @Descripcion     VARCHAR(max),
    @IdTipoActividad VARCHAR(36),
    @Ubicacion       VARCHAR(255),
    @FechaEjecucion  SMALLDATETIME,
    @IdCuad          VARCHAR(36),
    @Detalle         VARCHAR(max),
    @Intervalo       INT,
    @Estado          BIT,
    @Operacion       VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Actividad(Id, IdComp, Descripcion, IdTipoActividad, Ubicacion, FechaEjecucion, IdCuad, Detalle, Intervalo, Estado, Eliminado, Fecha_log)
        VALUES(@Id, @IdComp, @Descripcion, @IdTipoActividad, @Ubicacion, @FechaEjecucion, @IdCuad, @Detalle, @Intervalo, @Estado, 0, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Actividad
        SET IdComp = @IdComp, Descripcion = @Descripcion, IdTipoActividad = @IdTipoActividad, Ubicacion = @Ubicacion, FechaEjecucion = @FechaEjecucion, IdCuad = @IdCuad, Detalle = @Detalle, Intervalo = @Intervalo, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos (marcar como eliminado)
PRINT 'Creación del procedimiento Actividad Del'
GO
CREATE PROCEDURE dbo.db_Sp_Actividad_Del
    @Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.Actividad
    SET Eliminado = 1
    WHERE Id = @Id;
    
    -- Obtiene el estado "Eliminado" después de la actualización 
    SELECT Eliminado
    FROM dbo.Actividad
    WHERE Id = @Id;    
END
GO

-- Procedimiento para activar o desactivar los datos
PRINT 'Creación del procedimiento Actividad Active'
GO
CREATE PROCEDURE dbo.db_Sp_Actividad_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Actividad
    SET Estado = @Estado
    WHERE Id = @Id
END
GO
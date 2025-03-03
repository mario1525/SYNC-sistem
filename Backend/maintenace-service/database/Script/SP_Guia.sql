-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/5
-- Description: Creación de los procedimientos almacenados
--         para la tabla Guia de la DB DB_SYNC
-- ========================================================

PRINT 'Creación de procedimientos para la tabla Guia'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Guia_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Guia_Get
    DROP PROCEDURE dbo.db_Sp_Guia_Set
    DROP PROCEDURE dbo.db_Sp_Guia_Del
    DROP PROCEDURE dbo.db_Sp_Guia_Active
END
GO

-- Procedimiento para obtener los datos
PRINT 'Creación del procedimiento Guia Get'
GO
CREATE PROCEDURE dbo.db_Sp_Guia_Get
    @Id          VARCHAR(36) = NULL,
    @Nombre      VARCHAR(255) = NULL,
    @IdComp      VARCHAR(36) = NULL,
    @IdEsp       VARCHAR(36) = NULL,
    @Estado      INT = NULL
AS 
BEGIN
    SELECT Id, Nombre, Descripcion, Proceso, Inspeccion, Herramientas, IdComp, IdEsp, SeguridadInd, SeguridadAmb, Intervalo, Importante, Insumos, Personal, Duracion, Logistica, Situacion, Notas, CreatedBy, UpdatedBy, FechaUpdate, Estado, Fecha_log     
    FROM dbo.Guia
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND Nombre LIKE CASE WHEN ISNULL(@Nombre,'')='' THEN Nombre ELSE '%'+@Nombre+'%' END    
    AND IdComp = CASE WHEN ISNULL(@IdComp,'')='' THEN IdComp ELSE @IdComp END
    AND IdEsp = CASE WHEN ISNULL(@IdEsp,'')='' THEN IdEsp ELSE @IdEsp END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Eliminado = 0
END
GO

-- Procedimiento para insertar o actualizar los datos
PRINT 'Creación del procedimiento Guia Set'
GO
CREATE PROCEDURE dbo.db_Sp_Guia_Set
    @Id              VARCHAR(36),
    @Nombre          VARCHAR(255),
    @Descripcion     VARCHAR(max),
    @Proceso         VARCHAR(max),
    @Inspeccion      VARCHAR(max),
    @Herramientas    VARCHAR(max),
    @IdComp          VARCHAR(36),
    @IdEsp           VARCHAR(36),
    @SeguridadInd    VARCHAR(max),
    @SeguridadAmb    VARCHAR(max),
    @Intervalo       INT,
    @Importante      VARCHAR(max),
    @Insumos         VARCHAR(max),
    @Personal        INT,
    @Duracion        INT,
    @Logistica       VARCHAR(max),
    @Situacion       VARCHAR(255),
    @Notas           VARCHAR(max),
    @CreatedBy       VARCHAR(36),
    @UpdatedBy       VARCHAR(36),
    @Estado          BIT,
    @Operacion       VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Guia(Id, Nombre, Descripcion, Proceso, Inspeccion, Herramientas, IdComp, IdEsp, SeguridadInd, SeguridadAmb, Intervalo, Importante, Insumos, Personal, Duracion, Logistica, Situacion, Notas, CreatedBy, UpdatedBy, FechaUpdate, Estado, Eliminado, Fecha_log)
        VALUES(@Id, @Nombre, @Descripcion, @Proceso, @Inspeccion, @Herramientas, @IdComp, @IdEsp, @SeguridadInd, @SeguridadAmb, @Intervalo, @Importante, @Insumos, @Personal, @Duracion, @Logistica, @Situacion, @Notas, @CreatedBy, @UpdatedBy, DEFAULT, @Estado, 0, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Guia
        SET Nombre = @Nombre, Descripcion = @Descripcion, Proceso = @Proceso, Inspeccion = @Inspeccion, Herramientas = @Herramientas, IdComp = @IdComp, IdEsp = @IdEsp, SeguridadInd = @SeguridadInd, SeguridadAmb = @SeguridadAmb, Intervalo = @Intervalo, Importante = @Importante, Insumos = @Insumos, Personal = @Personal, Duracion = @Duracion, Logistica = @Logistica, Situacion = @Situacion, Notas = @Notas, UpdatedBy = @UpdatedBy, FechaUpdate = DEFAULT, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos (marcar como eliminado)
PRINT 'Creación del procedimiento Guia Del'
GO
CREATE PROCEDURE dbo.db_Sp_Guia_Del
    @Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.Guia
    SET Eliminado = 1
    WHERE Id = @Id;
    
    -- Obtiene el estado "Eliminado" después de la actualización 
    SELECT Eliminado
    FROM dbo.Guia
    WHERE Id = @Id;    
END
GO

-- Procedimiento para activar o desactivar los datos
PRINT 'Creación del procedimiento Guia Active'
GO
CREATE PROCEDURE dbo.db_Sp_Guia_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Guia
    SET Estado = @Estado
    WHERE Id = @Id
END
GO
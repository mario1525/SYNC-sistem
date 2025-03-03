-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/5
-- Description: Creación de los procedimientos almacenados
--         para la tabla Equipo de la DB DB_SYNC
-- ========================================================

PRINT 'Creación de procedimientos para la tabla Equipo'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Equipo_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Equipo_Get
    DROP PROCEDURE dbo.db_Sp_Equipo_Set
    DROP PROCEDURE dbo.db_Sp_Equipo_Del
    DROP PROCEDURE dbo.db_Sp_Equipo_Active
END
GO

-- Procedimiento para obtener los datos
PRINT 'Creación del procedimiento Equipo Get'
GO
CREATE PROCEDURE dbo.db_Sp_Equipo_Get
    @Id          VARCHAR(36) = NULL,
    @Nombre      VARCHAR(255) = NULL,
    @IdComp      VARCHAR(36) = NULL,
    @Marca       VARCHAR(255) = NULL,
    @NSerie      VARCHAR(255) = NULL,
    @Estado      INT = NULL
AS 
BEGIN
    SELECT Id, Nombre, Descripcion, IdComp, Modelo, NSerie, Ubicacion, Fabricante, Marca, Funcion, Peso, Cilindraje, Potencia, Ancho, Alto, Largo, Capacidad, AnioFabricacion, Caracteristicas, Seccion, Estado, Fecha_log     
    FROM dbo.Equipo
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND Nombre LIKE CASE WHEN ISNULL(@Nombre,'')='' THEN Nombre ELSE '%'+@Nombre+'%' END    
    AND IdComp = CASE WHEN ISNULL(@IdComp,'')='' THEN IdComp ELSE @IdComp END
    AND Marca LIKE CASE WHEN ISNULL(@Marca,'')='' THEN Marca ELSE '%'+@Marca+'%' END
    AND NSerie LIKE CASE WHEN ISNULL(@NSerie,'')='' THEN NSerie ELSE '%'+@NSerie+'%' END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Eliminado = 0
END
GO

-- Procedimiento para insertar o actualizar los datos
PRINT 'Creación del procedimiento Equipo Set'
GO
CREATE PROCEDURE dbo.db_Sp_Equipo_Set
    @Id              VARCHAR(36),
    @Nombre          VARCHAR(255),
    @Descripcion     VARCHAR(max),
    @IdComp          VARCHAR(36),
    @Modelo          VARCHAR(255),
    @NSerie          VARCHAR(255),
    @Ubicacion       VARCHAR(255),
    @Fabricante      VARCHAR(255),
    @Marca           VARCHAR(255),
    @Funcion         VARCHAR(max),
    @Peso            INT,
    @Cilindraje      INT,
    @Potencia        INT,
    @Ancho           INT,
    @Alto            INT,
    @Largo           INT,
    @Capacidad       INT,
    @AnioFabricacion INT,
    @Caracteristicas VARCHAR(max),
    @Seccion         VARCHAR(255),
    @Estado          BIT,
    @Operacion       VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Equipo(Id, Nombre, Descripcion, IdComp, Modelo, NSerie, Ubicacion, Fabricante, Marca, Funcion, Peso, Cilindraje, Potencia, Ancho, Alto, Largo, Capacidad, AnioFabricacion, Caracteristicas, Seccion, Estado, Eliminado, Fecha_log)
        VALUES(@Id, @Nombre, @Descripcion, @IdComp, @Modelo, @NSerie, @Ubicacion, @Fabricante, @Marca, @Funcion, @Peso, @Cilindraje, @Potencia, @Ancho, @Alto, @Largo, @Capacidad, @AnioFabricacion, @Caracteristicas, @Seccion, @Estado, 0, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Equipo
        SET Nombre = @Nombre, Descripcion = @Descripcion, IdComp = @IdComp, Modelo = @Modelo, NSerie = @NSerie, Ubicacion = @Ubicacion, Fabricante = @Fabricante, Marca = @Marca, Funcion = @Funcion, Peso = @Peso, Cilindraje = @Cilindraje, Potencia = @Potencia, Ancho = @Ancho, Alto = @Alto, Largo = @Largo, Capacidad = @Capacidad, AnioFabricacion = @AnioFabricacion, Caracteristicas = @Caracteristicas, Seccion = @Seccion, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

-- Procedimiento para eliminar los datos (marcar como eliminado)
PRINT 'Creación del procedimiento Equipo Del'
GO
CREATE PROCEDURE dbo.db_Sp_Equipo_Del
    @Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.Equipo
    SET Eliminado = 1
    WHERE Id = @Id;
    
    -- Obtiene el estado "Eliminado" después de la actualización 
    SELECT Eliminado
    FROM dbo.Equipo
    WHERE Id = @Id;    
END
GO

-- Procedimiento para activar o desactivar los datos
PRINT 'Creación del procedimiento Equipo Active'
GO
CREATE PROCEDURE dbo.db_Sp_Equipo_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Equipo
    SET Estado = @Estado
    WHERE Id = @Id
END
GO
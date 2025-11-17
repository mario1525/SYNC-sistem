-- ========================================================
-- Author:        [Tu Nombre]
-- Create Date:   2024/03/24
-- Description:   Creación de procedimientos almacenados
--                para la tabla UbicacionEquipo de la DB
-- ========================================================

PRINT 'Creacion procedimientos tabla UbicacionEquipo'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_UbicacionEquipo_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_UbicacionEquipo_Get
    DROP PROCEDURE dbo.db_Sp_UbicacionEquipo_Set
    DROP PROCEDURE dbo.db_Sp_UbicacionEquipo_Del
    DROP PROCEDURE dbo.db_Sp_UbicacionEquipo_Active
END

PRINT 'Creacion procedimiento UbicacionEquipo Get '
GO
CREATE PROCEDURE dbo.db_Sp_UbicacionEquipo_Get
    @Id              VARCHAR(36) = NULL,
    @IdEquipo        VARCHAR(36) = NULL,
    @TipoUbicacion   VARCHAR(50) = NULL,
    @IdPlanta        VARCHAR(36) = NULL,
    @IdAreaFuncional VARCHAR(36) = NULL,
    @IdBodega        VARCHAR(36) = NULL,
    @IdSeccionBodega VARCHAR(36) = NULL,
    @IdPatio         VARCHAR(36) = NULL,
    @Estado          BIT = NULL
AS 
BEGIN
    SELECT * 
    FROM dbo.UbicacionEquipo
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdEquipo IS NULL OR IdEquipo = @IdEquipo)
      AND (@TipoUbicacion IS NULL OR TipoUbicacion = @TipoUbicacion)
      AND (@IdPlanta IS NULL OR IdPlanta = @IdPlanta)
      AND (@IdAreaFuncional IS NULL OR IdAreaFuncional = @IdAreaFuncional)
      AND (@IdBodega IS NULL OR IdBodega = @IdBodega)
      AND (@IdSeccionBodega IS NULL OR IdSeccionBodega = @IdSeccionBodega)
      AND (@IdPatio IS NULL OR IdPatio = @IdPatio)
      AND (@Estado IS NULL OR Estado = @Estado)
END
GO

PRINT 'Creacion procedimiento UbicacionEquipo Set '
GO
CREATE PROCEDURE dbo.db_Sp_UbicacionEquipo_Set
    @Id              VARCHAR(36),
    @IdEquipo        VARCHAR(36),
    @TipoUbicacion   VARCHAR(50),
    @IdPlanta        VARCHAR(36),
    @IdAreaFuncional VARCHAR(36) = NULL,
    @IdBodega        VARCHAR(36) = NULL,
    @IdSeccionBodega VARCHAR(36) = NULL,
    @IdPatio         VARCHAR(36) = NULL,
    @Estado          BIT,
    @Operacion       CHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.UbicacionEquipo(Id, IdEquipo, TipoUbicacion, IdPlanta, IdAreaFuncional, IdBodega, IdSeccionBodega, IdPatio, Estado, Fecha_log)
        VALUES(@Id, @IdEquipo, @TipoUbicacion, @IdPlanta, @IdAreaFuncional, @IdBodega, @IdSeccionBodega, @IdPatio, @Estado, DEFAULT)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.UbicacionEquipo
        SET IdEquipo = @IdEquipo, TipoUbicacion = @TipoUbicacion, IdPlanta = @IdPlanta, IdAreaFuncional = @IdAreaFuncional,
            IdBodega = @IdBodega, IdSeccionBodega = @IdSeccionBodega, IdPatio = @IdPatio, Estado = @Estado
        WHERE Id = @Id
    END
END
GO

PRINT 'Creacion procedimiento UbicacionEquipo Del '
GO
CREATE PROCEDURE dbo.db_Sp_UbicacionEquipo_Del
    @Id VARCHAR(36)
AS
BEGIN
    DELETE FROM dbo.UbicacionEquipo WHERE Id = @Id
END
GO

PRINT 'Creacion procedimiento UbicacionEquipo Active '
GO
CREATE PROCEDURE dbo.db_Sp_UbicacionEquipo_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.UbicacionEquipo
    SET Estado = @Estado
    WHERE Id = @Id
END
GO

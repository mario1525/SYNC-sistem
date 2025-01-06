-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/5
-- Description: creacion de los procedimientos almacenados
--         para la tabla comp de la DB DB_SYNC
-- ========================================================

PRINT 'Creacion procedimientos tabla Compania'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'dbSpCompania%')
BEGIN
    DROP PROCEDURE dbo.dbSpCompGet
	DROP PROCEDURE dbo.dbSpCompSet
	DROP PROCEDURE dbo.dbSpCompDet
	DROP PROCEDURE dbo.dbSpCompActive
END

PRINT 'Creacion procedimiento Comp Get '
GO
CREATE PROCEDURE dbo.dbSpCompGet
    @Id          VARCHAR(36),
    @Nombre      VARCHAR(255),
    @NIT         VARCHAR(255),
    @Sector      VARCHAR(255),
    @Ciudad      VARCHAR(255),
    @Direccion   VARCHAR(255),
    @Estado      INT
AS 
BEGIN
    SELECT Id, Nombre, Ciudad, NIT, Direccion, Sector, Estado, Fecha_log     
    FROM dbo.Comp
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND Nombre LIKE CASE WHEN ISNULL(@Nombre,'')='' THEN Nombre ELSE '%'+@Nombre+'%' END    
    AND NIT LIKE CASE WHEN ISNULL(@NIT,'')='' THEN NIT ELSE '%'+@NIT+'%' END
    AND Ciudad LIKE CASE WHEN ISNULL(@Ciudad,'')='' THEN Ciudad ELSE '%'+@Ciudad+'%' END
    AND Sector LIKE CASE WHEN ISNULL(@Sector,'')='' THEN Sector ELSE '%'+@Sector+'%' END
    AND Direccion LIKE CASE WHEN ISNULL(@Direccion,'')='' THEN Direccion ELSE '%'+@Direccion+'%' END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Eliminado = 0
END
GO


PRINT 'Creacion procedimiento Compania Set '
GO
CREATE PROCEDURE dbo.dbSpCompSet
    @Id          VARCHAR(36),
    @Nombre      VARCHAR(255),
    @NIT         VARCHAR(255),
    @Direccion   VARCHAR(255),
    @Sector      VARCHAR(255),
    @Ciudad      VARCHAR(255),
    @Estado      BIT,
    @Operacion   VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Comp(Id, Nombre, Ciudad, NIT, Direccion, Sector, Estado, Fecha_log, Eliminado)
        VALUES(@Id, @Nombre,@Ciudad, @NIT, @Direccion, @Sector, @Estado, DEFAULT, 0)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Comp
        SET Nombre = @Nombre, Ciudad = @Ciudad, NIT = @NIT, Direccion = @Direccion, Sector = @Sector, Estado = @Estado
        WHERE Id = @Id
    END
END
GO


PRINT 'Creacion procedimiento Compania Del '
GO
CREATE PROCEDURE dbo.dbSpCompDel
    @Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.Comp
    SET Eliminado = 1
    WHERE Id = @Id;
    
    -- Obtiene el estado "Eliminado" después de la actualización 
    SELECT Eliminado
    FROM dbo.Comp
    WHERE Id = @Id;    
END

GO
PRINT 'Creacion procedimiento Compania Active '
GO
CREATE PROCEDURE dbo.dbSpCompActive
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Comp
    SET Estado = @Estado
    WHERE Id = @Id
END
GO




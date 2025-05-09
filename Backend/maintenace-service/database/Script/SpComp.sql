-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/5
-- Description: creacion de los procedimientos almacenados
--         para la tabla comp de la DB DB_SYNC
-- ========================================================

PRINT 'Creacion procedimientos tabla Compania'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Comp_%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Comp_Get
	DROP PROCEDURE dbo.db_Sp_Comp_Set
	DROP PROCEDURE dbo.db_Sp_Comp_Del
	DROP PROCEDURE dbo.db_Sp_Comp_Active
END

PRINT 'Creacion procedimiento Comp Get '
GO
CREATE PROCEDURE dbo.db_Sp_Comp_Get
    @Id          VARCHAR(36),
    @Nombre      VARCHAR(255),
    @NIT         VARCHAR(255),
    @Sector      VARCHAR(255),
    @Ciudad      VARCHAR(255),
    @Direccion   VARCHAR(255),
    @Estado      INT
AS 
BEGIN
    SELECT Id, Nombre, City, NIT, Direccion, Sector, Estado, LogDate     
    FROM dbo.Comp
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND Nombre LIKE CASE WHEN ISNULL(@Nombre,'')='' THEN Nombre ELSE '%'+@Nombre+'%' END    
    AND NIT LIKE CASE WHEN ISNULL(@NIT,'')='' THEN NIT ELSE '%'+@NIT+'%' END
    AND City LIKE CASE WHEN ISNULL(@Ciudad,'')='' THEN City ELSE '%'+@Ciudad+'%' END
    AND Sector LIKE CASE WHEN ISNULL(@Sector,'')='' THEN Sector ELSE '%'+@Sector+'%' END
    AND Direccion LIKE CASE WHEN ISNULL(@Direccion,'')='' THEN Direccion ELSE '%'+@Direccion+'%' END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Deleted = 0
END
GO

PRINT 'Creacion procedimiento Compania Set '
GO
CREATE PROCEDURE dbo.db_Sp_Comp_Set
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
        INSERT INTO dbo.Comp(Id, Nombre, City, NIT, Direccion, Sector, Estado, LogDate, Deleted)
        VALUES(@Id, @Nombre,@Ciudad, @NIT, @Direccion, @Sector, @Estado, DEFAULT, 0)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Comp
        SET Nombre = @Nombre, City = @Ciudad, NIT = @NIT, Direccion = @Direccion, Sector = @Sector, Estado = @Estado
        WHERE Id = @Id
    END
END
GO


PRINT 'Creacion procedimiento Compania Del '
GO
CREATE PROCEDURE dbo.db_Sp_Comp_Del
    @Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.Comp
    SET Deleted = 1
    WHERE Id = @Id;
    
    -- Obtiene el estado "Eliminado" después de la actualización 
    SELECT Deleted
    FROM dbo.Comp
    WHERE Id = @Id;    
END

GO
PRINT 'Creacion procedimiento Compania Active '
GO
CREATE PROCEDURE dbo.db_Sp_Comp_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.Comp
    SET Estado = @Estado
    WHERE Id = @Id
END
GO



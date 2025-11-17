-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/05
-- Description: Procedimientos almacenados para la tabla Comp en DB_SYNC
--              (Unificada). Idempotentes y alineados con el esquema de Database/script.sql
-- ========================================================

USE DB_SYNC;
GO

PRINT 'Creación/Actualización de SPs para tabla Comp';
IF OBJECT_ID('dbo.db_Sp_Comp_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Comp_Get;
IF OBJECT_ID('dbo.db_Sp_Comp_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Comp_Set;
IF OBJECT_ID('dbo.db_Sp_Comp_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Comp_Del;
IF OBJECT_ID('dbo.db_Sp_Comp_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Comp_Active;
GO

-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener compañías con filtros opcionales
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Comp_Get
    @Id        VARCHAR(36)  = NULL,
    @Nombre    VARCHAR(255) = NULL,
    @NIT       VARCHAR(255) = NULL,
    @Sector    VARCHAR(255) = NULL,
    @Ciudad    VARCHAR(255) = NULL,
    @Direccion VARCHAR(255) = NULL,
    @Estado    INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  Id,
            Nombre,
            Ciudad,
            NIT,
            Direccion,
            Sector,
            Estado,
            Eliminado,
            Fecha_log
    FROM dbo.Comp
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@NIT IS NULL OR NIT LIKE '%' + @NIT + '%')
      AND (@Sector IS NULL OR Sector LIKE '%' + @Sector + '%')
      AND (@Ciudad IS NULL OR Ciudad LIKE '%' + @Ciudad + '%')
      AND (@Direccion IS NULL OR Direccion LIKE '%' + @Direccion + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO

-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar compañía (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Comp_Set
    @Id        VARCHAR(36),
    @Nombre    VARCHAR(255),
    @NIT       VARCHAR(255),
    @Direccion VARCHAR(255),
    @Sector    VARCHAR(255),
    @Ciudad    VARCHAR(255),
    @Estado    BIT,
    @Operacion VARCHAR(1)   -- 'I' insertar, 'A' actualizar
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Comp (Id, Nombre, Ciudad, NIT, Direccion, Sector, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @Nombre, @Ciudad, @NIT, @Direccion, @Sector, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Comp
        SET Nombre    = @Nombre,
            Ciudad    = @Ciudad,
            NIT       = @NIT,
            Direccion = @Direccion,
            Sector    = @Sector,
            Estado    = @Estado
        WHERE Id = @Id;
    END
END
GO

-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/05
-- Description: Eliminado lógico de compañía
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Comp_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Comp
    SET Eliminado = 1
    WHERE Id = @Id;

    SELECT Eliminado
    FROM dbo.Comp
    WHERE Id = @Id;
END
GO

-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar compañía
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Comp_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Comp
    SET Estado = @Estado
    WHERE Id = @Id;
END
GO

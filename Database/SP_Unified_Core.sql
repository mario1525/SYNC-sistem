-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Procedimientos almacenados unificados para DB_SYNC (paquete núcleo)
--               Incluye CRUD/Activación para tablas: Comp, Esp, CompEsp, Users,
--               User_Cred, Planta, TipoArea, Area, SeccionArea, Equipo.
--               Todos los objetos se crean con idempotencia (DROP si existen).
-- ========================================================

USE DB_SYNC;
GO


/* ========================================================
   Comp - Compañía
   ======================================================== */
PRINT 'Creando SPs de Comp';
IF OBJECT_ID('dbo.db_Sp_Comp_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Comp_Get;
IF OBJECT_ID('dbo.db_Sp_Comp_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Comp_Set;
IF OBJECT_ID('dbo.db_Sp_Comp_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Comp_Del;
IF OBJECT_ID('dbo.db_Sp_Comp_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Comp_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener compañías con filtros opcionales
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Comp_Get
    @Id        VARCHAR(36) = NULL,
    @Nombre    VARCHAR(255) = NULL,
    @NIT       VARCHAR(255) = NULL,
    @Sector    VARCHAR(255) = NULL,
    @Ciudad    VARCHAR(255) = NULL,
    @Direccion VARCHAR(255) = NULL,
    @Estado    INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Ciudad, NIT, Direccion, Sector, Estado, Eliminado, Fecha_log
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
-- Author:      Mario Beltran
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
    @Operacion VARCHAR(1)  -- 'I' insertar, 'A' actualizar
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
        SET Nombre = @Nombre,
            Ciudad = @Ciudad,
            NIT = @NIT,
            Direccion = @Direccion,
            Sector = @Sector,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Eliminado lógico de compañía
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Comp_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Comp SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Comp WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar compañía
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Comp_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Comp SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Esp - Especialidad
   ======================================================== */
PRINT 'Creando SPs de Esp';
IF OBJECT_ID('dbo.db_Sp_Esp_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Esp_Get;
IF OBJECT_ID('dbo.db_Sp_Esp_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Esp_Set;
IF OBJECT_ID('dbo.db_Sp_Esp_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Esp_Del;
IF OBJECT_ID('dbo.db_Sp_Esp_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Esp_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener especialidades con filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Esp_Get
    @Id VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Descripcion, Estado, Eliminado, Fecha_log
    FROM dbo.Esp
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar especialidad (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Esp_Set
    @Id VARCHAR(36),
    @Nombre VARCHAR(255),
    @Descripcion VARCHAR(MAX),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Esp (Id, Nombre, Descripcion, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @Nombre, @Descripcion, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Esp
        SET Nombre = @Nombre,
            Descripcion = @Descripcion,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Eliminado lógico de especialidad
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Esp_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Esp SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Esp WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar especialidad
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Esp_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Esp SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   CompEsp - Relación Compañía-Especialidad
   ======================================================== */
PRINT 'Creando SPs de CompEsp';
IF OBJECT_ID('dbo.db_Sp_CompEsp_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_CompEsp_Get;
IF OBJECT_ID('dbo.db_Sp_CompEsp_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_CompEsp_Set;
IF OBJECT_ID('dbo.db_Sp_CompEsp_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_CompEsp_Del;
IF OBJECT_ID('dbo.db_Sp_CompEsp_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_CompEsp_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener relaciones compañía-especialidad
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_CompEsp_Get
    @Id VARCHAR(36) = NULL,
    @IdComp VARCHAR(36) = NULL,
    @IdEsp VARCHAR(36) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdComp, IdEsp, Estado, Eliminado, Fecha_log
    FROM dbo.CompEsp
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@IdEsp IS NULL OR IdEsp = @IdEsp)
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar relación compañía-especialidad (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_CompEsp_Set
    @Id VARCHAR(36),
    @IdComp VARCHAR(36),
    @IdEsp VARCHAR(36),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.CompEsp (Id, IdComp, IdEsp, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @IdComp, @IdEsp, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.CompEsp
        SET IdComp = @IdComp,
            IdEsp = @IdEsp,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Eliminado lógico CompEsp
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_CompEsp_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.CompEsp SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.CompEsp WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar CompEsp
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_CompEsp_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.CompEsp SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Users - Usuarios
   ======================================================== */
PRINT 'Creando SPs de Users';
IF OBJECT_ID('dbo.db_Sp_Users_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Users_Get;
IF OBJECT_ID('dbo.db_Sp_Users_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Users_Set;
IF OBJECT_ID('dbo.db_Sp_Users_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Users_Del;
IF OBJECT_ID('dbo.db_Sp_Users_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Users_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener usuarios con filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Users_Get
    @Id             VARCHAR(36) = NULL,
    @Nombre         VARCHAR(40) = NULL,
    @Apellido       VARCHAR(40) = NULL,
    @Identificacion BIGINT = NULL,
    @Correo         VARCHAR(60) = NULL,
    @IdComp         VARCHAR(36) = NULL,
    @IdCuad         VARCHAR(36) = NULL,
    @IdEsp          VARCHAR(36) = NULL,
    @Cargo          VARCHAR(60) = NULL,
    @Rol            VARCHAR(200) = NULL,
    @Estado         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Apellido, Identificacion, Correo, IdComp, IdCuad, IdEsp, Cargo, Rol, Estado, Eliminado, Fecha_log
    FROM dbo.Users
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Apellido IS NULL OR Apellido LIKE '%' + @Apellido + '%')
      AND (@Identificacion IS NULL OR Identificacion = @Identificacion)
      AND (@Correo IS NULL OR Correo LIKE '%' + @Correo + '%')
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@IdCuad IS NULL OR IdCuad = @IdCuad)
      AND (@IdEsp IS NULL OR IdEsp = @IdEsp)
      AND (@Cargo IS NULL OR Cargo LIKE '%' + @Cargo + '%')
      AND (@Rol IS NULL OR Rol LIKE '%' + @Rol + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar usuario (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Users_Set
    @Id             VARCHAR(36),
    @Nombre         VARCHAR(40),
    @Apellido       VARCHAR(40),
    @Identificacion BIGINT,
    @Correo         VARCHAR(60),
    @IdComp         VARCHAR(36),
    @IdCuad         VARCHAR(36),
    @IdEsp          VARCHAR(36),
    @Cargo          VARCHAR(60),
    @Rol            VARCHAR(200),
    @Estado         BIT,
    @Operacion      VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Users (Id, Nombre, Apellido, Identificacion, Correo, IdComp, IdCuad, IdEsp, Cargo, Rol, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @Nombre, @Apellido, @Identificacion, @Correo, @IdComp, @IdCuad, @IdEsp, @Cargo, @Rol, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Users
        SET Nombre = @Nombre,
            Apellido = @Apellido,
            Identificacion = @Identificacion,
            Correo = @Correo,
            IdComp = @IdComp,
            IdCuad = @IdCuad,
            IdEsp = @IdEsp,
            Cargo = @Cargo,
            Rol = @Rol,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Eliminado lógico usuario
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Users_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Users SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Users WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar usuario
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Users_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Users SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   User_Cred - Credenciales de usuario
   ======================================================== */
PRINT 'Creando SPs de User_Cred';
IF OBJECT_ID('dbo.db_Sp_User_Cred_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_User_Cred_Get;
IF OBJECT_ID('dbo.db_Sp_User_Cred_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_User_Cred_Set;
IF OBJECT_ID('dbo.db_Sp_User_Cred_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_User_Cred_Del;
IF OBJECT_ID('dbo.db_Sp_User_Cred_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_User_Cred_Active;
IF OBJECT_ID('dbo.db_Sp_User_Cred_Validate','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_User_Cred_Validate;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener credenciales con filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_User_Cred_Get
    @IdUserCred VARCHAR(36) = NULL,
    @User       VARCHAR(255) = NULL,
    @Estado     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Usuario, Contrasenia, IdUser, Estado, Eliminado, Fecha_log
    FROM dbo.User_Cred
    WHERE (@IdUserCred IS NULL OR Id = @IdUserCred)
      AND (@User IS NULL OR Usuario LIKE '%' + @User + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar credenciales (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_User_Cred_Set
    @Id          VARCHAR(36),
    @Usuario     VARCHAR(255),
    @Contrasenia VARCHAR(255),
    @IdUser      VARCHAR(36),
    @Estado      BIT,
    @Operacion   VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.User_Cred (Id, Usuario, Contrasenia, IdUser, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @Usuario, @Contrasenia, @IdUser, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.User_Cred
        SET Usuario = @Usuario,
            Contrasenia = @Contrasenia,
            IdUser = @IdUser,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Eliminado lógico de credenciales
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_User_Cred_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.User_Cred SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.User_Cred WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar credenciales
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_User_Cred_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.User_Cred SET Estado = @Estado WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Validar si un usuario ya tiene credenciales activas
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_User_Cred_Validate
    @IdUsuario VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Resultado BIT =
    CASE WHEN EXISTS (
        SELECT 1
        FROM dbo.User_Cred
        WHERE IdUser = @IdUsuario
          AND Estado = 1
          AND Eliminado = 0
    ) THEN 1 ELSE 0 END;

    SELECT @Resultado AS EsValido;
END
GO


/* ========================================================
   Planta
   ======================================================== */
PRINT 'Creando SPs de Planta';
IF OBJECT_ID('dbo.db_Sp_Planta_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Planta_Get;
IF OBJECT_ID('dbo.db_Sp_Planta_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Planta_Set;
IF OBJECT_ID('dbo.db_Sp_Planta_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Planta_Del;
IF OBJECT_ID('dbo.db_Sp_Planta_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Planta_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener plantas con filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Planta_Get
    @Id VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Region VARCHAR(255) = NULL,
    @IdComp VARCHAR(36) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Region, IdComp, Estado, Fecha_log
    FROM dbo.Planta
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Region IS NULL OR Region LIKE '%' + @Region + '%')
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@Estado IS NULL OR Estado = @Estado);
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar planta (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Planta_Set
    @Id VARCHAR(36),
    @Nombre VARCHAR(255),
    @Region VARCHAR(255),
    @IdComp VARCHAR(36),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Planta (Id, Nombre, Region, IdComp, Estado, Fecha_log)
        VALUES (@Id, @Nombre, @Region, @IdComp, @Estado, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Planta
        SET Nombre = @Nombre,
            Region = @Region,
            IdComp = @IdComp,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Marcar planta inactiva (baja lógica)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Planta_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Planta SET Estado = 0 WHERE Id = @Id;
    SELECT Estado FROM dbo.Planta WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar planta
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Planta_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Planta SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   TipoArea
   ======================================================== */
PRINT 'Creando SPs de TipoArea';
IF OBJECT_ID('dbo.db_Sp_TipoArea_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_TipoArea_Get;
IF OBJECT_ID('dbo.db_Sp_TipoArea_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_TipoArea_Set;
IF OBJECT_ID('dbo.db_Sp_TipoArea_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_TipoArea_Del;
IF OBJECT_ID('dbo.db_Sp_TipoArea_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_TipoArea_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener TipoArea con filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_TipoArea_Get
    @Id VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Descripcion, Estado, Fecha_log
    FROM dbo.TipoArea
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Estado IS NULL OR Estado = @Estado);
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar TipoArea (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_TipoArea_Set
    @Id VARCHAR(36),
    @Nombre VARCHAR(255),
    @Descripcion VARCHAR(255) = NULL,
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.TipoArea (Id, Nombre, Descripcion, Estado, Fecha_log)
        VALUES (@Id, @Nombre, @Descripcion, @Estado, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.TipoArea
        SET Nombre = @Nombre,
            Descripcion = @Descripcion,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica TipoArea
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_TipoArea_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TipoArea SET Estado = 0 WHERE Id = @Id;
    SELECT Estado FROM dbo.TipoArea WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar TipoArea
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_TipoArea_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TipoArea SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Area
   ======================================================== */
PRINT 'Creando SPs de Area';
IF OBJECT_ID('dbo.db_Sp_Area_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Area_Get;
IF OBJECT_ID('dbo.db_Sp_Area_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Area_Set;
IF OBJECT_ID('dbo.db_Sp_Area_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Area_Del;
IF OBJECT_ID('dbo.db_Sp_Area_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Area_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener áreas con filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Area_Get
    @Id VARCHAR(36) = NULL,
    @IdPlanta VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @TipoArea VARCHAR(36) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdPlanta, Nombre, TipoArea, Estado, Fecha_log
    FROM dbo.Area
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdPlanta IS NULL OR IdPlanta = @IdPlanta)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@TipoArea IS NULL OR TipoArea = @TipoArea)
      AND (@Estado IS NULL OR Estado = @Estado);
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar área (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Area_Set
    @Id VARCHAR(36),
    @IdPlanta VARCHAR(36),
    @Nombre VARCHAR(255),
    @TipoArea VARCHAR(36),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Area (Id, IdPlanta, Nombre, TipoArea, Estado, Fecha_log)
        VALUES (@Id, @IdPlanta, @Nombre, @TipoArea, @Estado, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Area
        SET IdPlanta = @IdPlanta,
            Nombre = @Nombre,
            TipoArea = @TipoArea,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica de área
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Area_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Area SET Estado = 0 WHERE Id = @Id;
    SELECT Estado FROM dbo.Area WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar área
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Area_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Area SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   SeccionArea
   ======================================================== */
PRINT 'Creando SPs de SeccionArea';
IF OBJECT_ID('dbo.db_Sp_SeccionArea_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_SeccionArea_Get;
IF OBJECT_ID('dbo.db_Sp_SeccionArea_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_SeccionArea_Set;
IF OBJECT_ID('dbo.db_Sp_SeccionArea_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_SeccionArea_Del;
IF OBJECT_ID('dbo.db_Sp_SeccionArea_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_SeccionArea_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener secciones por filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_SeccionArea_Get
    @Id VARCHAR(36) = NULL,
    @IdArea VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdArea, Nombre, Estado, Fecha_log
    FROM dbo.SeccionArea
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdArea IS NULL OR IdArea = @IdArea)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Estado IS NULL OR Estado = @Estado);
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar sección (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_SeccionArea_Set
    @Id VARCHAR(36),
    @IdArea VARCHAR(36),
    @Nombre VARCHAR(255),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.SeccionArea (Id, IdArea, Nombre, Estado, Fecha_log)
        VALUES (@Id, @IdArea, @Nombre, @Estado, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.SeccionArea
        SET IdArea = @IdArea,
            Nombre = @Nombre,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica sección
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_SeccionArea_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.SeccionArea SET Estado = 0 WHERE Id = @Id;
    SELECT Estado FROM dbo.SeccionArea WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar sección
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_SeccionArea_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.SeccionArea SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Equipo
   ======================================================== */
PRINT 'Creando SPs de Equipo';
IF OBJECT_ID('dbo.db_Sp_Equipo_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Equipo_Get;
IF OBJECT_ID('dbo.db_Sp_Equipo_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Equipo_Set;
IF OBJECT_ID('dbo.db_Sp_Equipo_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Equipo_Del;
IF OBJECT_ID('dbo.db_Sp_Equipo_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Equipo_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener equipos con filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Equipo_Get
    @Id VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @IdComp VARCHAR(36) = NULL,
    @Marca VARCHAR(255) = NULL,
    @NSerie VARCHAR(255) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Descripcion, IdComp, Modelo, NSerie, Fabricante, Marca, Funcion,
           Peso, Cilindraje, Potencia, Ancho, Alto, Largo, Capacidad, AnioFabricacion,
           Caracteristicas, Seccion, Estado, Eliminado, Fecha_log
    FROM dbo.Equipo
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@Marca IS NULL OR Marca LIKE '%' + @Marca + '%')
      AND (@NSerie IS NULL OR NSerie LIKE '%' + @NSerie + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar equipo (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Equipo_Set
    @Id VARCHAR(36),
    @Nombre VARCHAR(255),
    @Descripcion VARCHAR(MAX),
    @IdComp VARCHAR(36),
    @Modelo VARCHAR(255),
    @NSerie VARCHAR(255),
    @Fabricante VARCHAR(255),
    @Marca VARCHAR(255),
    @Funcion VARCHAR(MAX),
    @Peso INT,
    @Cilindraje INT,
    @Potencia INT,
    @Ancho INT,
    @Alto INT,
    @Largo INT,
    @Capacidad INT,
    @AnioFabricacion INT,
    @Caracteristicas VARCHAR(MAX),
    @Seccion VARCHAR(255),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Equipo (Id, Nombre, Descripcion, IdComp, Modelo, NSerie, Fabricante, Marca, Funcion,
                                Peso, Cilindraje, Potencia, Ancho, Alto, Largo, Capacidad, AnioFabricacion,
                                Caracteristicas, Seccion, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @Nombre, @Descripcion, @IdComp, @Modelo, @NSerie, @Fabricante, @Marca, @Funcion,
                @Peso, @Cilindraje, @Potencia, @Ancho, @Alto, @Largo, @Capacidad, @AnioFabricacion,
                @Caracteristicas, @Seccion, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Equipo
        SET Nombre = @Nombre,
            Descripcion = @Descripcion,
            IdComp = @IdComp,
            Modelo = @Modelo,
            NSerie = @NSerie,
            Fabricante = @Fabricante,
            Marca = @Marca,
            Funcion = @Funcion,
            Peso = @Peso,
            Cilindraje = @Cilindraje,
            Potencia = @Potencia,
            Ancho = @Ancho,
            Alto = @Alto,
            Largo = @Largo,
            Capacidad = @Capacidad,
            AnioFabricacion = @AnioFabricacion,
            Caracteristicas = @Caracteristicas,
            Seccion = @Seccion,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Eliminado lógico de equipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Equipo_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Equipo SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Equipo WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar equipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Equipo_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Equipo SET Estado = @Estado WHERE Id = @Id;
END
GO

PRINT 'SP_Unified_Core finalizado correctamente';
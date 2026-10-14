-- ===================================================================================
-- Users y User_Cred
-- ===================================================================================

CREATE OR ALTER PROCEDURE dbo.db_Sp_Users_Get
    @Id             VARCHAR(36)  = NULL,
    @Nombre         VARCHAR(255) = NULL,
    @Identificacion BIGINT       = NULL,
    @Correo         VARCHAR(255) = NULL,
    @IdComp         VARCHAR(36)  = NULL,
    @IdCuad         VARCHAR(36)  = NULL,
    @IdEsp          VARCHAR(36)  = NULL,
    @Rol            VARCHAR(60)  = NULL,
    @Estado         BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.Id, u.Nombre, u.Apellido, u.Identificacion, u.Correo, u.IdComp, co.Nombre AS CompNombre,
           u.IdCuad, cu.Nombre AS CuadNombre, u.IdEsp, es.Nombre AS EspNombre, u.Cargo, u.Rol,
           cr.Usuario, CAST(CASE WHEN cr.Id IS NULL THEN 0 ELSE 1 END AS BIT) AS TieneCredencial,
           u.Estado, u.Fecha_log
    FROM dbo.Users u
    LEFT JOIN dbo.Comp co ON co.Id = u.IdComp
    LEFT JOIN dbo.Cuad cu ON cu.Id = u.IdCuad
    LEFT JOIN dbo.Esp es  ON es.Id = u.IdEsp
    LEFT JOIN dbo.User_Cred cr ON cr.IdUser = u.Id AND cr.Eliminado = 0
    WHERE u.Eliminado = 0
      AND (@Id IS NULL OR u.Id = @Id)
      AND (@Nombre IS NULL OR u.Nombre LIKE '%' + @Nombre + '%' OR u.Apellido LIKE '%' + @Nombre + '%')
      AND (@Identificacion IS NULL OR u.Identificacion = @Identificacion)
      AND (@Correo IS NULL OR u.Correo LIKE '%' + @Correo + '%')
      AND (@IdComp IS NULL OR u.IdComp = @IdComp)
      AND (@IdCuad IS NULL OR u.IdCuad = @IdCuad)
      AND (@IdEsp IS NULL OR u.IdEsp = @IdEsp)
      AND (@Rol IS NULL OR u.Rol = @Rol)
      AND (@Estado IS NULL OR u.Estado = @Estado)
    ORDER BY u.Nombre, u.Apellido;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Users_Set
    @Id             VARCHAR(36),
    @Nombre         VARCHAR(255),
    @Apellido       VARCHAR(255),
    @Identificacion BIGINT,
    @Correo         VARCHAR(255),
    @IdComp         VARCHAR(36),
    @IdCuad         VARCHAR(36) = NULL,
    @IdEsp          VARCHAR(36) = NULL,
    @Cargo          VARCHAR(60),
    @Rol            VARCHAR(60),
    @Estado         BIT,
    @Operacion      CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Users WHERE Identificacion = @Identificacion AND IdComp = @IdComp AND Eliminado = 0)
            THROW 50409, 'Ya existe un usuario con esa identificación en la compañía.', 1;
        INSERT INTO dbo.Users (Id, Nombre, Apellido, Identificacion, Correo, IdComp, IdCuad, IdEsp, Cargo, Rol, Estado, Eliminado)
        VALUES (@Id, @Nombre, ISNULL(@Apellido, ''), @Identificacion, ISNULL(@Correo, ''), @IdComp, @IdCuad, @IdEsp,
                ISNULL(@Cargo, ''), @Rol, @Estado, 0);
    END
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Users
        SET Nombre = @Nombre, Apellido = ISNULL(@Apellido, ''), Identificacion = @Identificacion,
            Correo = ISNULL(@Correo, ''), IdCuad = @IdCuad, IdEsp = @IdEsp, Cargo = ISNULL(@Cargo, ''),
            Rol = @Rol, Estado = @Estado
        WHERE Id = @Id AND IdComp = @IdComp AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Users_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Users SET Eliminado = 1 WHERE Id = @Id;
    UPDATE dbo.User_Cred SET Eliminado = 1 WHERE IdUser = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Users_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Users SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- User_Cred
------------------------------------------------------------
-- Login: coincidencia EXACTA de usuario; devuelve datos del usuario para el token.
CREATE OR ALTER PROCEDURE dbo.db_Sp_User_Cred_Login @Usuario VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP 1 cr.Id AS IdCred, cr.Contrasenia, u.Id AS IdUser, u.Nombre, u.Apellido, u.Rol, u.IdComp
    FROM dbo.User_Cred cr
    JOIN dbo.Users u ON u.Id = cr.IdUser AND u.Eliminado = 0 AND u.Estado = 1
    WHERE cr.Usuario = @Usuario AND cr.Eliminado = 0 AND cr.Estado = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_User_Cred_Get
    @Id     VARCHAR(36) = NULL,
    @IdUser VARCHAR(36) = NULL,
    @IdComp VARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    -- Nunca devuelve la contraseña
    SELECT cr.Id, cr.Usuario, cr.IdUser, cr.Estado, cr.Fecha_log
    FROM dbo.User_Cred cr
    JOIN dbo.Users u ON u.Id = cr.IdUser AND u.Eliminado = 0
    WHERE cr.Eliminado = 0
      AND (@Id IS NULL OR cr.Id = @Id)
      AND (@IdUser IS NULL OR cr.IdUser = @IdUser)
      AND (@IdComp IS NULL OR u.IdComp = @IdComp);
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_User_Cred_Set
    @Id          VARCHAR(36),
    @Usuario     VARCHAR(255),
    @Contrasenia VARCHAR(255) = NULL,   -- hash BCrypt; NULL en 'A' = no cambia
    @IdUser      VARCHAR(36),
    @Estado      BIT,
    @Operacion   CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.User_Cred WHERE IdUser = @IdUser AND Eliminado = 0)
            THROW 50409, 'El usuario ya tiene credenciales asignadas.', 1;
        IF EXISTS (SELECT 1 FROM dbo.User_Cred WHERE Usuario = @Usuario AND Eliminado = 0)
            THROW 50409, 'El nombre de usuario ya está en uso.', 1;
        INSERT INTO dbo.User_Cred (Id, Usuario, Contrasenia, IdUser, Estado, Eliminado)
        VALUES (@Id, @Usuario, @Contrasenia, @IdUser, @Estado, 0);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.User_Cred WHERE Usuario = @Usuario AND Id <> @Id AND Eliminado = 0)
            THROW 50409, 'El nombre de usuario ya está en uso.', 1;
        UPDATE dbo.User_Cred
        SET Usuario = @Usuario, Contrasenia = ISNULL(@Contrasenia, Contrasenia), Estado = @Estado
        WHERE Id = @Id AND Eliminado = 0;
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_User_Cred_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.User_Cred SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_User_Cred_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.User_Cred SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

DROP PROCEDURE IF EXISTS dbo.db_Sp_User_Cred_Validate;
GO

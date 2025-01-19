-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2025/01/19
-- Description: creacion de los procedimientos almacenados
-- para la tabla usuarios de la DB DB_SYNC_USER
-- ========================================================

PRINT 'Creacion procedimientos tabla Usuarios'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_Users%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_Users_Get
	DROP PROCEDURE dbo.db_Sp_Users_Set
	DROP PROCEDURE dbo.db_Sp_Users_Det
	DROP PROCEDURE dbo.db_Sp_Users_Active
END


-- sp para obtener los datos
PRINT 'Creacion procedimiento usuario Get '
GO
CREATE PROCEDURE dbo.db_Sp_Users_Get
    @Id                               VARCHAR(36),
    @Nombre                           VARCHAR(40),
    @Apellido						  VARCHAR(40),
    @Identificacion                   BIGINT,
    @Correo						      VARCHAR(60),    
	@IdComp                           VARCHAR(36),
    @IdCuad                           VARCHAR(36),
    @IdEsp                            VARCHAR(36),
	@Cargo                            VARCHAR(60),
    @Rol                              VARCHAR(200),
    @Estado                           INT 
AS 
BEGIN
    SELECT Id, Nombre, Apellido, Identificacion, Correo, IdComp, IdCuad, IdEsp, Cargo, Rol, Estado, Fecha_log     
    FROM dbo.Users
    WHERE Id = CASE WHEN ISNULL(@Id,'')='' THEN Id ELSE @Id END
    AND Nombre LIKE CASE WHEN ISNULL(@Nombre,'')='' THEN Nombre ELSE '%'+@Nombre+'%' END    
    AND Apellido LIKE CASE WHEN ISNULL(@Apellido,'')='' THEN Apellido ELSE '%'+@Apellido+'%' END
    AND Identificacion LIKE CASE WHEN ISNULL(@Identificacion,0)=0 THEN Identificacion ELSE '%'+@Identificacion+'%' END
	AND Correo LIKE CASE WHEN ISNULL(@Correo,'')='' THEN Correo ELSE '%'+@Correo+'%' END
	AND IdComp LIKE CASE WHEN ISNULL(@IdComp,'')='' THEN IdComp ELSE '%'+@IdComp+'%' END
    AND IdCuad LIKE CASE WHEN ISNULL(@IdCuad,'')='' THEN IdCuad ELSE '%'+@IdCuad+'%' END
    AND IdEsp LIKE CASE WHEN ISNULL(@IdEsp,'')='' THEN IdEsp ELSE '%'+@IdEsp+'%' END
    AND Cargo LIKE CASE WHEN ISNULL(@Cargo,'')='' THEN Cargo ELSE '%'+@Cargo+'%' END
	AND Rol LIKE CASE WHEN ISNULL(@Rol,'')='' THEN Rol ELSE '%'+@Rol+'%' END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Eliminado = 0
END


-- sp para insertar y actualizar
GO
PRINT'Creacion procedimiento usuario Set '
GO
CREATE PROCEDURE dbo.db_Sp_Users_Set
    @Id                 VARCHAR(36),
    @Nombre             VARCHAR(40),
    @Apellido           VARCHAR(40),
    @Identificacion     BIGINT,
    @Correo             VARCHAR(60),
    @IdComp             VARCHAR(36),
    @IdCuad             VARCHAR(36),
    @IdEsp              VARCHAR(36),
    @Cargo              VARCHAR(60),
    @Rol                VARCHAR(200),
    @Estado             BIT,
    @Operacion          VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Users(Id, Nombre, Apellido, Identificacion, Correo, IdComp, IdCuad, IdEsp, Cargo, Rol, Estado, Eliminado, Fecha_log)
        VALUES(@Id, @Nombre, @Apellido, @Identificacion, @Correo, @IdComp, @IdCuad, @IdEsp, @Cargo, @Rol, @Estado, 0, GETDATE());
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Users
        SET Nombre         = @Nombre,
            Apellido       = @Apellido,
            Identificacion = @Identificacion,
            Correo         = @Correo,
            IdComp         = @IdComp,
            IdCuad         = @IdCuad,
            IdEsp          = @IdEsp,
            Cargo          = @Cargo,
            Rol            = @Rol,
            Estado         = @Estado
        WHERE Id           = @Id;
    END
END;
GO


-- sp para eliminar
GO
PRINT 'Creacion procedimiento usuario Del '
GO
CREATE PROCEDURE dbo.db_Sp_Users_Del
	@Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.Users
    SET Eliminado = 1
    WHERE Id = @Id;
	
	-- Obtiene el estado "Eliminado" despues de la actualizacion 
    SELECT Eliminado
    FROM dbo.Users
    WHERE Id = @Id;    
END 


-- sp para activar o desactivar
GO
PRINT 'Creacion procedimiento usuario Active '
GO
CREATE PROCEDURE dbo.db_Sp_Users_Active
	@Id VARCHAR(36),
	@Estado BIT
AS
BEGIN   
    UPDATE dbo.Users
        SET Estado = @Estado           
        WHERE Id = @Id;
END;


-- ========================================================
-- Author:		Mario Beltran
-- Create Date: 2025/01/19
-- Description: creacion de los procedimientos almacenados
-- para la tabla User_Cred de la DB DB_SYNC_USER
-- ========================================================

PRINT 'Creacion procedimientos tabla User_Cred'
IF EXISTS(SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'db_Sp_User_Cred%')
BEGIN
    DROP PROCEDURE dbo.db_Sp_User_Cred_Get
    DROP PROCEDURE dbo.db_Sp_User_Cred_Set
    DROP PROCEDURE dbo.db_Sp_User_Cred_Del
    DROP PROCEDURE dbo.db_Sp_User_Cred_Active
    DROP PROCEDURE dbo.db_Sp_User_Cred_Validate
END

-- sp para obtener los datos
PRINT 'Creacion procedimiento User_Cred Get '
GO
CREATE PROCEDURE dbo.db_Sp_User_Cred_Get
    @IdUserCred VARCHAR(36),
    @User VARCHAR(255),
    @Estado INT
AS 
BEGIN
    SELECT Id, Usuario, Contrasenia, IdUser, Estado, Fecha_log     
    FROM dbo.User_Cred
    WHERE Id = CASE WHEN ISNULL(@IdUserCred,'')='' THEN Id ELSE @IdUserCred END
    AND Usuario LIKE CASE WHEN ISNULL(@User,'')='' THEN Usuario ELSE '%'+@User+'%' END
    AND Estado = CASE WHEN ISNULL(@Estado,0) = 1 THEN 1 ELSE 0 END
    AND Eliminado = 0
END


-- sp para insertar y actualizar
GO
PRINT 'Creacion procedimiento User_Cred Set '
GO
CREATE PROCEDURE dbo.db_Sp_User_Cred_Set
    @Id              VARCHAR(36),
    @Usuario         VARCHAR(255),
    @Contrasenia     VARCHAR(255),
    @IdUser          VARCHAR(36),
    @Estado          BIT,
    @Operacion       VARCHAR(1)
AS
BEGIN
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.User_Cred(Id, Usuario, Contrasenia, IdUser, Estado, Fecha_log, Eliminado)
        VALUES(@Id, @Usuario, @Contrasenia, @IdUser, @Estado, DEFAULT, 0)
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.User_Cred
        SET Usuario = @Usuario, Contrasenia = @Contrasenia, IdUser = @IdUser, Estado = @Estado
        WHERE Id = @Id
    END
END


-- sp para eliminar
GO
PRINT 'Creacion procedimiento User_Cred Del '
GO
CREATE PROCEDURE dbo.db_Sp_User_Cred_Del
    @Id VARCHAR(36)
AS
BEGIN
    -- Actualiza el estado "Eliminado" a 1
    UPDATE dbo.User_Cred
    SET Eliminado = 1
    WHERE Id = @Id;
    
    -- Obtiene el estado "Eliminado" después de la actualización 
    SELECT Eliminado
    FROM dbo.User_Cred
    WHERE Id = @Id;    
END

-- sp para activar o desactivar
GO
PRINT 'Creacion procedimiento User_Cred Active '
GO
CREATE PROCEDURE dbo.db_Sp_User_Cred_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    UPDATE dbo.User_Cred
    SET Estado = @Estado
    WHERE Id = @Id
END
GO

-- sp para validar si un usuario ya tiene credenciales 
PRINT 'Creacion procedimiento Validate Credencial '
GO
CREATE PROCEDURE dbo.db_Sp_User_Cred_Validate
    @IdUsuario VARCHAR(36)
AS 
BEGIN
    DECLARE @Resultado BIT;

    -- Verifica si existe el usuario que cumple con las condiciones
    SELECT @Resultado = CASE 
                            WHEN EXISTS (
                                SELECT 1    
                                FROM dbo.User_Cred
                                WHERE IdUser = CASE WHEN ISNULL(@IdUsuario, '') = '' THEN IdUser ELSE @IdUsuario END  
                                AND Estado = 1
                                AND Eliminado = 0
                            ) 
                            THEN 1 
                            ELSE 0 
                        END;

    -- Retorna el resultado
    SELECT @Resultado AS EsValido;
END
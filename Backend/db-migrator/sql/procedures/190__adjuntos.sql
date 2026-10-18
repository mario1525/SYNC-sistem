-- ===================================================================================
-- Adjunto (fotos de equipos, evidencias de ejecución y fotos de fallas)
-- ===================================================================================

-- Metadatos (sin los bytes)
CREATE OR ALTER PROCEDURE dbo.db_Sp_Adjunto_Get
    @Id        VARCHAR(36) = NULL,
    @IdComp    VARCHAR(36) = NULL,
    @Entidad   VARCHAR(20) = NULL,
    @IdEntidad VARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdComp, Entidad, IdEntidad, Ref, Nombre, ContentType, Tamano, IdUser, Fecha
    FROM dbo.Adjunto
    WHERE Eliminado = 0
      AND (@Id IS NULL OR Id = @Id)
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@Entidad IS NULL OR Entidad = @Entidad)
      AND (@IdEntidad IS NULL OR IdEntidad = @IdEntidad)
    ORDER BY Fecha;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Adjunto_Datos
    @Id     VARCHAR(36),
    @IdComp VARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Nombre, ContentType, Datos
    FROM dbo.Adjunto
    WHERE Id = @Id AND Eliminado = 0 AND (@IdComp IS NULL OR IdComp = @IdComp);
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Adjunto_Set
    @Id          VARCHAR(36),
    @IdComp      VARCHAR(36),
    @Entidad     VARCHAR(20),
    @IdEntidad   VARCHAR(36),
    @Ref         VARCHAR(36) = NULL,
    @Nombre      VARCHAR(255),
    @ContentType VARCHAR(100),
    @Datos       VARBINARY(MAX),
    @IdUser      VARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Adjunto (Id, IdComp, Entidad, IdEntidad, Ref, Nombre, ContentType, Tamano, Datos, IdUser, Eliminado)
    VALUES (@Id, @IdComp, @Entidad, @IdEntidad, @Ref, @Nombre, @ContentType, DATALENGTH(@Datos), @Datos, @IdUser, 0);
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Adjunto_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Adjunto SET Eliminado = 1 WHERE Id = @Id;
END
GO

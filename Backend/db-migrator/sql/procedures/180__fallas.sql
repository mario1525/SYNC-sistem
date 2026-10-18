-- ===================================================================================
-- Falla (reportes de falla hechos desde campo)
-- ===================================================================================

CREATE OR ALTER PROCEDURE dbo.db_Sp_Falla_Get
    @Id          VARCHAR(36) = NULL,
    @IdComp      VARCHAR(36) = NULL,
    @IdEquipo    VARCHAR(36) = NULL,
    @EstadoFalla VARCHAR(15) = NULL,
    @Estado      BIT         = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT f.Id, f.IdComp, f.IdEquipo, e.Nombre AS EquipoNombre, e.NSerie AS EquipoSerie,
           f.Tipo, f.Prioridad, f.EquipoDetenido, f.Descripcion, f.EstadoFalla,
           f.IdUserReporta, LTRIM(RTRIM(ISNULL(u.Nombre, '') + ' ' + ISNULL(u.Apellido, ''))) AS ReportaNombre,
           f.FechaReporte, f.FechaCierre,
           (SELECT COUNT(*) FROM dbo.Adjunto ad WHERE ad.Entidad = 'Falla' AND ad.IdEntidad = f.Id AND ad.Eliminado = 0) AS NumFotos,
           f.Estado, f.Fecha_log
    FROM dbo.Falla f
    JOIN dbo.Equipo e ON e.Id = f.IdEquipo
    LEFT JOIN dbo.Users u ON u.Id = f.IdUserReporta
    WHERE f.Eliminado = 0
      AND (@Id IS NULL OR f.Id = @Id)
      AND (@IdComp IS NULL OR f.IdComp = @IdComp)
      AND (@IdEquipo IS NULL OR f.IdEquipo = @IdEquipo)
      AND (@EstadoFalla IS NULL OR f.EstadoFalla = @EstadoFalla)
      AND (@Estado IS NULL OR f.Estado = @Estado)
    ORDER BY CASE f.EstadoFalla WHEN 'Abierta' THEN 0 WHEN 'EnAtencion' THEN 1 ELSE 2 END,
             CASE f.Prioridad WHEN 'Critica' THEN 0 WHEN 'Alta' THEN 1 WHEN 'Media' THEN 2 ELSE 3 END,
             f.FechaReporte DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Falla_Set
    @Id             VARCHAR(36),
    @IdComp         VARCHAR(36),
    @IdEquipo       VARCHAR(36),
    @Tipo           VARCHAR(30),
    @Prioridad      VARCHAR(10),
    @EquipoDetenido BIT,
    @Descripcion    VARCHAR(MAX) = NULL,
    @EstadoFalla    VARCHAR(15),
    @IdUserReporta  VARCHAR(36)  = NULL,
    @Estado         BIT,
    @Operacion      CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.Falla (Id, IdComp, IdEquipo, Tipo, Prioridad, EquipoDetenido, Descripcion,
                               EstadoFalla, IdUserReporta, Estado, Eliminado)
        VALUES (@Id, @IdComp, @IdEquipo, @Tipo, @Prioridad, @EquipoDetenido, @Descripcion,
                @EstadoFalla, @IdUserReporta, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Falla
        SET Tipo = @Tipo, Prioridad = @Prioridad, EquipoDetenido = @EquipoDetenido, Descripcion = @Descripcion,
            FechaCierre = CASE WHEN @EstadoFalla = 'Cerrada' THEN ISNULL(FechaCierre, SYSDATETIME()) ELSE NULL END,
            EstadoFalla = @EstadoFalla, Estado = @Estado
        WHERE Id = @Id AND IdComp = @IdComp AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Falla_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Falla SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Falla_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Falla SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

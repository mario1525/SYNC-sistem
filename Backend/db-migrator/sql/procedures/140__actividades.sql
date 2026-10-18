-- ===================================================================================
-- TipoActividad, Actividad y Actividad_Equipo
-- ===================================================================================

CREATE OR ALTER PROCEDURE dbo.db_Sp_TipoActividad_Get
    @Id     VARCHAR(36)  = NULL,
    @IdComp VARCHAR(36)  = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Estado BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdComp, Nombre, Descripcion, Estado, Fecha_log
    FROM dbo.TipoActividad
    WHERE Eliminado = 0
      AND (@Id IS NULL OR Id = @Id)
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
    ORDER BY Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_TipoActividad_Set
    @Id          VARCHAR(36),
    @IdComp      VARCHAR(36),
    @Nombre      VARCHAR(255),
    @Descripcion VARCHAR(MAX),
    @Estado      BIT,
    @Operacion   CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.TipoActividad (Id, IdComp, Nombre, Descripcion, Estado, Eliminado)
        VALUES (@Id, @IdComp, @Nombre, ISNULL(@Descripcion, ''), @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.TipoActividad SET Nombre = @Nombre, Descripcion = ISNULL(@Descripcion, ''), Estado = @Estado
        WHERE Id = @Id AND IdComp = @IdComp AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_TipoActividad_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TipoActividad SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_TipoActividad_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TipoActividad SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- Actividad
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_Actividad_Get
    @Id              VARCHAR(36)   = NULL,
    @IdComp          VARCHAR(36)   = NULL,
    @IdTipoActividad VARCHAR(36)   = NULL,
    @IdCuad          VARCHAR(36)   = NULL,
    @FechaDesde      SMALLDATETIME = NULL,
    @FechaHasta      SMALLDATETIME = NULL,
    @Estado          BIT           = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.Id, a.IdComp, a.Descripcion, a.IdTipoActividad, t.Nombre AS TipoActividadNombre,
           a.Ubicacion, a.FechaEjecucion, a.IdCuad, c.Nombre AS CuadNombre, a.Detalle, a.Intervalo,
           (SELECT COUNT(*) FROM dbo.Actividad_Equipo ae WHERE ae.IdActividad = a.Id AND ae.Eliminado = 0) AS NumEquipos,
           a.Estado, a.Fecha_log
    FROM dbo.Actividad a
    LEFT JOIN dbo.TipoActividad t ON t.Id = a.IdTipoActividad
    LEFT JOIN dbo.Cuad c ON c.Id = a.IdCuad
    WHERE a.Eliminado = 0
      AND (@Id IS NULL OR a.Id = @Id)
      AND (@IdComp IS NULL OR a.IdComp = @IdComp)
      AND (@IdTipoActividad IS NULL OR a.IdTipoActividad = @IdTipoActividad)
      AND (@IdCuad IS NULL OR a.IdCuad = @IdCuad)
      AND (@FechaDesde IS NULL OR a.FechaEjecucion >= @FechaDesde)
      AND (@FechaHasta IS NULL OR a.FechaEjecucion < DATEADD(DAY, 1, @FechaHasta))
      AND (@Estado IS NULL OR a.Estado = @Estado)
    ORDER BY a.FechaEjecucion DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Actividad_Set
    @Id              VARCHAR(36),
    @IdComp          VARCHAR(36),
    @Descripcion     VARCHAR(MAX),
    @IdTipoActividad VARCHAR(36),
    @Ubicacion       VARCHAR(255),
    @FechaEjecucion  SMALLDATETIME,
    @IdCuad          VARCHAR(36) = NULL,
    @Detalle         VARCHAR(MAX),
    @Intervalo       INT,
    @Estado          BIT,
    @Operacion       CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.Actividad (Id, IdComp, Descripcion, IdTipoActividad, Ubicacion, FechaEjecucion,
                                   IdCuad, Detalle, Intervalo, Estado, Eliminado)
        VALUES (@Id, @IdComp, ISNULL(@Descripcion, ''), @IdTipoActividad, ISNULL(@Ubicacion, ''),
                ISNULL(@FechaEjecucion, GETDATE()), @IdCuad, ISNULL(@Detalle, ''), @Intervalo, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Actividad
        SET Descripcion = ISNULL(@Descripcion, ''), IdTipoActividad = @IdTipoActividad,
            Ubicacion = ISNULL(@Ubicacion, ''), FechaEjecucion = ISNULL(@FechaEjecucion, FechaEjecucion),
            IdCuad = @IdCuad, Detalle = ISNULL(@Detalle, ''), Intervalo = @Intervalo, Estado = @Estado
        WHERE Id = @Id AND IdComp = @IdComp AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Actividad_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Actividad SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Actividad_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Actividad SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- ActividadEquipo (equipos intervenidos en una actividad)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_ActividadEquipo_Get
    @Id          VARCHAR(36)   = NULL,
    @IdActividad VARCHAR(36)   = NULL,
    @IdEquipo    VARCHAR(36)   = NULL,
    @IdGuia      VARCHAR(36)   = NULL,
    @IdComp      VARCHAR(36)   = NULL,
    @IdCuad      VARCHAR(36)   = NULL,
    @FechaDesde  SMALLDATETIME = NULL,
    @FechaHasta  SMALLDATETIME = NULL,
    @EstadoEjec  VARCHAR(20)   = NULL,
    @Estado      BIT           = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ae.Id, ae.IdActividad, ae.IdEquipo, e.Nombre AS EquipoNombre, e.NSerie AS EquipoSerie,
           ae.IdGuia, g.Nombre AS GuiaNombre, g.Duracion AS GuiaDuracion,
           a.FechaEjecucion, a.Descripcion AS ActividadDescripcion, t.Nombre AS TipoActividadNombre,
           a.IdCuad, c.Nombre AS CuadNombre,
           CASE WHEN s.Id IS NULL THEN NULL
                ELSE p.Nombre + ' / ' + ar.Nombre + ' / ' + s.Nombre END AS UbicacionNombre,
           ae.EstadoEjec, ae.FechaInicio, ae.FechaFin, ae.IdUserEjec, ae.Observacion,
           (SELECT COUNT(*) FROM dbo.Valid v JOIN dbo.Proced pr ON pr.Id = v.IdProced AND pr.Eliminado = 0
             WHERE pr.IdGuia = ae.IdGuia AND v.Eliminado = 0) AS TotalValid,
           (SELECT COUNT(*) FROM dbo.EjecucionValid ev JOIN dbo.Valid v ON v.Id = ev.IdValid AND v.Eliminado = 0
             WHERE ev.IdActividadEquipo = ae.Id AND ev.Cumple = 1) AS ValidCumplidas,
           ae.Estado, ae.Fecha_log
    FROM dbo.Actividad_Equipo ae
    JOIN dbo.Actividad a ON a.Id = ae.IdActividad AND a.Eliminado = 0
    JOIN dbo.Equipo e    ON e.Id = ae.IdEquipo
    LEFT JOIN dbo.Guia g ON g.Id = ae.IdGuia
    LEFT JOIN dbo.TipoActividad t ON t.Id = a.IdTipoActividad
    LEFT JOIN dbo.Cuad c ON c.Id = a.IdCuad
    LEFT JOIN dbo.UbicacionEquipo u ON u.IdEquipo = e.Id AND u.Eliminado = 0
    LEFT JOIN dbo.SeccionArea s ON s.Id = u.IdSeccionArea
    LEFT JOIN dbo.Area ar ON ar.Id = s.IdArea
    LEFT JOIN dbo.Planta p ON p.Id = ar.IdPlanta
    WHERE ae.Eliminado = 0
      AND (@Id IS NULL OR ae.Id = @Id)
      AND (@IdActividad IS NULL OR ae.IdActividad = @IdActividad)
      AND (@IdEquipo IS NULL OR ae.IdEquipo = @IdEquipo)
      AND (@IdGuia IS NULL OR ae.IdGuia = @IdGuia)
      AND (@IdComp IS NULL OR a.IdComp = @IdComp)
      AND (@IdCuad IS NULL OR a.IdCuad = @IdCuad)
      AND (@FechaDesde IS NULL OR a.FechaEjecucion >= @FechaDesde)
      AND (@FechaHasta IS NULL OR a.FechaEjecucion < DATEADD(DAY, 1, @FechaHasta))
      AND (@EstadoEjec IS NULL OR ae.EstadoEjec = @EstadoEjec)
      AND (@Estado IS NULL OR ae.Estado = @Estado)
    ORDER BY a.FechaEjecucion DESC, e.Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_ActividadEquipo_Set
    @Id          VARCHAR(36),
    @IdActividad VARCHAR(36),
    @IdEquipo    VARCHAR(36),
    @IdGuia      VARCHAR(36) = NULL,
    @Estado      BIT,
    @Operacion   CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Actividad_Equipo WHERE IdActividad = @IdActividad AND IdEquipo = @IdEquipo AND Eliminado = 0)
            THROW 50409, 'El equipo ya está incluido en la actividad.', 1;
        INSERT INTO dbo.Actividad_Equipo (Id, IdActividad, IdEquipo, IdGuia, Estado, Eliminado)
        VALUES (@Id, @IdActividad, @IdEquipo, @IdGuia, @Estado, 0);
    END
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Actividad_Equipo SET IdGuia = @IdGuia, Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_ActividadEquipo_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Actividad_Equipo SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_ActividadEquipo_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Actividad_Equipo SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

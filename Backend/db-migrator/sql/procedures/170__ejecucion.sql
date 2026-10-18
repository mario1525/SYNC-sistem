-- ===================================================================================
-- Ejecución en campo: resultado de cada verificación y estado de Actividad_Equipo
-- ===================================================================================

CREATE OR ALTER PROCEDURE dbo.db_Sp_EjecucionValid_Get
    @IdActividadEquipo VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ev.Id, ev.IdActividadEquipo, ev.IdValid, ev.Cumple, ev.Observacion, ev.IdUser, ev.Fecha
    FROM dbo.EjecucionValid ev
    WHERE ev.IdActividadEquipo = @IdActividadEquipo;
END
GO

-- Registra (o corrige) el resultado de una verificación. La primera marca inicia la ejecución.
CREATE OR ALTER PROCEDURE dbo.db_Sp_EjecucionValid_Set
    @Id                VARCHAR(36),
    @IdActividadEquipo VARCHAR(36),
    @IdValid           VARCHAR(36),
    @Cumple            BIT,
    @Observacion       VARCHAR(MAX) = NULL,
    @IdUser            VARCHAR(36)  = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRAN;

    UPDATE dbo.EjecucionValid WITH (UPDLOCK, HOLDLOCK)
    SET Cumple = @Cumple, Observacion = @Observacion, IdUser = @IdUser, Fecha = SYSDATETIME()
    WHERE IdActividadEquipo = @IdActividadEquipo AND IdValid = @IdValid;

    IF @@ROWCOUNT = 0
        INSERT INTO dbo.EjecucionValid (Id, IdActividadEquipo, IdValid, Cumple, Observacion, IdUser)
        VALUES (@Id, @IdActividadEquipo, @IdValid, @Cumple, @Observacion, @IdUser);

    UPDATE dbo.Actividad_Equipo
    SET EstadoEjec = 'EnCurso', FechaInicio = ISNULL(FechaInicio, SYSDATETIME()), IdUserEjec = ISNULL(IdUserEjec, @IdUser)
    WHERE Id = @IdActividadEquipo AND EstadoEjec = 'Pendiente';

    COMMIT;
END
GO

-- Cambia el estado de ejecución (EnCurso / Completada / Pendiente al reabrir).
CREATE OR ALTER PROCEDURE dbo.db_Sp_ActividadEquipo_Ejecucion
    @Id          VARCHAR(36),
    @EstadoEjec  VARCHAR(20),
    @IdUser      VARCHAR(36)  = NULL,
    @Observacion VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Actividad_Equipo
    SET EstadoEjec  = @EstadoEjec,
        FechaInicio = CASE WHEN @EstadoEjec = 'Pendiente' THEN NULL ELSE ISNULL(FechaInicio, SYSDATETIME()) END,
        FechaFin    = CASE WHEN @EstadoEjec = 'Completada' THEN SYSDATETIME() ELSE NULL END,
        IdUserEjec  = ISNULL(@IdUser, IdUserEjec),
        Observacion = ISNULL(@Observacion, Observacion)
    WHERE Id = @Id AND Eliminado = 0;
END
GO

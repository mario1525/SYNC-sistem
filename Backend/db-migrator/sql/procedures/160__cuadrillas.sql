-- ===================================================================================
-- Cuad (Cuadrilla), Turno y Horario (cuadrilla x turno x día de la semana)
-- ===================================================================================

CREATE OR ALTER PROCEDURE dbo.db_Sp_Cuad_Get
    @Id     VARCHAR(36)  = NULL,
    @Nombre VARCHAR(255) = NULL,
    @IdComp VARCHAR(36)  = NULL,
    @Estado BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.Id, c.Nombre, c.IdComp, c.Estado, c.Fecha_log,
           (SELECT COUNT(*) FROM dbo.Users u WHERE u.IdCuad = c.Id AND u.Eliminado = 0) AS NumIntegrantes
    FROM dbo.Cuad c
    WHERE c.Eliminado = 0
      AND (@Id IS NULL OR c.Id = @Id)
      AND (@Nombre IS NULL OR c.Nombre LIKE '%' + @Nombre + '%')
      AND (@IdComp IS NULL OR c.IdComp = @IdComp)
      AND (@Estado IS NULL OR c.Estado = @Estado)
    ORDER BY c.Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Cuad_Set
    @Id        VARCHAR(36),
    @Nombre    VARCHAR(255),
    @IdComp    VARCHAR(36),
    @Estado    BIT,
    @Operacion CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.Cuad (Id, Nombre, IdComp, Estado, Eliminado) VALUES (@Id, @Nombre, @IdComp, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Cuad SET Nombre = @Nombre, Estado = @Estado WHERE Id = @Id AND IdComp = @IdComp AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Cuad_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Cuad SET Eliminado = 1 WHERE Id = @Id;
    UPDATE dbo.Users SET IdCuad = NULL WHERE IdCuad = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Cuad_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Cuad SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- Turno
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_Turno_Get
    @Id     VARCHAR(36)  = NULL,
    @Nombre VARCHAR(255) = NULL,
    @IdComp VARCHAR(36)  = NULL,
    @Estado BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, IdComp, HoraInicio, HoraFin, Estado, Fecha_log
    FROM dbo.Turno
    WHERE Eliminado = 0
      AND (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@Estado IS NULL OR Estado = @Estado)
    ORDER BY HoraInicio;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Turno_Set
    @Id         VARCHAR(36),
    @Nombre     VARCHAR(255),
    @IdComp     VARCHAR(36),
    @HoraInicio TIME,
    @HoraFin    TIME,
    @Estado     BIT = 1,
    @Operacion  CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
        INSERT INTO dbo.Turno (Id, Nombre, IdComp, HoraInicio, HoraFin, Estado, Eliminado)
        VALUES (@Id, @Nombre, @IdComp, @HoraInicio, @HoraFin, @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Turno SET Nombre = @Nombre, HoraInicio = @HoraInicio, HoraFin = @HoraFin, Estado = @Estado
        WHERE Id = @Id AND IdComp = @IdComp AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Turno_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Turno SET Eliminado = 1 WHERE Id = @Id;
    UPDATE dbo.Horario SET Eliminado = 1 WHERE IdTurno = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Turno_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Turno SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- Horario
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_Horario_Get
    @Id        VARCHAR(36) = NULL,
    @IdTurno   VARCHAR(36) = NULL,
    @IdCuad    VARCHAR(36) = NULL,
    @DiaSemana INT         = NULL,
    @IdComp    VARCHAR(36) = NULL,
    @Estado    BIT         = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT h.Id, h.IdTurno, t.Nombre AS TurnoNombre, t.HoraInicio, t.HoraFin,
           h.IdCuad, c.Nombre AS CuadNombre, h.DiaSemana, h.EsFestivo, h.Estado, h.Fecha_log
    FROM dbo.Horario h
    JOIN dbo.Turno t ON t.Id = h.IdTurno AND t.Eliminado = 0
    JOIN dbo.Cuad c  ON c.Id = h.IdCuad AND c.Eliminado = 0
    WHERE h.Eliminado = 0
      AND (@Id IS NULL OR h.Id = @Id)
      AND (@IdTurno IS NULL OR h.IdTurno = @IdTurno)
      AND (@IdCuad IS NULL OR h.IdCuad = @IdCuad)
      AND (@DiaSemana IS NULL OR h.DiaSemana = @DiaSemana)
      AND (@IdComp IS NULL OR c.IdComp = @IdComp)
      AND (@Estado IS NULL OR h.Estado = @Estado)
    ORDER BY h.DiaSemana, t.HoraInicio;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Horario_Set
    @Id        VARCHAR(36),
    @IdTurno   VARCHAR(36),
    @IdCuad    VARCHAR(36),
    @DiaSemana INT,
    @EsFestivo BIT,
    @Estado    BIT,
    @Operacion CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @DiaSemana NOT BETWEEN 1 AND 7
        THROW 50400, 'DiaSemana debe estar entre 1 (lunes) y 7 (domingo).', 1;
    IF @Operacion = 'I'
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.Horario WHERE IdCuad = @IdCuad AND IdTurno = @IdTurno AND DiaSemana = @DiaSemana AND Eliminado = 0)
            THROW 50409, 'La cuadrilla ya tiene ese turno asignado ese día.', 1;
        INSERT INTO dbo.Horario (Id, IdTurno, IdCuad, DiaSemana, EsFestivo, Estado, Eliminado)
        VALUES (@Id, @IdTurno, @IdCuad, @DiaSemana, @EsFestivo, @Estado, 0);
    END
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Horario
        SET IdTurno = @IdTurno, IdCuad = @IdCuad, DiaSemana = @DiaSemana, EsFestivo = @EsFestivo, Estado = @Estado
        WHERE Id = @Id AND Eliminado = 0;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Horario_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Horario SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Horario_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Horario SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

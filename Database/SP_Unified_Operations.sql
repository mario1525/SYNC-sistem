-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Procedimientos almacenados unificados para DB_SYNC (paquete operaciones)
--              Tablas cubiertas:
--              Cuad, Turno, Horario, TipoActividad, Actividad, Actividad_Equipo,
--              Guia, Proced, Valid, Guia_Equipo, UbicacionEquipo.
--              Todos los objetos se crean con idempotencia.
-- ========================================================

USE DB_SYNC;
GO


/* ========================================================
   Cuad - Cuadrilla
   ======================================================== */
PRINT 'Creando SPs de Cuad';
IF OBJECT_ID('dbo.db_Sp_Cuad_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Cuad_Get;
IF OBJECT_ID('dbo.db_Sp_Cuad_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Cuad_Set;
IF OBJECT_ID('dbo.db_Sp_Cuad_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Cuad_Del;
IF OBJECT_ID('dbo.db_Sp_Cuad_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Cuad_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener cuadrillas con filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Cuad_Get
    @Id    VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @IdComp VARCHAR(36) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, IdComp, Estado, Eliminado, Fecha_log
    FROM dbo.Cuad
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar cuadrilla (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Cuad_Set
    @Id VARCHAR(36),
    @Nombre VARCHAR(255),
    @IdComp VARCHAR(36),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Cuad (Id, Nombre, IdComp, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @Nombre, @IdComp, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Cuad
        SET Nombre = @Nombre,
            IdComp = @IdComp,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica cuadrilla
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Cuad_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Cuad SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Cuad WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar cuadrilla
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Cuad_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Cuad SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Turno
   ======================================================== */
PRINT 'Creando SPs de Turno';
IF OBJECT_ID('dbo.db_Sp_Turno_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Turno_Get;
IF OBJECT_ID('dbo.db_Sp_Turno_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Turno_Set;
IF OBJECT_ID('dbo.db_Sp_Turno_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Turno_Del;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener turnos con filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Turno_Get
    @Id     VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @IdComp VARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, IdComp, HoraInicio, HoraFin, Eliminado, Fecha_log
    FROM dbo.Turno
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar turno (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Turno_Set
    @Id VARCHAR(36),
    @Nombre VARCHAR(255),
    @IdComp VARCHAR(36),
    @HoraInicio TIME,
    @HoraFin TIME,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Turno (Id, Nombre, IdComp, HoraInicio, HoraFin, Eliminado, Fecha_log)
        VALUES (@Id, @Nombre, @IdComp, @HoraInicio, @HoraFin, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Turno
        SET Nombre = @Nombre,
            IdComp = @IdComp,
            HoraInicio = @HoraInicio,
            HoraFin = @HoraFin
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica turno
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Turno_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Turno SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Turno WHERE Id = @Id;
END
GO


/* ========================================================
   Horario
   ======================================================== */
PRINT 'Creando SPs de Horario';
IF OBJECT_ID('dbo.db_Sp_Horario_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Horario_Get;
IF OBJECT_ID('dbo.db_Sp_Horario_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Horario_Set;
IF OBJECT_ID('dbo.db_Sp_Horario_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Horario_Del;
IF OBJECT_ID('dbo.db_Sp_Horario_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Horario_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener horarios con filtros
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Horario_Get
    @Id VARCHAR(36) = NULL,
    @IdTurno VARCHAR(36) = NULL,
    @IdCuad VARCHAR(36) = NULL,
    @DiaSemana INT = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdTurno, IdCuad, DiaSemana, EsFestivo, Estado, Eliminado, Fecha_log
    FROM dbo.Horario
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdTurno IS NULL OR IdTurno = @IdTurno)
      AND (@IdCuad IS NULL OR IdCuad = @IdCuad)
      AND (@DiaSemana IS NULL OR DiaSemana = @DiaSemana)
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar horario (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Horario_Set
    @Id VARCHAR(36),
    @IdTurno VARCHAR(36),
    @IdCuad VARCHAR(36),
    @DiaSemana INT,
    @EsFestivo BIT,
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Horario (Id, IdTurno, IdCuad, DiaSemana, EsFestivo, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @IdTurno, @IdCuad, @DiaSemana, @EsFestivo, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Horario
        SET IdTurno = @IdTurno,
            IdCuad = @IdCuad,
            DiaSemana = @DiaSemana,
            EsFestivo = @EsFestivo,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica horario
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Horario_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Horario SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Horario WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar horario
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Horario_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Horario SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   TipoActividad
   ======================================================== */
PRINT 'Creando SPs de TipoActividad';
IF OBJECT_ID('dbo.db_Sp_TipoActividad_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_TipoActividad_Get;
IF OBJECT_ID('dbo.db_Sp_TipoActividad_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_TipoActividad_Set;
IF OBJECT_ID('dbo.db_Sp_TipoActividad_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_TipoActividad_Del;
IF OBJECT_ID('dbo.db_Sp_TipoActividad_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_TipoActividad_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener tipos de actividad
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_TipoActividad_Get
    @Id VARCHAR(36) = NULL,
    @IdComp VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdComp, Nombre, Descripcion, Estado, Eliminado, Fecha_log
    FROM dbo.TipoActividad
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar TipoActividad (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_TipoActividad_Set
    @Id VARCHAR(36),
    @IdComp VARCHAR(36),
    @Nombre VARCHAR(255),
    @Descripcion VARCHAR(MAX),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.TipoActividad (Id, IdComp, Nombre, Descripcion, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @IdComp, @Nombre, @Descripcion, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.TipoActividad
        SET IdComp = @IdComp,
            Nombre = @Nombre,
            Descripcion = @Descripcion,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica TipoActividad
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_TipoActividad_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TipoActividad SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.TipoActividad WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar TipoActividad
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_TipoActividad_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.TipoActividad SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Actividad
   ======================================================== */
PRINT 'Creando SPs de Actividad';
IF OBJECT_ID('dbo.db_Sp_Actividad_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Actividad_Get;
IF OBJECT_ID('dbo.db_Sp_Actividad_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Actividad_Set;
IF OBJECT_ID('dbo.db_Sp_Actividad_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Actividad_Del;
IF OBJECT_ID('dbo.db_Sp_Actividad_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Actividad_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener actividades
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Actividad_Get
    @Id VARCHAR(36) = NULL,
    @IdComp VARCHAR(36) = NULL,
    @IdTipoActividad VARCHAR(36) = NULL,
    @Ubicacion VARCHAR(255) = NULL,
    @IdCuad VARCHAR(36) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdComp, Descripcion, IdTipoActividad, Ubicacion, FechaEjecucion, IdCuad, Detalle, Intervalo, Estado, Eliminado, Fecha_log
    FROM dbo.Actividad
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@IdTipoActividad IS NULL OR IdTipoActividad = @IdTipoActividad)
      AND (@Ubicacion IS NULL OR Ubicacion LIKE '%' + @Ubicacion + '%')
      AND (@IdCuad IS NULL OR IdCuad = @IdCuad)
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar actividad (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Actividad_Set
    @Id VARCHAR(36),
    @IdComp VARCHAR(36),
    @Descripcion VARCHAR(MAX),
    @IdTipoActividad VARCHAR(36),
    @Ubicacion VARCHAR(255),
    @FechaEjecucion SMALLDATETIME,
    @IdCuad VARCHAR(36),
    @Detalle VARCHAR(MAX),
    @Intervalo INT,
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Actividad (Id, IdComp, Descripcion, IdTipoActividad, Ubicacion, FechaEjecucion, IdCuad, Detalle, Intervalo, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @IdComp, @Descripcion, @IdTipoActividad, @Ubicacion, @FechaEjecucion, @IdCuad, @Detalle, @Intervalo, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Actividad
        SET IdComp = @IdComp,
            Descripcion = @Descripcion,
            IdTipoActividad = @IdTipoActividad,
            Ubicacion = @Ubicacion,
            FechaEjecucion = @FechaEjecucion,
            IdCuad = @IdCuad,
            Detalle = @Detalle,
            Intervalo = @Intervalo,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica Actividad
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Actividad_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Actividad SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Actividad WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar Actividad
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Actividad_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Actividad SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Actividad_Equipo
   ======================================================== */
PRINT 'Creando SPs de Actividad_Equipo';
IF OBJECT_ID('dbo.db_Sp_ActividadEquipo_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_ActividadEquipo_Get;
IF OBJECT_ID('dbo.db_Sp_ActividadEquipo_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_ActividadEquipo_Set;
IF OBJECT_ID('dbo.db_Sp_ActividadEquipo_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_ActividadEquipo_Del;
IF OBJECT_ID('dbo.db_Sp_ActividadEquipo_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_ActividadEquipo_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener Actividad_Equipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_ActividadEquipo_Get
    @Id VARCHAR(36) = NULL,
    @IdActividad VARCHAR(36) = NULL,
    @IdEquipo VARCHAR(36) = NULL,
    @IdGuia VARCHAR(36) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdActividad, IdEquipo, IdGuia, Estado, Eliminado, Fecha_log
    FROM dbo.Actividad_Equipo
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdActividad IS NULL OR IdActividad = @IdActividad)
      AND (@IdEquipo IS NULL OR IdEquipo = @IdEquipo)
      AND (@IdGuia IS NULL OR IdGuia = @IdGuia)
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar Actividad_Equipo (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_ActividadEquipo_Set
    @Id VARCHAR(36),
    @IdActividad VARCHAR(36),
    @IdEquipo VARCHAR(36),
    @IdGuia VARCHAR(36),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Actividad_Equipo (Id, IdActividad, IdEquipo, IdGuia, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @IdActividad, @IdEquipo, @IdGuia, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Actividad_Equipo
        SET IdActividad = @IdActividad,
            IdEquipo = @IdEquipo,
            IdGuia = @IdGuia,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica Actividad_Equipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_ActividadEquipo_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Actividad_Equipo SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Actividad_Equipo WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar Actividad_Equipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_ActividadEquipo_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Actividad_Equipo SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Guia
   ======================================================== */
PRINT 'Creando SPs de Guia';
IF OBJECT_ID('dbo.db_Sp_Guia_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Guia_Get;
IF OBJECT_ID('dbo.db_Sp_Guia_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Guia_Set;
IF OBJECT_ID('dbo.db_Sp_Guia_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Guia_Del;
IF OBJECT_ID('dbo.db_Sp_Guia_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Guia_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener guías
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Guia_Get
    @Id VARCHAR(36) = NULL,
    @IdComp VARCHAR(36) = NULL,
    @IdEsp VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Nombre, Descripcion, Proceso, Inspeccion, Herramientas, IdComp, IdEsp,
           SeguridadInd, SeguridadAmb, Intervalo, Importante, Insumos, Personal, Duracion,
           Logistica, Situacion, Notas, CreatedBy, UpdatedBy, FechaUpdate, Estado, Eliminado, Fecha_log
    FROM dbo.Guia
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdComp IS NULL OR IdComp = @IdComp)
      AND (@IdEsp IS NULL OR IdEsp = @IdEsp)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar guía (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Guia_Set
    @Id VARCHAR(36),
    @Nombre VARCHAR(255),
    @Descripcion VARCHAR(MAX),
    @Proceso VARCHAR(MAX),
    @Inspeccion VARCHAR(MAX),
    @Herramientas VARCHAR(MAX),
    @IdComp VARCHAR(36),
    @IdEsp VARCHAR(36),
    @SeguridadInd VARCHAR(MAX),
    @SeguridadAmb VARCHAR(MAX),
    @Intervalo INT,
    @Importante VARCHAR(MAX),
    @Insumos VARCHAR(MAX),
    @Personal INT,
    @Duracion INT,
    @Logistica VARCHAR(MAX),
    @Situacion VARCHAR(255),
    @Notas VARCHAR(MAX),
    @UpdatedBy VARCHAR(36),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Guia (Id, Nombre, Descripcion, Proceso, Inspeccion, Herramientas, IdComp, IdEsp,
            SeguridadInd, SeguridadAmb, Intervalo, Importante, Insumos, Personal, Duracion, Logistica,
            Situacion, Notas, CreatedBy, UpdatedBy, FechaUpdate, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @Nombre, @Descripcion, @Proceso, @Inspeccion, @Herramientas, @IdComp, @IdEsp,
            @SeguridadInd, @SeguridadAmb, @Intervalo, @Importante, @Insumos, @Personal, @Duracion, @Logistica,
            @Situacion, @Notas, @UpdatedBy, @UpdatedBy, DEFAULT, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Guia
        SET Nombre = @Nombre,
            Descripcion = @Descripcion,
            Proceso = @Proceso,
            Inspeccion = @Inspeccion,
            Herramientas = @Herramientas,
            IdComp = @IdComp,
            IdEsp = @IdEsp,
            SeguridadInd = @SeguridadInd,
            SeguridadAmb = @SeguridadAmb,
            Intervalo = @Intervalo,
            Importante = @Importante,
            Insumos = @Insumos,
            Personal = @Personal,
            Duracion = @Duracion,
            Logistica = @Logistica,
            Situacion = @Situacion,
            Notas = @Notas,
            UpdatedBy = @UpdatedBy,
            Estado = @Estado,
            FechaUpdate = GETDATE()
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica guía
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Guia_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Guia SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Guia WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar guía
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Guia_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Guia SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Proced
   ======================================================== */
PRINT 'Creando SPs de Proced';
IF OBJECT_ID('dbo.db_Sp_Proced_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Proced_Get;
IF OBJECT_ID('dbo.db_Sp_Proced_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Proced_Set;
IF OBJECT_ID('dbo.db_Sp_Proced_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Proced_Del;
IF OBJECT_ID('dbo.db_Sp_Proced_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Proced_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener procedimientos (Proced)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Proced_Get
    @Id VARCHAR(36) = NULL,
    @IdGuia VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdGuia, Nombre, Descripcion, Estado, Eliminado, Fecha_log
    FROM dbo.Proced
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdGuia IS NULL OR IdGuia = @IdGuia)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar Proced (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Proced_Set
    @Id VARCHAR(36),
    @IdGuia VARCHAR(36),
    @Nombre VARCHAR(255),
    @Descripcion VARCHAR(MAX),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Proced (Id, IdGuia, Nombre, Descripcion, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @IdGuia, @Nombre, @Descripcion, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Proced
        SET IdGuia = @IdGuia,
            Nombre = @Nombre,
            Descripcion = @Descripcion,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica Proced
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Proced_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Proced SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Proced WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar Proced
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Proced_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Proced SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Valid
   ======================================================== */
PRINT 'Creando SPs de Valid';
IF OBJECT_ID('dbo.db_Sp_Valid_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Valid_Get;
IF OBJECT_ID('dbo.db_Sp_Valid_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Valid_Set;
IF OBJECT_ID('dbo.db_Sp_Valid_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Valid_Del;
IF OBJECT_ID('dbo.db_Sp_Valid_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_Valid_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener Valid (validaciones de Proced)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Valid_Get
    @Id VARCHAR(36) = NULL,
    @IdProced VARCHAR(36) = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdProced, Nombre, Descripcion, Estado, Eliminado, Fecha_log
    FROM dbo.Valid
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdProced IS NULL OR IdProced = @IdProced)
      AND (@Nombre IS NULL OR Nombre LIKE '%' + @Nombre + '%')
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar Valid (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Valid_Set
    @Id VARCHAR(36),
    @IdProced VARCHAR(36),
    @Nombre VARCHAR(255),
    @Descripcion VARCHAR(MAX),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Valid (Id, IdProced, Nombre, Descripcion, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @IdProced, @Nombre, @Descripcion, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Valid
        SET IdProced = @IdProced,
            Nombre = @Nombre,
            Descripcion = @Descripcion,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica Valid
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Valid_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Valid SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Valid WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar Valid
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_Valid_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Valid SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   Guia_Equipo
   ======================================================== */
PRINT 'Creando SPs de Guia_Equipo';
IF OBJECT_ID('dbo.db_Sp_GuiaEquipo_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_GuiaEquipo_Get;
IF OBJECT_ID('dbo.db_Sp_GuiaEquipo_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_GuiaEquipo_Set;
IF OBJECT_ID('dbo.db_Sp_GuiaEquipo_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_GuiaEquipo_Del;
IF OBJECT_ID('dbo.db_Sp_GuiaEquipo_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_GuiaEquipo_Active;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener relaciones guía-equipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_GuiaEquipo_Get
    @Id VARCHAR(36) = NULL,
    @IdGuia VARCHAR(36) = NULL,
    @IdEquipo VARCHAR(36) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdGuia, IdEquipo, Estado, Eliminado, Fecha_log
    FROM dbo.Guia_Equipo
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdGuia IS NULL OR IdGuia = @IdGuia)
      AND (@IdEquipo IS NULL OR IdEquipo = @IdEquipo)
      AND (@Estado IS NULL OR Estado = @Estado)
      AND Eliminado = 0;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar guía-equipo (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_GuiaEquipo_Set
    @Id VARCHAR(36),
    @IdGuia VARCHAR(36),
    @IdEquipo VARCHAR(36),
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.Guia_Equipo (Id, IdGuia, IdEquipo, Estado, Eliminado, Fecha_log)
        VALUES (@Id, @IdGuia, @IdEquipo, @Estado, 0, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.Guia_Equipo
        SET IdGuia = @IdGuia,
            IdEquipo = @IdEquipo,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Baja lógica guía-equipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_GuiaEquipo_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Guia_Equipo SET Eliminado = 1 WHERE Id = @Id;
    SELECT Eliminado FROM dbo.Guia_Equipo WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar guía-equipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_GuiaEquipo_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Guia_Equipo SET Estado = @Estado WHERE Id = @Id;
END
GO


/* ========================================================
   UbicacionEquipo
   ======================================================== */
PRINT 'Creando SPs de UbicacionEquipo';
IF OBJECT_ID('dbo.db_Sp_UbicacionEquipo_Get','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_UbicacionEquipo_Get;
IF OBJECT_ID('dbo.db_Sp_UbicacionEquipo_Set','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_UbicacionEquipo_Set;
IF OBJECT_ID('dbo.db_Sp_UbicacionEquipo_Active','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_UbicacionEquipo_Active;
IF OBJECT_ID('dbo.db_Sp_UbicacionEquipo_Del','P') IS NOT NULL DROP PROCEDURE dbo.db_Sp_UbicacionEquipo_Del;
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Obtener ubicaciones de equipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_UbicacionEquipo_Get
    @Id VARCHAR(36) = NULL,
    @IdEquipo VARCHAR(36) = NULL,
    @TipoUbicacion VARCHAR(50) = NULL, -- 'Almacen' | 'Produccion'
    @IdSeccionArea VARCHAR(36) = NULL,
    @Estado INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, IdEquipo, TipoUbicacion, IdSeccionArea, Estado, Fecha_log
    FROM dbo.UbicacionEquipo
    WHERE (@Id IS NULL OR Id = @Id)
      AND (@IdEquipo IS NULL OR IdEquipo = @IdEquipo)
      AND (@TipoUbicacion IS NULL OR TipoUbicacion = @TipoUbicacion)
      AND (@IdSeccionArea IS NULL OR IdSeccionArea = @IdSeccionArea)
      AND (@Estado IS NULL OR Estado = @Estado);
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Insertar/Actualizar ubicación equipo (I/A)
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_UbicacionEquipo_Set
    @Id VARCHAR(36),
    @IdEquipo VARCHAR(36),
    @TipoUbicacion VARCHAR(50),
    @IdSeccionArea VARCHAR(36) = NULL,
    @Estado BIT,
    @Operacion VARCHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    IF @Operacion = 'I'
    BEGIN
        INSERT INTO dbo.UbicacionEquipo (Id, IdEquipo, TipoUbicacion, IdSeccionArea, Estado, Fecha_log)
        VALUES (@Id, @IdEquipo, @TipoUbicacion, @IdSeccionArea, @Estado, DEFAULT);
    END
    ELSE IF @Operacion = 'A'
    BEGIN
        UPDATE dbo.UbicacionEquipo
        SET IdEquipo = @IdEquipo,
            TipoUbicacion = @TipoUbicacion,
            IdSeccionArea = @IdSeccionArea,
            Estado = @Estado
        WHERE Id = @Id;
    END
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Activar/Desactivar UbicacionEquipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_UbicacionEquipo_Active
    @Id VARCHAR(36),
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.UbicacionEquipo SET Estado = @Estado WHERE Id = @Id;
END
GO
-- ========================================================
-- Author:      Mario Beltran
-- Create Date: 2024/06/05
-- Description: Eliminar (baja lógica por estado=0) UbicacionEquipo
-- ========================================================
CREATE PROCEDURE dbo.db_Sp_UbicacionEquipo_Del
    @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.UbicacionEquipo SET Estado = 0 WHERE Id = @Id;
    SELECT Estado FROM dbo.UbicacionEquipo WHERE Id = @Id;
END
GO

PRINT 'SP_Unified_Operations finalizado correctamente';
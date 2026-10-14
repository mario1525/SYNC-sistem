-- ===================================================================================
-- Equipo + UbicacionEquipo
-- db_Sp_Equipo_Set guarda el equipo y su ubicación vigente en UNA transacción.
-- Un cambio de ubicación conserva el historial (la anterior queda Eliminado = 1).
-- ===================================================================================

CREATE OR ALTER PROCEDURE dbo.db_Sp_Equipo_Get
    @Id            VARCHAR(36)  = NULL,
    @Nombre        VARCHAR(255) = NULL,
    @IdComp        VARCHAR(36)  = NULL,
    @Marca         VARCHAR(255) = NULL,
    @NSerie        VARCHAR(255) = NULL,
    @IdPlanta      VARCHAR(36)  = NULL,
    @IdArea        VARCHAR(36)  = NULL,
    @IdSeccionArea VARCHAR(36)  = NULL,
    @Estado        BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT e.Id, e.Nombre, e.Descripcion, e.IdComp, e.Modelo, e.NSerie, e.Fabricante, e.Marca, e.Funcion,
           e.Peso, e.Cilindraje, e.Potencia, e.Ancho, e.Alto, e.Largo, e.Capacidad, e.AnioFabricacion,
           e.Caracteristicas, e.Estado, e.Fecha_log,
           u.Id AS IdUbicacion, u.TipoUbicacion, u.IdSeccionArea,
           s.Nombre AS SeccionNombre, a.Id AS IdArea, a.Nombre AS AreaNombre,
           p.Id AS IdPlanta, p.Nombre AS PlantaNombre, t.Nombre AS TipoAreaNombre
    FROM dbo.Equipo e
    LEFT JOIN dbo.UbicacionEquipo u ON u.IdEquipo = e.Id AND u.Eliminado = 0
    LEFT JOIN dbo.SeccionArea s ON s.Id = u.IdSeccionArea
    LEFT JOIN dbo.Area a        ON a.Id = s.IdArea
    LEFT JOIN dbo.Planta p      ON p.Id = a.IdPlanta
    LEFT JOIN dbo.TipoArea t    ON t.Id = a.TipoArea
    WHERE e.Eliminado = 0
      AND (@Id IS NULL OR e.Id = @Id)
      AND (@Nombre IS NULL OR e.Nombre LIKE '%' + @Nombre + '%')
      AND (@IdComp IS NULL OR e.IdComp = @IdComp)
      AND (@Marca IS NULL OR e.Marca LIKE '%' + @Marca + '%')
      AND (@NSerie IS NULL OR e.NSerie LIKE '%' + @NSerie + '%')
      AND (@IdPlanta IS NULL OR p.Id = @IdPlanta)
      AND (@IdArea IS NULL OR a.Id = @IdArea)
      AND (@IdSeccionArea IS NULL OR u.IdSeccionArea = @IdSeccionArea)
      AND (@Estado IS NULL OR e.Estado = @Estado)
    ORDER BY e.Nombre;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Equipo_Set
    @Id              VARCHAR(36),
    @Nombre          VARCHAR(255),
    @Descripcion     VARCHAR(MAX),
    @IdComp          VARCHAR(36),
    @Modelo          VARCHAR(255),
    @NSerie          VARCHAR(255),
    @Fabricante      VARCHAR(255),
    @Marca           VARCHAR(255),
    @Funcion         VARCHAR(MAX),
    @Peso            INT,
    @Cilindraje      INT,
    @Potencia        INT,
    @Ancho           INT,
    @Alto            INT,
    @Largo           INT,
    @Capacidad       INT,
    @AnioFabricacion INT,
    @Caracteristicas VARCHAR(MAX),
    @Estado          BIT,
    @TipoUbicacion   VARCHAR(50) = NULL,   -- 'Produccion' | 'Almacen' | NULL (sin cambio)
    @IdSeccionArea   VARCHAR(36) = NULL,
    @Operacion       CHAR(1)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRAN;

    IF @Operacion = 'I'
        INSERT INTO dbo.Equipo (Id, Nombre, Descripcion, IdComp, Modelo, NSerie, Fabricante, Marca, Funcion,
                                Peso, Cilindraje, Potencia, Ancho, Alto, Largo, Capacidad, AnioFabricacion,
                                Caracteristicas, Estado, Eliminado)
        VALUES (@Id, @Nombre, ISNULL(@Descripcion, ''), @IdComp, ISNULL(@Modelo, ''), ISNULL(@NSerie, ''),
                ISNULL(@Fabricante, ''), ISNULL(@Marca, ''), ISNULL(@Funcion, ''),
                @Peso, @Cilindraje, @Potencia, @Ancho, @Alto, @Largo, @Capacidad, @AnioFabricacion,
                ISNULL(@Caracteristicas, ''), @Estado, 0);
    ELSE IF @Operacion = 'A'
        UPDATE dbo.Equipo
        SET Nombre = @Nombre, Descripcion = ISNULL(@Descripcion, ''), Modelo = ISNULL(@Modelo, ''),
            NSerie = ISNULL(@NSerie, ''), Fabricante = ISNULL(@Fabricante, ''), Marca = ISNULL(@Marca, ''),
            Funcion = ISNULL(@Funcion, ''), Peso = @Peso, Cilindraje = @Cilindraje, Potencia = @Potencia,
            Ancho = @Ancho, Alto = @Alto, Largo = @Largo, Capacidad = @Capacidad,
            AnioFabricacion = @AnioFabricacion, Caracteristicas = ISNULL(@Caracteristicas, ''), Estado = @Estado
        WHERE Id = @Id AND IdComp = @IdComp AND Eliminado = 0;

    -- Ubicación vigente: solo se crea un registro nuevo si cambió
    IF @TipoUbicacion IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.UbicacionEquipo
                       WHERE IdEquipo = @Id AND Eliminado = 0
                         AND TipoUbicacion = @TipoUbicacion
                         AND ISNULL(IdSeccionArea, '') = ISNULL(@IdSeccionArea, ''))
    BEGIN
        UPDATE dbo.UbicacionEquipo SET Eliminado = 1 WHERE IdEquipo = @Id AND Eliminado = 0;
        INSERT INTO dbo.UbicacionEquipo (Id, IdEquipo, TipoUbicacion, IdSeccionArea, Estado, Eliminado)
        VALUES (CONVERT(VARCHAR(36), NEWID()), @Id, @TipoUbicacion, @IdSeccionArea, 1, 0);
    END

    COMMIT;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Equipo_Del @Id VARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Equipo SET Eliminado = 1 WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE dbo.db_Sp_Equipo_Active @Id VARCHAR(36), @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Equipo SET Estado = @Estado WHERE Id = @Id AND Eliminado = 0;
END
GO

------------------------------------------------------------
-- Historial de ubicaciones de un equipo (incluye las no vigentes)
------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.db_Sp_UbicacionEquipo_Get
    @IdEquipo VARCHAR(36),
    @IdComp   VARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.Id, u.IdEquipo, u.TipoUbicacion, u.IdSeccionArea, s.Nombre AS SeccionNombre,
           a.Nombre AS AreaNombre, p.Nombre AS PlantaNombre,
           CAST(CASE WHEN u.Eliminado = 0 THEN 1 ELSE 0 END AS BIT) AS Vigente, u.Fecha_log
    FROM dbo.UbicacionEquipo u
    JOIN dbo.Equipo e ON e.Id = u.IdEquipo
    LEFT JOIN dbo.SeccionArea s ON s.Id = u.IdSeccionArea
    LEFT JOIN dbo.Area a        ON a.Id = s.IdArea
    LEFT JOIN dbo.Planta p      ON p.Id = a.IdPlanta
    WHERE u.IdEquipo = @IdEquipo
      AND (@IdComp IS NULL OR e.IdComp = @IdComp)
    ORDER BY u.Eliminado, u.Fecha_log DESC;
END
GO

DROP PROCEDURE IF EXISTS dbo.db_Sp_UbicacionEquipo_Set, dbo.db_Sp_UbicacionEquipo_Del, dbo.db_Sp_UbicacionEquipo_Active;
GO

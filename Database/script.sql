-- ===================================================================================
-- Author:		Mario Beltran
-- Create Date: 2024/05/15
--        
-- Description: Creation of the DB DB_SYNC
-- ===================================================================================


------------------------------------------------------------
--   CREACIÓN DE BASE DE DATOS
------------------------------------------------------------
PRINT 'Creating the DB'
IF NOT EXISTS(SELECT name FROM sys.databases WHERE name = 'DB_SYNC')
BEGIN
    CREATE DATABASE DB_SYNC;
END
GO

USE DB_SYNC;
GO


------------------------------------------------------------
--   TABLA: Comp
------------------------------------------------------------
PRINT 'Creating table Comp'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Comp' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Comp(
        Id            VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        Nombre        VARCHAR(255) NOT NULL DEFAULT '',
        Ciudad        VARCHAR(255) NOT NULL DEFAULT '',
        NIT           VARCHAR(255) NOT NULL DEFAULT '',
        Direccion     VARCHAR(255) NOT NULL DEFAULT '',
        Sector        VARCHAR(255) NOT NULL DEFAULT '',
        Estado        BIT NOT NULL DEFAULT 1,
        Eliminado     BIT NOT NULL DEFAULT 0,
        Fecha_log     SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

------------------------------------------------------------
--   TABLA: Esp (Especialidad)
------------------------------------------------------------
PRINT 'Creating table Esp'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Esp' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Esp(
        Id          VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        Nombre      VARCHAR(255) NOT NULL DEFAULT '',
        Descripcion VARCHAR(MAX) NOT NULL,
        Estado      BIT NOT NULL DEFAULT 1,
        Eliminado   BIT NOT NULL DEFAULT 0,
        Fecha_log   SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

------------------------------------------------------------
--   TABLA: CompEsp  (Relación Compañía – Especialidad)
------------------------------------------------------------
PRINT 'Creating table CompEsp'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'CompEsp' AND type = 'U')
BEGIN
    CREATE TABLE dbo.CompEsp (
        Id          VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdComp      VARCHAR(36) NOT NULL,
        IdEsp       VARCHAR(36) NOT NULL,
        Estado      BIT NOT NULL DEFAULT 1,
        Eliminado   BIT NOT NULL DEFAULT 0,
        Fecha_log   SMALLDATETIME DEFAULT CURRENT_TIMESTAMP,

        CONSTRAINT FK_CompEsp_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id) ON DELETE CASCADE,
        CONSTRAINT FK_CompEsp_Esp  FOREIGN KEY (IdEsp)  REFERENCES dbo.Esp(Id)  ON DELETE CASCADE
    );
END
GO

------------------------------------------------------------
--   TABLA: Cuad (Cuadrilla)
------------------------------------------------------------
PRINT 'Creating table Cuad'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Cuad' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Cuad(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',
        IdComp          VARCHAR(36) NOT NULL DEFAULT '',
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- agregar FK Cuad -> Comp
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Cuad' AND type = 'U')
AND NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Cuad') AND name = 'FKCuad_Comp')
BEGIN
    ALTER TABLE dbo.Cuad 
        ADD CONSTRAINT FKCuad_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id);
END
GO


------------------------------------------------------------
--   TABLA: Users
------------------------------------------------------------
PRINT 'Creating table Users'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Users' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Users(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',
        Apellido        VARCHAR(255) NOT NULL DEFAULT '',
        Identificacion  BIGINT        NOT NULL DEFAULT 0,
        Correo          VARCHAR(255) NOT NULL DEFAULT '',
        IdComp          VARCHAR(36) NOT NULL DEFAULT '',
        IdCuad          VARCHAR(36) NOT NULL DEFAULT '',
        IdEsp           VARCHAR(36) NOT NULL DEFAULT '',
        Cargo           VARCHAR(60) NOT NULL DEFAULT '',
        Rol             VARCHAR(60) NOT NULL DEFAULT 'Usuario',
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- agregar FKs Users -> Comp, Cuad, Esp
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Users' AND type = 'U')
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Users') AND name = 'FKUsers_Comp')
        ALTER TABLE dbo.Users ADD CONSTRAINT FKUsers_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id);
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Users') AND name = 'FKUsers_Cuad')
        ALTER TABLE dbo.Users ADD CONSTRAINT FKUsers_Cuad FOREIGN KEY (IdCuad) REFERENCES dbo.Cuad(Id);
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Users') AND name = 'FKUsers_Esp')
        ALTER TABLE dbo.Users ADD CONSTRAINT FKUsers_Esp FOREIGN KEY (IdEsp) REFERENCES dbo.Esp(Id);
END
GO


------------------------------------------------------------
--   TABLA: User_Cred
------------------------------------------------------------
PRINT 'Creating table User_Cred'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'User_Cred' AND type = 'U')
BEGIN
    CREATE TABLE dbo.User_Cred(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        Usuario         VARCHAR(255) NOT NULL DEFAULT '',
        Contrasenia     VARCHAR(255) NOT NULL DEFAULT '',
        IdUser          VARCHAR(36) NOT NULL DEFAULT '',
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- agregar FK User_Cred -> Users
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'User_Cred' AND type = 'U')
AND NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.User_Cred') AND name = 'FKUser_Cred_User')
BEGIN
    ALTER TABLE dbo.User_Cred
        ADD CONSTRAINT FKUser_Cred_User FOREIGN KEY (IdUser) REFERENCES dbo.Users(Id);
END
GO


------------------------------------------------------------
--   TABLA: Turno
------------------------------------------------------------
PRINT 'Creating table Turno'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Turno' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Turno(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',
        IdComp          VARCHAR(36) NOT NULL DEFAULT '',
        HoraInicio      TIME NOT NULL DEFAULT '00:00:00',
        HoraFin         TIME NOT NULL DEFAULT '00:00:00',
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FK Turno -> Comp
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Turno' AND type = 'U')
AND NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Turno') AND name = 'FKTurno_Comp')
BEGIN
    ALTER TABLE dbo.Turno
        ADD CONSTRAINT FKTurno_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id);
END
GO


------------------------------------------------------------
--   TABLA: Horario
------------------------------------------------------------
PRINT 'Creating table Horario'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Horario' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Horario(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdTurno         VARCHAR(36) NOT NULL DEFAULT '',
        IdCuad          VARCHAR(36) NOT NULL DEFAULT '',
        DiaSemana       INT NOT NULL DEFAULT 0,
        EsFestivo       BIT NOT NULL DEFAULT 0,
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FKs Horario -> Turno, Cuad
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Horario' AND type = 'U')
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Horario') AND name = 'FKHorario_Turno')
        ALTER TABLE dbo.Horario ADD CONSTRAINT FKHorario_Turno FOREIGN KEY (IdTurno) REFERENCES dbo.Turno(Id);
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Horario') AND name = 'FKHorario_Cuad')
        ALTER TABLE dbo.Horario ADD CONSTRAINT FKHorario_Cuad FOREIGN KEY (IdCuad) REFERENCES dbo.Cuad(Id);
END
GO


------------------------------------------------------------
--   TABLA: Planta
------------------------------------------------------------
PRINT 'Creating table Planta'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Planta' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Planta (
        Id          VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        Nombre      VARCHAR(255) NOT NULL,
        Region      VARCHAR(255) NOT NULL,
        IdComp      VARCHAR(36) NOT NULL DEFAULT '',
        Estado      BIT NOT NULL DEFAULT 1,
        Fecha_log   SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FK Planta -> Comp
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Planta' AND type = 'U')
AND NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Planta') AND name = 'FKPlanta_Comp')
BEGIN
    ALTER TABLE dbo.Planta 
        ADD CONSTRAINT FKPlanta_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id);
END
GO


------------------------------------------------------------
--   TABLA: TipoArea (ID consecutivo)
------------------------------------------------------------
PRINT 'Creating table TipoArea'
IF NOT EXISTS(
    SELECT name 
    FROM sys.objects 
    WHERE name = 'TipoArea' 
      AND type = 'U')
BEGIN
    CREATE TABLE dbo.TipoArea (
        Id          VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        Nombre      VARCHAR(255) NOT NULL,
        Descripcion VARCHAR(255) NULL,
        Estado      BIT NOT NULL DEFAULT 1,
        Fecha_log   SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO


------------------------------------------------------------
--   TABLA: Area
--   Nota: Mantuvimos Area.TipoArea como VARCHAR(36) (sin FK a TipoArea) por petición.
------------------------------------------------------------
PRINT 'Creating table Area'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Area' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Area (
        Id          VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdPlanta    VARCHAR(36) NOT NULL DEFAULT '',
        Nombre      VARCHAR(255) NOT NULL,
        TipoArea    VARCHAR(36) NOT NULL DEFAULT '',  -- conserva VARCHAR(36) por tu pedido
        Estado      BIT NOT NULL DEFAULT 1,
        Fecha_log   SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FK Area -> Planta (ON DELETE CASCADE)
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Area' AND type = 'U')
AND NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Area') AND name = 'FKArea_Planta')
BEGIN
    ALTER TABLE dbo.Area
        ADD CONSTRAINT FKArea_Planta FOREIGN KEY (IdPlanta) REFERENCES dbo.Planta(Id) ON DELETE CASCADE;
END
GO


------------------------------------------------------------
--   TABLA: SeccionArea
------------------------------------------------------------
PRINT 'Creating table SeccionArea'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'SeccionArea' AND type = 'U')
BEGIN
    CREATE TABLE dbo.SeccionArea (
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdArea          VARCHAR(36) NOT NULL DEFAULT '',
        Nombre          VARCHAR(255) NOT NULL,
        Estado          BIT NOT NULL DEFAULT 1,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FK SeccionArea -> Area
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'SeccionArea' AND type = 'U')
AND NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.SeccionArea') AND name = 'FKSeccion_Area')
BEGIN
    ALTER TABLE dbo.SeccionArea
        ADD CONSTRAINT FKSeccion_Area FOREIGN KEY (IdArea) REFERENCES dbo.Area(Id) ON DELETE CASCADE;
END
GO

-- FK Area -> TipoArea
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Area' AND type = 'U')
AND EXISTS(SELECT 1 FROM sys.objects WHERE name = 'TipoArea' AND type = 'U')
AND NOT EXISTS(
    SELECT 1 
    FROM sys.foreign_keys 
    WHERE name = 'FKArea_TipoArea'
)
BEGIN
    ALTER TABLE dbo.Area
        ADD CONSTRAINT FKArea_TipoArea 
        FOREIGN KEY (TipoArea) 
        REFERENCES dbo.TipoArea(Id);
END
GO


------------------------------------------------------------
--   TABLA: Equipo
------------------------------------------------------------
PRINT 'Creating table Equipo'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Equipo' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Equipo(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',
        Descripcion     VARCHAR(MAX) NOT NULL,
        IdComp          VARCHAR(36) NOT NULL DEFAULT '',
        Modelo          VARCHAR(255) NOT NULL DEFAULT '',
        NSerie          VARCHAR(255) NOT NULL DEFAULT '',
        Fabricante      VARCHAR(255) NOT NULL DEFAULT '',
        Marca           VARCHAR(255) NOT NULL DEFAULT '',
        Funcion         VARCHAR(MAX) NOT NULL,
        Peso            INT NOT NULL DEFAULT 0,
        Cilindraje      INT NOT NULL DEFAULT 0,
        Potencia        INT NOT NULL DEFAULT 0,
        Ancho           INT NOT NULL DEFAULT 0,
        Alto            INT NOT NULL DEFAULT 0,
        Largo           INT NOT NULL DEFAULT 0,
        Capacidad       INT NOT NULL DEFAULT 0,
        AnioFabricacion INT NOT NULL DEFAULT 0,
        Caracteristicas VARCHAR(MAX) NOT NULL,
        Seccion         VARCHAR(255) NOT NULL DEFAULT '',
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FK Equipo -> Comp
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Equipo' AND type = 'U')
AND NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Equipo') AND name = 'FKEquipo_Comp')
BEGIN
    ALTER TABLE dbo.Equipo 
        ADD CONSTRAINT FKEquipo_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id);
END
GO


------------------------------------------------------------
--   TABLA: UbicacionEquipo (solo referencia a SeccionArea para inferir Area/Planta)
------------------------------------------------------------
PRINT 'Creating table UbicacionEquipo'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'UbicacionEquipo' AND type = 'U')
BEGIN
    CREATE TABLE dbo.UbicacionEquipo (
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdEquipo        VARCHAR(36) NOT NULL DEFAULT '',
        TipoUbicacion   VARCHAR(50) NOT NULL CHECK (TipoUbicacion IN ('Almacen', 'Produccion')),
        IdSeccionArea   VARCHAR(36) NULL,
        Estado          BIT NOT NULL DEFAULT 1,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FKs UbicacionEquipo
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'UbicacionEquipo' AND type = 'U')
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.UbicacionEquipo') AND name = 'FKUbicacionEquipo_Equipo')
        ALTER TABLE dbo.UbicacionEquipo ADD CONSTRAINT FKUbicacionEquipo_Equipo FOREIGN KEY (IdEquipo) REFERENCES dbo.Equipo(Id) ON DELETE CASCADE;
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.UbicacionEquipo') AND name = 'FKUbicacionEquipo_SeccionArea')
        ALTER TABLE dbo.UbicacionEquipo ADD CONSTRAINT FKUbicacionEquipo_SeccionArea FOREIGN KEY (IdSeccionArea) REFERENCES dbo.SeccionArea(Id);
END
GO


------------------------------------------------------------
--   TABLA: Guia
------------------------------------------------------------
PRINT 'Creating table Guia'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Guia' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Guia(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',
        Descripcion     VARCHAR(MAX) NOT NULL,
        Proceso         VARCHAR(MAX) NOT NULL,
        Inspeccion      VARCHAR(MAX) NOT NULL,
        Herramientas    VARCHAR(MAX) NOT NULL,
        IdComp          VARCHAR(36) NOT NULL DEFAULT '',
        IdEsp           VARCHAR(36) NOT NULL DEFAULT '',
        SeguridadInd    VARCHAR(MAX) NOT NULL,
        SeguridadAmb    VARCHAR(MAX) NOT NULL,
        Intervalo       INT NOT NULL DEFAULT 0,
        Importante      VARCHAR(MAX) NOT NULL,
        Insumos         VARCHAR(MAX) NOT NULL,
        Personal        INT NOT NULL DEFAULT 0,
        Duracion        INT NOT NULL DEFAULT 0,
        Logistica       VARCHAR(MAX) NOT NULL,
        Situacion       VARCHAR(255) NOT NULL DEFAULT '',
        Notas           VARCHAR(MAX) NOT NULL,
        CreatedBy       VARCHAR(36) NOT NULL DEFAULT '',
        UpdatedBy       VARCHAR(36) NOT NULL DEFAULT '',
        FechaUpdate     SMALLDATETIME DEFAULT CURRENT_TIMESTAMP,
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FKs Guia -> Comp, Esp
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Guia' AND type = 'U')
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Guia') AND name = 'FKGuia_Comp')
        ALTER TABLE dbo.Guia ADD CONSTRAINT FKGuia_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id);
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Guia') AND name = 'FKGuia_Esp')
        ALTER TABLE dbo.Guia ADD CONSTRAINT FKGuia_Esp FOREIGN KEY (IdEsp) REFERENCES dbo.Esp(Id);
END
GO


------------------------------------------------------------
--   TABLA: Proced
------------------------------------------------------------
PRINT 'Creating table Proced'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Proced' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Proced(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdGuia          VARCHAR(36) NOT NULL DEFAULT '',
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',
        Descripcion     VARCHAR(MAX) NOT NULL,
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FK Proced -> Guia
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Proced' AND type = 'U')
AND NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Proced') AND name = 'FKProced_Guia')
BEGIN
    ALTER TABLE dbo.Proced 
        ADD CONSTRAINT FKProced_Guia FOREIGN KEY (IdGuia) REFERENCES dbo.Guia(Id);
END
GO


------------------------------------------------------------
--   TABLA: Valid
------------------------------------------------------------
PRINT 'Creating table Valid'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Valid' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Valid(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdProced        VARCHAR(36) NOT NULL DEFAULT '',
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',
        Descripcion     VARCHAR(MAX) NOT NULL,
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FK Valid -> Proced
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Valid' AND type = 'U')
AND NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Valid') AND name = 'FKValid_Proced')
BEGIN
    ALTER TABLE dbo.Valid 
        ADD CONSTRAINT FKValid_Proced FOREIGN KEY (IdProced) REFERENCES dbo.Proced(Id);
END
GO


------------------------------------------------------------
--   TABLA: Guia_Equipo
------------------------------------------------------------
PRINT 'Creating table Guia_Equipo'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Guia_Equipo' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Guia_Equipo(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdGuia          VARCHAR(36) NOT NULL DEFAULT '',
        IdEquipo        VARCHAR(36) NOT NULL DEFAULT '',
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FKs Guia_Equipo -> Guia, Equipo
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Guia_Equipo' AND type = 'U')
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Guia_Equipo') AND name = 'FKGuia_Equipo_Guia')
        ALTER TABLE dbo.Guia_Equipo ADD CONSTRAINT FKGuia_Equipo_Guia FOREIGN KEY (IdGuia) REFERENCES dbo.Guia(Id);
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Guia_Equipo') AND name = 'FKGuia_Equipo_Equipo')
        ALTER TABLE dbo.Guia_Equipo ADD CONSTRAINT FKGuia_Equipo_Equipo FOREIGN KEY (IdEquipo) REFERENCES dbo.Equipo(Id);
END
GO


------------------------------------------------------------
--   TABLA: TipoActividad
------------------------------------------------------------
PRINT 'Creating table TipoActividad'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'TipoActividad' AND type = 'U')
BEGIN
    CREATE TABLE dbo.TipoActividad(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdComp          VARCHAR(36) NOT NULL DEFAULT '',
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',
        Descripcion     VARCHAR(MAX) NOT NULL,
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FK TipoActividad -> Comp
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'TipoActividad' AND type = 'U')
AND NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.TipoActividad') AND name = 'FKTipoActividad_Comp')
BEGIN
    ALTER TABLE dbo.TipoActividad 
        ADD CONSTRAINT FKTipoActividad_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id);
END
GO


------------------------------------------------------------
--   TABLA: Actividad
------------------------------------------------------------
PRINT 'Creating table Actividad'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Actividad' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Actividad(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdComp          VARCHAR(36) NOT NULL DEFAULT '',
        Descripcion     VARCHAR(MAX) NOT NULL,
        IdTipoActividad VARCHAR(36) NOT NULL DEFAULT '',
        Ubicacion       VARCHAR(255) NOT NULL DEFAULT '',
        FechaEjecucion  SMALLDATETIME DEFAULT CURRENT_TIMESTAMP,
        IdCuad          VARCHAR(36) NOT NULL DEFAULT '',
        Detalle         VARCHAR(MAX) NOT NULL,
        Intervalo       INT NOT NULL DEFAULT 0,
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FKs Actividad -> TipoActividad, Comp
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Actividad' AND type = 'U')
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Actividad') AND name = 'FKActividad_TipoActividad')
        ALTER TABLE dbo.Actividad ADD CONSTRAINT FKActividad_TipoActividad FOREIGN KEY (IdTipoActividad) REFERENCES dbo.TipoActividad(Id);
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Actividad') AND name = 'FKActividad_Comp')
        ALTER TABLE dbo.Actividad ADD CONSTRAINT FKActividad_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id);
END
GO


------------------------------------------------------------
--   TABLA: Actividad_Equipo
------------------------------------------------------------
PRINT 'Creating table Actividad_Equipo'
IF NOT EXISTS(SELECT name FROM sys.objects WHERE name = 'Actividad_Equipo' AND type = 'U')
BEGIN
    CREATE TABLE dbo.Actividad_Equipo(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
        IdActividad     VARCHAR(36) NOT NULL DEFAULT '',
        IdEquipo        VARCHAR(36) NOT NULL DEFAULT '',
        IdGuia          VARCHAR(36) NOT NULL DEFAULT '',
        Estado          BIT NOT NULL DEFAULT 1,
        Eliminado       BIT NOT NULL DEFAULT 0,
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
    );
END
GO

-- FKs Actividad_Equipo -> Actividad, Equipo, Guia
IF EXISTS(SELECT 1 FROM sys.objects WHERE name = 'Actividad_Equipo' AND type = 'U')
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Actividad_Equipo') AND name = 'FKActividad_Equipo_Actividad')
        ALTER TABLE dbo.Actividad_Equipo ADD CONSTRAINT FKActividad_Equipo_Actividad FOREIGN KEY (IdActividad) REFERENCES dbo.Actividad(Id);
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Actividad_Equipo') AND name = 'FKActividad_Equipo_Equipo')
        ALTER TABLE dbo.Actividad_Equipo ADD CONSTRAINT FKActividad_Equipo_Equipo FOREIGN KEY (IdEquipo) REFERENCES dbo.Equipo(Id);
    IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('dbo.Actividad_Equipo') AND name = 'FKActividad_Equipo_Guia')
        ALTER TABLE dbo.Actividad_Equipo ADD CONSTRAINT FKActividad_Equipo_Guia FOREIGN KEY (IdGuia) REFERENCES dbo.Guia(Id);
END
GO


------------------------------------------------------------
--   ÍNDICES RECOMENDADOS (FKs)
--   (Mejoran rendimiento en joins/integridad)
------------------------------------------------------------
PRINT 'Creating recommended indexes (if not exist)'

IF OBJECT_ID('dbo.Cuad','U') IS NOT NULL
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_Cuad_IdComp' AND object_id = OBJECT_ID('dbo.Cuad'))
        CREATE INDEX IX_Cuad_IdComp ON dbo.Cuad(IdComp);
END

IF OBJECT_ID('dbo.Users','U') IS NOT NULL
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_IdComp' AND object_id = OBJECT_ID('dbo.Users'))
        CREATE INDEX IX_Users_IdComp ON dbo.Users(IdComp);
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_IdCuad' AND object_id = OBJECT_ID('dbo.Users'))
        CREATE INDEX IX_Users_IdCuad ON dbo.Users(IdCuad);
END

IF OBJECT_ID('dbo.User_Cred','U') IS NOT NULL
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_User_Cred_IdUser' AND object_id = OBJECT_ID('dbo.User_Cred'))
        CREATE INDEX IX_User_Cred_IdUser ON dbo.User_Cred(IdUser);
END

IF OBJECT_ID('dbo.Turno','U') IS NOT NULL
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_Turno_IdComp' AND object_id = OBJECT_ID('dbo.Turno'))
        CREATE INDEX IX_Turno_IdComp ON dbo.Turno(IdComp);
END

IF OBJECT_ID('dbo.Horario','U') IS NOT NULL
BEGIN
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_Horario_IdTurno' AND object_id = OBJECT_ID('dbo.Horario'))
        CREATE INDEX IX_Horario_IdTurno ON dbo.Horario(IdTurno);
    IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name = 'IX_Horario_IdCuad' AND object_id = OBJECT_ID('dbo.Horario'))
        CREATE INDEX IX_Horario_IdCuad ON dbo.Horario(IdCuad);
END

GO

PRINT 'Script execution completed.'


-- ===================================================================================
-- Author:		Mario Beltran
-- Create Date: 2024/05/15
--        
-- Description: Creation of the DB DB_SYNC_MAINTENANCE
-- ===================================================================================

PRINT 'Creating the DB'
IF NOT EXISTS(SELECT NAME FROM SYSDATABASES WHERE NAME = 'DB_SYNC_MAINTENANCE')
BEGIN
    CREATE DATABASE DB_SYNC_MAINTENANCE
END
GO  

USE DB_SYNC_MAINTENANCE
GO

-- Table Comp
PRINT 'Creating the table Comp'
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Comp')
BEGIN
    CREATE TABLE dbo.Comp(
        Id            VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',            /* Internal record ID */
        Nombre        VARCHAR(255) NOT NULL DEFAULT '',                       /* Company name */
        Ciudad        VARCHAR(255) NOT NULL DEFAULT '',                       /* City where the main office is located */
        NIT           VARCHAR(255) NOT NULL DEFAULT '',                       /* Company registration number */
        Direccion     VARCHAR(255) NOT NULL DEFAULT '',                       /* Company address */ 
        Sector        VARCHAR(255) NOT NULL DEFAULT '',                       /* Sector in which the company operates */
        Estado        BIT NOT NULL DEFAULT 1,                                 /* Status */
        Eliminado       BIT NOT NULL DEFAULT 0,                                 /* Deleted */
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP                 /* Log date */
    ) 
END
GO

-- Guia
PRINT 'creacion de la tabla Guia_ '
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Guia')
BEGIN
    CREATE TABLE dbo.Guia(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',             /*Nombre de la guia*/
        Descripcion     VARCHAR(max) NOT NULL DEFAULT '',             /*Descripcion de la guia*/
        Proceso         VARCHAR(max) NOT NULL DEFAULT '',             /*Proceso de la guia*/
        Inspeccion      VARCHAR(max) NOT NULL DEFAULT '',             /*Inspeccion de la guia*/
        Herramientas    VARCHAR(max) NOT NULL DEFAULT '',             /*Herramientas de la guia*/
        IdComp   	    VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Compania*/
        IdEsp   	    VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Especialidad*/
        SeguridadInd    VARCHAR(max) NOT NULL DEFAULT '',             /*Seguridad industrial de la guia*/
        SeguridadAmb    VARCHAR(max) NOT NULL DEFAULT '',             /*Seguridad ambiental de la guia*/
        Intervalo       INT NOT NULL DEFAULT 0,                       /*Intervalo de la guia*/ 
        Importante      VARCHAR(max) NOT NULL DEFAULT '',             /*Informacion importante de la guia*/ 
        Insumos         VARCHAR(max) NOT NULL DEFAULT '',             /*Insumos de la guia*/
        Personal        INT NOT NULL DEFAULT 0,                       /*N personas necesarias para ejecutar la guia*/
        Duracion        INT NOT NULL DEFAULT 0,                       /*Duracion de la guia en horas*/ 
        Logistica       VARCHAR(max) NOT NULL DEFAULT '',             /*Logistica de la guia*/
        Situacion       VARCHAR(255) NOT NULL DEFAULT '',             /*Situacion*/  
        Notas           VARCHAR(max) NOT NULL DEFAULT '',             /*Notas de la guia*/
        CreatedBy       VARCHAR(36) NOT NULL DEFAULT '',              /*Usuario que creo la guia*/
        UpdatedBy       VARCHAR(36) NOT NULL DEFAULT '',              /*Usuario que actualizo la guia*/ 
        FechaUpdate     SMALLDATETIME DEFAULT CURRENT_TIMESTAMP,      /*Fecha de actualizacion de la guia*/ 
        Estado			BIT NOT NULL DEFAULT 1,                       /*Estado*/
		Eliminado		BIT NOT NULL DEFAULT 0,                       /*Eliminado*/        
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
        ALTER TABLE dbo.Guia ADD CONSTRAINT
		FKGuia_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id)
END
GO

-- Procedimiento
PRINT 'creacion de la tabla Proced '
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Proced')
BEGIN
    CREATE TABLE dbo.Proced(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        IdGuia          VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Guia*/
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',             /*Nombre del procedimiento*/
        Descripcion     VARCHAR(max) NOT NULL DEFAULT '',             /*Descripcion del procedimiento*/
        Estado          BIT NOT NULL DEFAULT 1,                       /*Estado*/
        Eliminado		BIT NOT NULL DEFAULT 0,                       /*Eliminado*/        
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
        ALTER TABLE dbo.Proced ADD CONSTRAINT
        FKProced_Guia FOREIGN KEY (IdGuia) REFERENCES dbo.Guia(Id)
END

-- Validacion
PRINT 'creacion de la tabla Valid '    
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Valid')
BEGIN
    CREATE TABLE dbo.Valid(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        IdProced        VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Procedimiento*/
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',             /*Nombre de la validacion*/
        Descripcion     VARCHAR(max) NOT NULL DEFAULT '',             /*Descripcion de la validacion*/
        Estado          BIT NOT NULL DEFAULT 1,                       /*Estado*/
        Eliminado		BIT NOT NULL DEFAULT 0,                       /*Eliminado*/        
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
        ALTER TABLE dbo.Valid ADD CONSTRAINT
        FKValid_Proced FOREIGN KEY (IdProced) REFERENCES dbo.Proced(Id)
END

-- Tabla Planta (Ubicación general)
CREATE TABLE dbo.Planta (
    Id          VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
    Nombre      VARCHAR(255) NOT NULL , /* Nombre de la planta */
    Region      VARCHAR(255) NOT NULL, /* Región donde está ubicada la planta */
    IdComp   	VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Compania*/
    Estado      BIT NOT NULL DEFAULT 1, /* Activo/Inactivo */
    Fecha_log SMALLDATETIME DEFAULT CURRENT_TIMESTAMP
)ON [PRIMARY]
    ALTER TABLE dbo.Planta ADD CONSTRAINT
	FKPlanta_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id)
END

-- Tabla Área Funcional (Si el equipo está en producción)
CREATE TABLE dbo.AreaFuncional (
    Id          VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
    IdPlanta    VARCHAR(36) NOT NULL DEFAULT '', /* Relación con Planta */
    Nombre      VARCHAR(255) NOT NULL, /* Nombre del área funcional */
    Estado      BIT NOT NULL DEFAULT 1, /* Activo/Inactivo */
    Fecha_log SMALLDATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IdPlanta) REFERENCES Planta(Id) ON DELETE CASCADE
);

-- Tabla Bodega (Si el equipo está almacenado)
CREATE TABLE dbo.Bodega (
    Id          VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
    IdPlanta    VARCHAR(36) NOT NULL DEFAULT '', /* Relación con Planta */
    Nombre      VARCHAR(255) NOT NULL, /* Nombre de la bodega */
    Estado      BIT NOT NULL DEFAULT 1, /* Activo/Inactivo */
    Fecha_log SMALLDATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IdPlanta) REFERENCES Planta(Id) ON DELETE CASCADE
);

-- Tabla Sección de Bodega (Para dividir las bodegas en secciones)
CREATE TABLE dbo.SeccionBodega (
    Id          VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
    IdBodega    VARCHAR(36) NOT NULL DEFAULT '', /* Relación con Bodega */
    Nombre      VARCHAR(255) NOT NULL, /* Nombre de la sección dentro de la bodega */
    Estado      BIT NOT NULL DEFAULT 1, /* Activo/Inactivo */
    Fecha_log SMALLDATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IdBodega) REFERENCES Bodega(Id) ON DELETE CASCADE
);

-- Tabla Patio (Para almacenamiento en espacios abiertos)
CREATE TABLE dbo.Patio (
    Id          VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
    IdBodega    VARCHAR(36) NOT NULL DEFAULT '', /* Relación con Bodega */
    Nombre      VARCHAR(255) NOT NULL, /* Nombre del patio dentro de la bodega */
    Estado      BIT NOT NULL DEFAULT 1, /* Activo/Inactivo */
    Fecha_log   SMALLDATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IdBodega) REFERENCES Bodega(Id) ON DELETE CASCADE
);

-- Equipo
PRINT 'creacion de la tabla Equipo '   
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Equipo')
BEGIN
    CREATE TABLE dbo.Equipo(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',             /*Nombre del equipo*/
        Descripcion     VARCHAR(max) NOT NULL DEFAULT '',             /*Descripcion del equipo*/
        IdComp   	    VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Compania*/
        Modelo          VARCHAR(255) NOT NULL DEFAULT '',             /*Modelo del equipo*/
        NSerie          VARCHAR(255) NOT NULL DEFAULT '',             /*Numero de serie del equipo*/             /*Ubicacion del equipo*/  
        Fabricante      VARCHAR(255) NOT NULL DEFAULT '',             /*Fabricante del equipo*/
        Marca           VARCHAR(255) NOT NULL DEFAULT '',             /*Marca del equipo*/
        Funcion         VARCHAR(max) NOT NULL DEFAULT '',             /*Funcion del equipo*/
        peso            INT NOT NULL DEFAULT 0,                       /*Peso del equipo*/
        Cilindraje      INT NOT NULL DEFAULT 0,                       /*Cilindraje del equipo*/
        Potencia        INT NOT NULL DEFAULT 0,                       /*Potencia del equipo*/
        Ancho           INT NOT NULL DEFAULT 0,                       /*Ancho del equipo*/
        Alto            INT NOT NULL DEFAULT 0,                       /*Alto del equipo*/
        Largo           INT NOT NULL DEFAULT 0,                       /*Largo del equipo*/
        Capacidad       INT NOT NULL DEFAULT 0,                       /*Capacidad del equipo*/
        AnioFabricacion INT NOT NULL DEFAULT 0,                       /*Año de fabricacion del equipo*/
        Caracteristicas VARCHAR(max) NOT NULL DEFAULT '',             /*Caracteristicas del equipo*/
        Seccion         VARCHAR(255) NOT NULL DEFAULT '',             /*Seccion del equipo*/
        Estado			BIT NOT NULL DEFAULT 1,                       /*Estado*/
        Eliminado		BIT NOT NULL DEFAULT 0,                       /*Eliminado*/
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
        ALTER TABLE dbo.Equipo ADD CONSTRAINT
        FKEquipo_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id)
END

CREATE TABLE dbo.UbicacionEquipo (
    Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',
    IdEquipo        VARCHAR(36) NOT NULL DEFAULT '', /* Relación con Equipo */
    TipoUbicacion   VARCHAR(50) NOT NULL CHECK (TipoUbicacion IN ('Bodega', 'Produccion')), /* Tipo de ubicación */
    IdPlanta        VARCHAR(36) NOT NULL DEFAULT '',  /* Planta donde se encuentra */
    IdAreaFuncional VARCHAR(36) NULL, /* Si está en producción */
    IdBodega        VARCHAR(36) NULL, /* Si está en almacenamiento */
    IdSeccionBodega VARCHAR(36) NULL, /* Sección de bodega */
    IdPatio         VARCHAR(36) NULL, /* Patio (si aplica) */
    Estado          BIT NOT NULL DEFAULT 1, /* Activo/Inactivo */
    Fecha_log SMALLDATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IdEquipo) REFERENCES dbo.Equipo(Id) ON DELETE CASCADE,
    FOREIGN KEY (IdPlanta) REFERENCES dbo.Planta(Id),
    FOREIGN KEY (IdAreaFuncional) REFERENCES dbo.AreaFuncional(Id) ,
    FOREIGN KEY (IdBodega) REFERENCES dbo.Bodega(Id),
    FOREIGN KEY (IdSeccionBodega) REFERENCES dbo.SeccionBodega(Id),
    FOREIGN KEY (IdPatio) REFERENCES dbo.Patio(Id)
);

-- Guia_Equipo
PRINT 'creacion de la tabla Guia_Equipo'
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Guia_Equipo')
BEGIN
    CREATE TABLE dbo.Guia_Equipo(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        IdGuia          VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Guia*/
        IdEquipo        VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Equipo*/
        Estado			BIT NOT NULL DEFAULT 1,                       /*Estado*/
        Eliminado		BIT NOT NULL DEFAULT 0,                       /*Eliminado*/
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
        ALTER TABLE dbo.Guia_Equipo ADD CONSTRAINT
        FKGuia_Equipo_Guia FOREIGN KEY (IdGuia) REFERENCES dbo.Guia_(Id)
        ALTER TABLE dbo.Guia_Equipo ADD CONSTRAINT
        FKGuia_Equipo_Equipo FOREIGN KEY (IdEquipo) REFERENCES dbo.Equipo(Id)
END

-- TipoActividad
PRINT 'creacion de la tabla TipoActividad '
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'TipoActividad')
BEGIN
    CREATE TABLE dbo.TipoActividad(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        IdComp   	    VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Compania*/
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',             /*Nombre del tipo de actividad*/
        Descripcion     VARCHAR(max) NOT NULL DEFAULT '',             /*Descripcion del tipo de actividad*/
        Estado			BIT NOT NULL DEFAULT 1,                       /*Estado*/
        Eliminado		BIT NOT NULL DEFAULT 0,                       /*Eliminado*/
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
        ALTER TABLE dbo.TipoActividad ADD CONSTRAINT
        FKTipoActividad_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id)
END

-- Actividad    
PRINT 'creacion de la tabla Actividad '
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Actividad')
BEGIN
    CREATE TABLE dbo.Actividad(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        IdComp   	    VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Compania*/
        Descripcion     VARCHAR(max) NOT NULL DEFAULT '',             /*Descripcion de la actividad*/
        IdTipoActividad VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla TipoActividad*/ 
        Ubicacion       VARCHAR(255) NOT NULL DEFAULT '',             /*Ubicacion de la actividad*/
        FechaEjecucion  SMALLDATETIME DEFAULT CURRENT_TIMESTAMP,      /*Fecha de Ejecucion de la actividad*/
        IdCuad          VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Cuadrilla*/
        Detalle         VARCHAR(max) NOT NULL DEFAULT '',             /*Detalle de la actividad*/
        Intervalo       INT NOT NULL DEFAULT 0,                       /*Intervalo de la actividad*/
        Estado			BIT NOT NULL DEFAULT 1,                       /*Estado*/
        Eliminado		BIT NOT NULL DEFAULT 0,                       /*Eliminado*/
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
        ALTER TABLE dbo.Actividad ADD CONSTRAINT
        FKActividad_TipoActividad FOREIGN KEY (IdTipoActividad) REFERENCES dbo.TipoActividad(Id)
        ALTER TABLE dbo.Actividad ADD CONSTRAINT
        FKActividad_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id)
END

-- Actividad_Equipo
PRINT 'creacion de la tabla Actividad_Equipo '
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Actividad_Equipo')
BEGIN
    CREATE TABLE dbo.Actividad_Equipo(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        IdActividad     VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Actividad*/
        IdEquipo        VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Equipo*/
        IdGuia          VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Guia*/
        Estado			BIT NOT NULL DEFAULT 1,                       /*Estado*/
        Eliminado		BIT NOT NULL DEFAULT 0,                       /*Eliminado*/
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
        ALTER TABLE dbo.Actividad_Equipo ADD CONSTRAINT
        FKActividad_Equipo_Actividad FOREIGN KEY (IdActividad) REFERENCES dbo.Actividad(Id)
        ALTER TABLE dbo.Actividad_Equipo ADD CONSTRAINT
        FKActividad_Equipo_Equipo FOREIGN KEY (IdEquipo) REFERENCES dbo.Equipo(Id)
        ALTER TABLE dbo.Actividad_Equipo ADD CONSTRAINT
        FKActividad_Equipo_Guia FOREIGN KEY (IdGuia) REFERENCES dbo.Guia(Id)
END
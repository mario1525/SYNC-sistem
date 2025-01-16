-- ===================================================================================
-- Author:		Mario Beltran
-- Create Date: 2024/05/15
--        
-- Description: Creation of the DB DB_SYNC_USER
-- ===================================================================================

PRINT 'Creating the DB'
IF NOT EXISTS(SELECT NAME FROM SYSDATABASES WHERE NAME = 'DB_SYNC_USER')
BEGIN
    CREATE DATABASE DB_SYNC_USER
END
GO  

USE DB_SYNC_USER
GO

-- Tabla Cuad_
PRINT 'creacion de la tabla Cuad'
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Cuad')
BEGIN
    CREATE TABLE dbo.Cuad(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',             /*Nombre de la cuadrilla*/
        IdComp   	    VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Compania*/
        Estado		    BIT NOT NULL DEFAULT 1,                       /*Estado*/
		Eliminado	    BIT NOT NULL DEFAULT 0,                       /*Eliminado*/
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
    ALTER TABLE dbo.Cuad ADD CONSTRAINT
		FKCuad_Comp FOREIGN KEY (IdComp) REFERENCES dbo.Comp(Id)
END
GO

-- Tabla User_
PRINT 'creacion de la tabla User_'
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'User')
BEGIN
    CREATE TABLE dbo.User(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',             /*Nombre del usuario*/
        Apellido        VARCHAR(255) NOT NULL DEFAULT '',             /*Apellido del usuario*/
        Identificacion  BIGINT        NOT NULL DEFAULT 0,             /*numero de indentificacion del usuario*/
        Correo          VARCHAR(255) NOT NULL DEFAULT '',             /*Apellido del usuario*/
        IdComp   	    VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Compania*/
        IdCuad  	    VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Cuadrilla*/
        IdEsp   	    VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Especialidad*/
        Cargo           VARCHAR(60) NOT NULL DEFAULT '',              /*Cargo interno en la compania*/
        Rol             VARCHAR(60) NOT NULL DEFAULT 'Usuario',       /*Rol del usuario*/
        Estado		    BIT NOT NULL DEFAULT 1,                       /*Estado del Usuario*/
		Eliminado	    BIT NOT NULL DEFAULT 0,                       /*Eliminado usuario*/
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
END
GO

-- User_Cred
PRINT 'creacion de la tabla User_Cred '
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'User_Cred')
BEGIN
    CREATE TABLE dbo.User_Cred(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        Usuario         VARCHAR(255) NOT NULL DEFAULT '',            /*Nombre de Usuario para logearse*/ 
        Contrasenia     VARCHAR(255) NOT NULL DEFAULT '',           /*Contraseña*/
        IdUser         VARCHAR(36) NOT NULL DEFAULT '',           /*FK de la Tabla usuario*/
        Estado			BIT NOT NULL DEFAULT 1,                   /*Estado del Usuario*/
		Eliminado		BIT NOT NULL DEFAULT 0,                  /*Eliminado usuario*/        
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP /*log fecha*/
    ) ON [PRIMARY]
        ALTER TABLE dbo.User_Cred ADD CONSTRAINT
		FKUser_Cred FOREIGN KEY (IdUser) REFERENCES dbo.User(Id)
END
GO

-- Tabla Horario
PRINT 'creacion de la tabla Horario'
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Horario')
BEGIN
    CREATE TABLE dbo.Horario(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        IdTurno         VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Turno*/
        IdCuad   	    VARCHAR(36) NOT NULL DEFAULT '',              /*FK de la tabla Cuadrilla*/
        DiaSemana       INT NOT NULL DEFAULT 0,                       /*Dia de la semana*/
        EsFestivo       BIT NOT NULL DEFAULT 0,                       /*Es festivo*/
        Estado		    BIT NOT NULL DEFAULT 1,                       /*Estado*/
        Eliminado	    BIT NOT NULL DEFAULT 0,                       /*Eliminado*/
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]
    ALTER TABLE dbo.Horario ADD CONSTRAINT
        FKHorario_Turno FOREIGN KEY (IdTurno) REFERENCES dbo.Turno(Id),
        FKHorario_Cuad FOREIGN KEY (IdCuad) REFERENCES dbo.Cuad(Id)
END

-- Tabla Turno
PRINT 'creacion de la tabla Turno'
IF NOT EXISTS(SELECT NAME FROM sysobjects WHERE NAME = 'Turno')
BEGIN
    CREATE TABLE dbo.Turno(
        Id              VARCHAR(36) NOT NULL PRIMARY KEY DEFAULT '',  /*id interno del registro*/
        Nombre          VARCHAR(255) NOT NULL DEFAULT '',             /*Nombre del Turno*/
        HoraInicio      TIME NOT NULL DEFAULT '00:00:00',             /*Hora de inicio del turno*/
        HoraFin         TIME NOT NULL DEFAULT '00:00:00',             /*Hora de fin del turno*/ 
        Eliminado	    BIT NOT NULL DEFAULT 0,                       /*Eliminado*/
        Fecha_log       SMALLDATETIME DEFAULT CURRENT_TIMESTAMP       /*log fecha*/
    ) ON [PRIMARY]

END


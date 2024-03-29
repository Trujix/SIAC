/*
	::: BASE DE DATOS DE SISTEMA SIAC VER 1.0 :::
*/

If(db_id(N'siac') IS NULL) CREATE DATABASE siac;

USE siac;

DROP TABLE IF EXISTS dbo.clinica;
DROP TABLE IF EXISTS dbo.usuariosclinica;
DROP TABLE IF EXISTS dbo.usuarios;
DROP TABLE IF EXISTS dbo.usuarioinfo;
DROP TABLE IF EXISTS dbo.especialidades;
DROP TABLE IF EXISTS dbo.horariosmedicos;
DROP TABLE IF EXISTS dbo.citasregistros;
DROP TABLE IF EXISTS dbo.pacientes;
DROP TABLE IF EXISTS dbo.pagosconsultas;

CREATE TABLE [dbo].[clinica](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave] [varchar](200) NOT NULL,
	[token] [varchar](200) NOT NULL,
	[idnotificacion] [varchar](200) NOT NULL,
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_CentrosID] PRIMARY KEY CLUSTERED ([clave] ASC)
);
INSERT INTO dbo.clinica (clave,token,idnotificacion,fechahora,admusuario) VALUES ('9999','tenNDKnFbgbXD7Ajplvj3ol498Z7Fr','KUE0CjBKpz5SPgJQuyAVxnmevio68J','2020-01-01','SiacMTG');

CREATE TABLE [dbo].[usuariosclinica](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idclinica] [int] NOT NULL,
	[nombreclinica] [varchar](MAX) NOT NULL DEFAULT 'CLINICA EJEMPLO'/*'--'*/,
	[claveclinica] [varchar](MAX) NOT NULL DEFAULT 'ABCDE123456'/*'--'*/,
	[direccion] [varchar](MAX) NOT NULL DEFAULT 'LA DIRECCION #123'/*'--'*/,
	[cp] [int] NOT NULL DEFAULT 20000/*0*/,
	[telefono] [float] NOT NULL DEFAULT 3131234567/**/,
	[colonia] [varchar](MAX) NOT NULL DEFAULT 'COLONIA'/*'--'*/,
	[localidad] [varchar](MAX) NOT NULL DEFAULT 'LOCALIDAD'/*'--'*/,
	[estadoindx] [varchar](50) NOT NULL DEFAULT '1'/*'--'*/,
	[municipioindx] [varchar](50) NOT NULL DEFAULT '1'/*'--'*/,
	[estado] [varchar](MAX) NOT NULL DEFAULT 'COLIMA'/*'--'*/,
	[municipio] [varchar](MAX) NOT NULL DEFAULT 'COLIMA'/*'--'*/,
	[logopersonalizado] [bit] NOT NULL DEFAULT 'False',
	[nombredirector] [varchar](MAX) NOT NULL DEFAULT 'SALVATORE LEONE'/*'--'*/,
	[siglalegal] [varchar](10) NOT NULL,
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_UsuarioClinicaID] PRIMARY KEY CLUSTERED ([id] ASC)
);
INSERT INTO dbo.usuariosclinica (idclinica,siglalegal,fechahora,admusuario) VALUES ('1','AA','2017-08-09','SiuraMTG');

CREATE TABLE [dbo].[usuarios](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[usuario] [varchar](200) NOT NULL,
	[tokenusuario] [varchar](200) NOT NULL,
	[tokenclinica] [varchar](200) NOT NULL,
	[idperfil] [int] NOT NULL DEFAULT 0,
	[nombre] [varchar](MAX) NOT NULL,
	[apellido] [varchar](MAX) NOT NULL,
	[correo] [varchar](200) NOT NULL,
	[pass] [varchar](200) NOT NULL,
	[administrador] [bit] NOT NULL DEFAULT 'False',
	[activo] [int] NOT NULL DEFAULT 1,
	[estatus] [int] NOT NULL DEFAULT 1,
	[idnotificacion] [varchar](200) NOT NULL,
	[tipo] [varchar](200) NOT NULL,
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_UsuarioID] PRIMARY KEY CLUSTERED ([id] ASC)
);
INSERT INTO dbo.usuarios (usuario,tokenusuario,tokenclinica,nombre,apellido,correo,pass,administrador,idnotificacion,tipo,fechahora,admusuario) VALUES ('adm','75996de9e8471c8a7dd7b05ff064b34d','tenNDKnFbgbXD7Ajplvj3ol498Z7Fr','Admin','Siac','correo@mail.com','202cb962ac59075b964b07152d234b70','true','56789','A','2017-08-09','SiacMTG');

CREATE TABLE [dbo].[usuarioinfo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idusuario] [int] NOT NULL,
	[idclinica] [int] NOT NULL,
	[direccion] [varchar](200) NOT NULL DEFAULT '--',
	[telefono] [float] NOT NULL DEFAULT 0,
	[celular] [float] NOT NULL DEFAULT 0,
	[imagenusuario] [bit] NOT NULL DEFAULT 'False',
	[idespecialidad] [int] NOT NULL DEFAULT 0,
	[consultorio] [varchar](200) NOT NULL DEFAULT '--',
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_UsuarioInfoID] PRIMARY KEY CLUSTERED ([id] ASC)
);
INSERT INTO dbo.usuarioinfo (idusuario,idclinica,fechahora,admusuario) VALUES (1,1,'2017-08-09','SiacMTG');

CREATE TABLE [dbo].[especialidades](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idclinica] [int] NOT NULL,
	[nombre] [varchar](200) NOT NULL,
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_EspecialidadID] PRIMARY KEY CLUSTERED ([id] ASC)
);
INSERT INTO dbo.especialidades (idclinica,nombre,fechahora,admusuario) VALUES (1,'Medicina General','2017-08-09','SiacMTG');
INSERT INTO dbo.especialidades (idclinica,nombre,fechahora,admusuario) VALUES (1,'Cardiologia','2017-08-09','SiacMTG');

CREATE TABLE [dbo].[horariosmedicos](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idclinica] [int] NOT NULL,
	[idusuario] [int] NOT NULL,
	[lunes] [varchar](200) NOT NULL DEFAULT '--',
	[martes] [varchar](200) NOT NULL DEFAULT '--',
	[miercoles] [varchar](200) NOT NULL DEFAULT '--',
	[jueves] [varchar](200) NOT NULL DEFAULT '--',
	[viernes] [varchar](200) NOT NULL DEFAULT '--',
	[sabado] [varchar](200) NOT NULL DEFAULT '--',
	[domingo] [varchar](200) NOT NULL DEFAULT '--',
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_HorarioMedicoID] PRIMARY KEY CLUSTERED ([id] ASC)
);

CREATE TABLE [dbo].[citasregistros](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idclinica] [int] NOT NULL,
	[idusuario] [int] NOT NULL,
	[idpaciente] [int] NOT NULL DEFAULT 0,
	[nombrepaciente] [varchar](200) NOT NULL,
	[horacita] [varchar](200) NOT NULL,
	[fechacita] [datetime] NOT NULL,
	[fechahoracita] [datetime] NOT NULL,
	[correo] [varchar](200) NOT NULL,
	[pagada] [bit] NOT NULL DEFAULT 'False',
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_CitasRegistrosID] PRIMARY KEY CLUSTERED ([id] ASC)
);

CREATE TABLE [dbo].[pacientes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idclinica] [int] NOT NULL,
	[nombre] [varchar](200) NOT NULL,
	[apellidop] [varchar](200) NOT NULL,
	[apellidom] [varchar](200) NOT NULL,
	[telefono] [float] NOT NULL,
	[correo] [varchar](200) NOT NULL,
	[activo] [bit] NOT NULL DEFAULT 'True',
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_PacienteID] PRIMARY KEY CLUSTERED ([id] ASC)
);

CREATE TABLE [dbo].[pagosconsultas](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[idclinica] [int] NOT NULL,
	[idconsulta] [int] NOT NULL,
	[montopago] [float] NOT NULL,
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_PagosConsultasID] PRIMARY KEY CLUSTERED ([id] ASC)
);
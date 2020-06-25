/*
	::: BASE DE DATOS DE SISTEMA SIAC VER 1.0 :::
*/

If(db_id(N'siac') IS NULL) 
	CREATE DATABASE siac;

USE siac;

DROP TABLE IF EXISTS dbo.clinica;
DROP TABLE IF EXISTS dbo.usuarios;

CREATE TABLE [dbo].[clinica](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[clave] [varchar](200) NOT NULL,
	[token] [varchar](200) NOT NULL,
	[idnotificacion] [varchar](200) NOT NULL,
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_CentrosID] PRIMARY KEY CLUSTERED ([clave] ASC)
);
INSERT INTO centros (clave,token,idnotificacion,fechahora,admusuario) VALUES ('9999','tenNDKnFbgbXD7Ajplvj3ol498Z7Fr','KUE0CjBKpz5SPgJQuyAVxnmevio68J','2020-01-01','SiacMTG');

CREATE TABLE [dbo].[usuarios](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[usuario] [varchar](200) NOT NULL,
	[tokenusuario] [varchar](200) NOT NULL,
	[tokenclinica] [varchar](200) NOT NULL,
	[nombre] [varchar](200) NOT NULL,
	[apellido] [varchar](200) NOT NULL,
	[correo] [varchar](200) NOT NULL,
	[pass] [varchar](200) NOT NULL,
	[administrador] [bit] NOT NULL DEFAULT 'False',
	[activo] [int] NOT NULL DEFAULT 1,
	[estatus] [int] NOT NULL DEFAULT 1,
	[idnotificacion] [varchar](200) NOT NULL,
	[fechahora] [datetime] NULL,
	[admusuario] [varchar](50) NULL,
		CONSTRAINT [PK_UsuarioID] PRIMARY KEY CLUSTERED ([id] ASC)
);
INSERT INTO usuarios (usuario,tokenusuario,tokenclinica,nombre,apellido,correo,pass,administrador,idnotificacion,fechahora,admusuario) VALUES ('adm','75996de9e8471c8a7dd7b05ff064b34d','tenNDKnFbgbXD7Ajplvj3ol498Z7Fr','Admin','Siac','correo@mail.com','202cb962ac59075b964b07152d234b70','true','56789','2017-08-09','SiacMTG');
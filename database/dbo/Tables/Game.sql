﻿CREATE TABLE [dbo].[Game] (
	[Id]			bigint NOT NULL PRIMARY KEY Identity,
	[GameId]		bigint NOT NULL,
	[IsFinished]	bit NOT NULL DEFAULT 0,
	[Event]			nvarchar(50) NULL,
	[Site]			nvarchar(50) NULL,
	[Date]			nvarchar(50) NULL,
	[Round]			nvarchar(50) NULL,
	[White]			nvarchar(50) NULL,
	[Black]			nvarchar(50) NULL,
	[Result]		nvarchar(5) NULL,
	[ECO]			int NULL,
	[WhiteElo]		int NULL,
	[BlackElo]		int NULL,
	[NaturalKey]	nvarchar(50) NULL,
	[FileName]		nvarchar(150) NULL,
	[Annotator]		nvarchar(50) NULL,
	[Source]		nvarchar(50) NULL,
	[Remark]		nvarchar(max) NULL,
	[PGN]			nvarchar(1000) NULL,
	[FEN]			nvarchar(100) NOT NULL,
    [IsActive]    BIT              CONSTRAINT [DF_Client_IsActive] DEFAULT ((1)) NULL,
    [CreatedById] BIGINT           NULL,
    [CreatedDate] DATETIME         NULL,
    [UpdatedById] BIGINT           NULL,
    [UpdatedDate] DATETIME         NULL,
    CONSTRAINT [PK_Game] PRIMARY KEY CLUSTERED ([Id] ASC)
)
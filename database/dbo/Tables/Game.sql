CREATE TABLE [dbo].[Game] (
    [Id]          BIGINT          IDENTITY (1, 1) NOT NULL,
    [IsFinished]  BIT             CONSTRAINT [DF__MetaData__IsFini__239E4DCF] DEFAULT ((0)) NOT NULL,
    [Event]       NVARCHAR (50)   NULL,
    [Site]        NVARCHAR (50)   NULL,
    [Date]        NVARCHAR (50)   NULL,
    [Round]       NVARCHAR (50)   NULL,
    [White]       NVARCHAR (50)   NULL,
    [Black]       NVARCHAR (50)   NULL,
    [Result]      NVARCHAR (8)    NULL,
    [ECO]         INT             NULL,
    [WhiteElo]    INT             NULL,
    [BlackElo]    INT             NULL,
    [NaturalKey]  NVARCHAR (50)   NULL,
    [FileName]    NVARCHAR (150)  NULL,
    [Annotator]   NVARCHAR (50)   NULL,
    [Source]      NVARCHAR (50)   NULL,
    [Remark]      NVARCHAR (MAX)  NULL,
    [PGN]         NVARCHAR (3000) NULL,
    [FEN]         NVARCHAR (100)  NOT NULL,
    [IsActive]    BIT             CONSTRAINT [DF_Client_IsActive] DEFAULT ((1)) NULL,
    [CreatedById] BIGINT          NULL,
    [CreatedDate] DATETIME        NULL,
    [UpdatedById] BIGINT          NULL,
    [UpdatedDate] DATETIME        NULL,
    CONSTRAINT [PK__MetaData__3214EC07C6DD192B] PRIMARY KEY CLUSTERED ([Id] ASC)
);




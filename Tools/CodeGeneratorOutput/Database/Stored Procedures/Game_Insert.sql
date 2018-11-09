CREATE PROCEDURE [dbo].[Game_Insert] (
	@isFinished bit,
	@event nvarchar(50),
	@site nvarchar(50),
	@date nvarchar(50),
	@round nvarchar(50),
	@white nvarchar(50),
	@black nvarchar(50),
	@result nvarchar(8),
	@eCO int,
	@whiteElo int,
	@blackElo int,
	@naturalKey nvarchar(50),
	@fileName nvarchar(150),
	@annotator nvarchar(50),
	@source nvarchar(50),
	@remark nvarchar(max),
	@pGN nvarchar(1000),
	@fEN nvarchar(100),
	@isActive bit,
	@createdById bigint,
	@createdDate datetime,
	@updatedById bigint,
	@updatedDate datetime,
	@id bigint OUTPUT
) AS
	INSERT INTO [dbo].[Game]	(
		[IsFinished], [Event], [Site], [Date], [Round], [White], [Black], [Result], [ECO], [WhiteElo], [BlackElo], [NaturalKey], [FileName], [Annotator], [Source], [Remark], [PGN], [FEN], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]
	)
	VALUES (
		@isFinished, @event, @site, @date, @round, @white, @black, @result, @eCO, @whiteElo, @blackElo, @naturalKey, @fileName, @annotator, @source, @remark, @pGN, @fEN, @isActive, @createdById, @createdDate, @updatedById, @updatedDate
	)
	set @id = Scope_Identity()

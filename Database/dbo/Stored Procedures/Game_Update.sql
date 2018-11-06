CREATE PROCEDURE [dbo].[Game_Update]
	@id bigint, @isFinished bit, @event nvarchar(50), @site nvarchar(50), @date nvarchar(50), @round nvarchar(50), @white nvarchar(50), @black nvarchar(50), @result nvarchar(5), @eCO int, @whiteElo int, @blackElo int, @naturalKey nvarchar(50), @fileName nvarchar(150), @annotator nvarchar(50), @source nvarchar(50), @remark nvarchar(max), @pGN nvarchar(1000), @fEN nvarchar(100), @isActive bit, @updatedById bigint, @updatedDate datetime
AS
	UPDATE [dbo].[Game]
	SET	
		[IsFinished] = @isFinished,
		[Event] = @event,
		[Site] = @site,
		[Date] = @date,
		[Round] = @round,
		[White] = @white,
		[Black] = @black,
		[Result] = @result,
		[ECO] = @eCO,
		[WhiteElo] = @whiteElo,
		[BlackElo] = @blackElo,
		[NaturalKey] = @naturalKey,
		[FileName] = @fileName,
		[Annotator] = @annotator,
		[Source] = @source,
		[Remark] = @remark,
		[PGN] = @pGN,
		[FEN] = @fEN,
		[IsActive] = @isActive,
		[UpdatedById] = @updatedById,
		[UpdatedDate] = @updatedDate
	WHERE Id = @id
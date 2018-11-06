CREATE Function [dbo].[Game_ReadAll] (@readActive bit, @readInactive bit)
RETURNS TABLE
AS
return 
	select 
		[Id], [IsFinished], [Event], [Site], [Date], [Round], [White], [Black], [Result], [ECO], [WhiteElo], [BlackElo], [NaturalKey], [FileName], [Annotator], [Source], [Remark], [PGN], [FEN], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate] 
	from [Game]
	where 
		([IsActive] = 1 and @readActive = 1)
		or
		([IsActive] = 0 and @readInactive = 1)
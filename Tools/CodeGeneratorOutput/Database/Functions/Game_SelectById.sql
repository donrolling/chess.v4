CREATE Function [dbo].[Game_SelectById] (@id bigint) RETURNS TABLE AS
	return 
		select top 1 
			[Id], [IsFinished], [Event], [Site], [Date], [Round], [White], [Black], [Result], [ECO], [WhiteElo], [BlackElo], [NaturalKey], [FileName], [Annotator], [Source], [Remark], [PGN], [FEN], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]	
		from [Game] 
		where Id = @id
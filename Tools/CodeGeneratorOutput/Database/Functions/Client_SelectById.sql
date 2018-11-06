CREATE Function [Membership].[Client_SelectById] (@id bigint) RETURNS TABLE AS
	return 
		select top 1 
			[Id], [Name], [Code], [Secret], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]	
		from [Client] 
		where Id = @id
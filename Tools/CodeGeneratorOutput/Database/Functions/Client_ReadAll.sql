CREATE Function [Membership].[Client_ReadAll] (@readActive bit, @readInactive bit)
RETURNS TABLE
AS
return 
	select 
		[Id], [Name], [Code], [Secret], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate] 
	from [Client]
	where 
		([IsActive] = 1 and @readActive = 1)
		or
		([IsActive] = 0 and @readInactive = 1)
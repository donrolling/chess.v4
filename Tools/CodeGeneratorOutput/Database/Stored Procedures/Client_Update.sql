CREATE PROCEDURE [Membership].[Client_Update]
	@id bigint, @name nvarchar(50), @code uniqueidentifier, @secret nvarchar(250), @isActive bit, @updatedById bigint, @updatedDate datetime
AS
	UPDATE [Membership].[Client]
	SET	
		[Name] = @name,
		[Code] = @code,
		[Secret] = @secret,
		[IsActive] = @isActive,
		[UpdatedById] = @updatedById,
		[UpdatedDate] = @updatedDate
	WHERE Id = @id
CREATE PROCEDURE [dbo].[User_Update]
	@id bigint, @guid uniqueidentifier, @email nvarchar(150), @password nvarchar(150), @salt nvarchar(150), @isActive bit, @updatedById bigint, @updatedDate datetime
AS
	UPDATE [dbo].[User]
	SET	
		[Guid] = @guid,
		[Email] = @email,
		[Password] = @password,
		[Salt] = @salt,
		[IsActive] = @isActive,
		[UpdatedById] = @updatedById,
		[UpdatedDate] = @updatedDate
	WHERE Id = @id
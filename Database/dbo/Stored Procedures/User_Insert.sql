CREATE PROCEDURE [dbo].[User_Insert] (
	@guid uniqueidentifier,
	@email nvarchar(150),
	@password nvarchar(150),
	@salt nvarchar(150),
	@isActive bit,
	@createdById bigint,
	@createdDate datetime,
	@updatedById bigint,
	@updatedDate datetime,
	@id bigint OUTPUT
) AS
	INSERT INTO [dbo].[User]	(
		[Guid], [Email], [Password], [Salt], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]
	)
	VALUES (
		@guid, @email, @password, @salt, @isActive, @createdById, @createdDate, @updatedById, @updatedDate
	)
	set @id = Scope_Identity()

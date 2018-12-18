CREATE PROCEDURE [dbo].[User_Insert] (
	@guid uniqueidentifier,
	@email nvarchar(150),
	@password nvarchar(150),
	@salt nvarchar(150),
	@verification nvarchar(150),
	@isActive bit,
	@createdById bigint,
	@createdDate datetime,
	@updatedById bigint,
	@updatedDate datetime,
	@id bigint OUTPUT
) AS
	INSERT INTO [dbo].[User]	(
		[Guid], [Email], [Password], [Salt], [Verification], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]
	)
	VALUES (
		@guid, @email, @password, @salt, @verification, @isActive, @createdById, @createdDate, @updatedById, @updatedDate
	)
	set @id = Scope_Identity()

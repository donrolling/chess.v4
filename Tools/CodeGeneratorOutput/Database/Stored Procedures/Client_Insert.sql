CREATE PROCEDURE [Membership].[Client_Insert] (
	@name nvarchar(50),
	@code uniqueidentifier,
	@secret nvarchar(250),
	@isActive bit,
	@createdById bigint,
	@createdDate datetime,
	@updatedById bigint,
	@updatedDate datetime,
	@id bigint OUTPUT
) AS
	INSERT INTO [Membership].[Client]	(
		[Name], [Code], [Secret], [IsActive], [CreatedById], [CreatedDate], [UpdatedById], [UpdatedDate]
	)
	VALUES (
		@name, @code, @secret, @isActive, @createdById, @createdDate, @updatedById, @updatedDate
	)
	set @id = Scope_Identity()
